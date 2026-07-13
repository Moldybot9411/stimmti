<script module lang="ts">
	export interface SessionContext {
		startSession: () => void;
	}
</script>

<script lang="ts">
	import { onMount, setContext } from 'svelte';
	import { fade } from 'svelte/transition';
	import { ParticipantRole, SessionState } from '$lib/wsClient/Backend.Models.Enums.js';
	import { Trophy, User } from '@lucide/svelte';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { type Option } from '$lib/components/MultipleChoice.svelte';
	import QuestionDisplay from '$lib/components/QuestionDisplay.svelte';
	import type { AnswerOptionDto } from '$lib/wsClient/Backend.Dto.js';
	import LoadingScreen from '$lib/components/LoadingScreen.svelte';

	let { children, data } = $props();
	let hub = $derived(data.hub);

	let phase: SessionState = $derived(hub.state?.sessionState ?? SessionState.Lobby);

	let choiceOptions: Option[] = $state([]); // Used for both single and multiple choice
	$effect(() => {
		const answers = hub.state?.currentQuestion?.answerOptions ?? [];

		choiceOptions = answers.map((x) => ({
			answerOption: x,
			checked: false,
		}));
	});

	onMount(() => {
		if (phase === SessionState.Lobby) {
			setFullscreen(false);
			return;
		}

		setFullscreen(true);
	});

	async function startSession() {
		if (hub.state?.participants.length == 0) {
			addToast({
				label: 'There are no participants in this session',
				icon: User,
				type: 'error',
			});
			return;
		}

		let res = await hub.startSession();

		if (!res) {
			addToast({ label: 'Error starting session', type: 'error' });
			return;
		}

		setFullscreen(true);
	}

	async function nextQuestion() {
		let res = await hub.nextQuestion();

		if (!res) {
			addToast({
				label: 'Serious error occured. Try reloading the page',
				type: 'error',
			});
		}
	}

	async function closeSession() {
		let res = await hub.closeSession();

		setFullscreen(false);

		if (!res) {
			addToast({
				label: 'Error closing session. Try reloading the page',
				type: 'error',
			});
			return;
		}
	}

	function setFullscreen(active: boolean) {
		var elem = document.documentElement;

		if (!active) {
			if (document.fullscreenElement) {
				document
					.exitFullscreen()
					.catch((err) => console.error('Error closing fullscreen:', err));
			}

			return;
		}

		var rfs = elem.requestFullscreen;

		if (typeof rfs !== undefined && rfs) {
			rfs.call(elem);
		}
	}

	async function submitAnswer(
		answerOptions?: AnswerOptionDto[],
		wordCloudTexts?: string[],
		text?: string,
		value?: number
	) {
		let res = await hub.submitAnswer(answerOptions, wordCloudTexts, text, value);

		if (!res) {
			addToast({ label: 'Error submitting answer', type: 'error' });
		}
	}

	setContext<SessionContext>('session', { startSession });
</script>

{#if phase !== SessionState.Lobby}
	<div class="navbar border-b border-base-300 bg-base-100 px-4">
		<div class="navbar-start gap-2">
			<span class="text-lg font-bold">Stimmti</span>

			<div class={['badge badge-outline', hub.connected ? 'badge-success' : 'badge-error']}>
				<div
					aria-label={hub.connected ? 'success' : 'error'}
					class={['status', hub.connected ? 'status-success' : 'status-error']}>
				</div>
				{hub.connected ? 'Connected' : 'Disconnected'}
			</div>
		</div>
		<div class="navbar-center font-bold opacity-80">
			{hub.state?.sessionName}
		</div>
		<div class="navbar-end">
			<ThemeToggle />
		</div>
	</div>
{/if}

{#if phase === SessionState.Lobby}
	<div out:fade={{ duration: 300 }}>
		{@render children()}
	</div>
{:else if phase === SessionState.Loading}
	<LoadingScreen />
{:else if phase === SessionState.Question}
	<div
		class="flex flex-col items-center justify-center px-4"
		in:fade={{ duration: 400 }}
		out:fade={{ duration: 400 }}>
		<QuestionDisplay
			questionName={hub.state?.currentQuestion?.name}
			questionDescription={hub.state?.currentQuestion?.description}
			role={hub.state?.role}
			questionType={hub.state?.currentQuestion?.questionType}
			bind:choiceOptions
			scaleMinValue={hub.state?.currentQuestion?.minValue}
			scaleMaxValue={hub.state?.currentQuestion?.maxValue}
			finishedAnsering={hub.state?.answeredThisRound}
			participantCount={hub.state?.participants.length}
			numWordCloudInputs={hub.state?.currentQuestion?.wordCloudMaxWords}
			answers={hub.participantAnswers}
			onNextQuestion={nextQuestion}
			onSubmitAnswer={submitAnswer} />
	</div>
{:else if phase === SessionState.Finished}
	{#if hub.state?.role === ParticipantRole.Presenter}
		<div
			class="absolute top-0 left-0 flex h-screen w-full items-center justify-center"
			in:fade={{ duration: 400 }}
			out:fade={{ duration: 400 }}>
			<div class="card min-w-96 bg-base-100 card-md">
				<div class="card-body items-center gap-2">
					<Trophy class="size-15 text-warning" />

					<span class="text-4xl font-bold">{hub.state.sessionName}</span>

					<span class="text-2xl opacity-80">
						{hub.state.participants.length} Participants
					</span>

					<div class="mt-4 card-actions w-full">
						<button class="btn btn-block btn-primary" onclick={closeSession}>
							Close Session
						</button>
					</div>
				</div>
			</div>
		</div>
	{:else}
		<div class="flex h-screen w-full flex-col items-center justify-center gap-2">
			<span class="text-4xl font-bold">Thanks for participating!</span>
			<span class="text-2xl opacity-80">You can close this tab now</span>
		</div>
	{/if}
{/if}
