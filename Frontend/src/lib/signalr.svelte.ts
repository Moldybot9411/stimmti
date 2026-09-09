import * as signalR from '@microsoft/signalr';
import type {
	ISessionHub,
	ISessionHubClient,
} from './wsClient/TypedSignalR.Client/Backend.Hubs.Interfaces';
import type {
	AnonymousProfilePictureDto,
	AnswerDisplayDto,
	AnswerOptionDto,
	JoinSessionDto,
	RestoreStateDto,
} from './wsClient/Backend.Dto';
import { getHubProxyFactory, getReceiverRegister } from './wsClient/TypedSignalR.Client';
import { goto } from '$app/navigation';
import { env } from '$env/dynamic/public';
import { ParticipantRole } from './wsClient/Backend.Models.Enums';

export class SessionConnection {
	private readonly baseUrl = env.PUBLIC_API_URL || 'http://localhost:5202';
	private readonly hubUrl = `${this.baseUrl}/defaulthub`;

	private currentRoomCode: string | null = null;
	private readonly subscription: { dispose: () => void };

	readonly connection: signalR.HubConnection;
	readonly sessionHub: ISessionHub;

	connected = $state(false);
	state = $state<RestoreStateDto | null>(null);
	participantAnswers: AnswerDisplayDto | undefined = $state();

	constructor() {
		this.connection = new signalR.HubConnectionBuilder()
			.withUrl(this.hubUrl, { withCredentials: true })
			.withAutomaticReconnect()
			.withStatefulReconnect()
			.build();

		this.sessionHub = getHubProxyFactory('ISessionHub').createHubProxy(this.connection);

		document.addEventListener('visibilitychange', () => {
			if (document.visibilityState === 'visible' && this.currentRoomCode) {
				if (this.connection.state === signalR.HubConnectionState.Connected) {
					this.joinSession(this.currentRoomCode);
				}
			}
		});

		this.connection.onreconnected(async () => {
			if (!this.currentRoomCode) return;
			this.connected = true;

			const success = await this.joinSession(this.currentRoomCode);

			if (!success) {
				goto('/live');
			}
		});

		this.connection.onreconnecting(() => {
			this.connected = false;
		});

		this.connection.onclose(() => {
			this.connected = false;
		});

		const receiver: ISessionHubClient = {
			participantJoined: async (data) => {
				this.state?.participants.push(data);
			},
			participantUpdated: async (data) => {
				let par = this.state?.participants.find((x) => x.name === data.oldName);
				if (!par) return;

				par.name = data.newName;
				par.profilePicture = data.profilePicture;
			},
			sessionStateChanged: async (data) => {
				if (!this.state) return;
				this.state.sessionState = data;
			},
			questionChanged: async (data) => {
				if (!this.state) return;
				this.state.currentQuestion = data;
				this.state.answeredThisRound = false;
				this.participantAnswers = undefined;
			},
			sessionClosed: async () => {
				if (this.state?.role === ParticipantRole.Participant) {
					goto('/live');
					return;
				}

				goto(`/app/sessions/${this.state?.sessionId}`);
			},
			answerSubmitted: async (data) => {
				this.participantAnswers = data;
			},
		};

		this.subscription = getReceiverRegister('ISessionHubClient').register(
			this.connection,
			receiver
		);
	}

	async init() {
		if (this.connected) return;

		await this.connection.start();
		this.connected = true;
	}

	async joinSession(roomCode: string): Promise<boolean> {
		if (!this.connected) {
			await this.init();
		}

		const data: JoinSessionDto = {
			roomCode,
			playerId: localStorage.getItem('anonymousUserId') ?? undefined,
		};

		const result = await this.sessionHub.joinSession(data);

		if (!result) return false;

		this.currentRoomCode = roomCode;
		this.state = result;

		if (result.userInformation) {
			localStorage.setItem('anonymousUserId', result.userInformation.id);
		}

		return true;
	}

	async leaveSession() {
		if (this.currentRoomCode && this.connected) {
			await this.sessionHub.leaveRoom(this.currentRoomCode);
			this.currentRoomCode = null;
			this.state = null;
		}
	}

	async updateParticipantData(name?: string, profilePicture?: AnonymousProfilePictureDto) {
		if (!this.currentRoomCode || !this.connected) return false;

		let anonymousUserId = localStorage.getItem('anonymousUserId');
		if (!anonymousUserId) return false;

		let res = await this.sessionHub.updateParticipantData({
			roomCode: this.currentRoomCode,
			anonymousUserId,
			name,
			profilePicture,
		});

		return res;
	}

	async startSession() {
		if (!this.currentRoomCode || !this.connected) return false;

		let res = await this.sessionHub.startSession(this.currentRoomCode);

		return res;
	}

	async nextQuestion() {
		if (!this.currentRoomCode || !this.connected) return false;

		let res = await this.sessionHub.nextQuestion(this.currentRoomCode);

		return res;
	}

	async closeSession() {
		if (!this.currentRoomCode || !this.connected) return false;

		let res = await this.sessionHub.closeSession(this.currentRoomCode);

		return res;
	}

	async submitAnswer(
		answerOptions?: AnswerOptionDto[],
		wordCloudTexts?: string[],
		text?: string,
		value?: number
	) {
		if (!this.currentRoomCode || !this.connected || !this.state?.userInformation) return false;

		let res = await this.sessionHub.submitAnswer({
			anonymousUserId: this.state.userInformation.id,
			roomCode: this.currentRoomCode,
			answerOptions: answerOptions ?? [],
			text,
			value,
			wordCloudAnswers: wordCloudTexts ?? [],
		});

		if (res) this.state.answeredThisRound = true;

		return res;
	}

	destroy() {
		this.subscription.dispose();
		this.connection.stop();
		this.connected = false;
	}
}
