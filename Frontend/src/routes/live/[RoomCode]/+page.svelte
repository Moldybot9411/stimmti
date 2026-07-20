<script lang="ts">
	import { page } from '$app/state';
	import AvatarCustomizer from '$lib/components/AvatarCustomizer.svelte';
	import CustomizableAvatar, {
		type CustomizableAvatarSettings,
	} from '$lib/components/CustomizableAvatar.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { getContext } from 'svelte';
	import { Play, Save, Users } from '@lucide/svelte';
	import type { SessionContext } from './+layout.svelte';
	import QRCode from 'qrcode';
	import UserAvatar from '$lib/components/UserAvatar.svelte';
	import { ParticipantRole } from '$lib/wsClient/Backend.Models.Enums.js';
	import { addToast } from '$lib/components/Toast/Toast.svelte';

	let { data, params } = $props();
	let hub = $derived(data.hub);

	let presenterCanvas: HTMLCanvasElement | undefined = $state();
	let modalCanvas: HTMLCanvasElement | undefined = $state();
	let fullscreenQRModal: HTMLDialogElement | undefined = $state();

	let avatarSettings: CustomizableAvatarSettings | undefined = $derived(
		hub.state?.userInformation?.profilePicture
	);
	let participantName: string | undefined = $derived(hub.state?.userInformation?.name);

	$effect(() => {
		const url = page.url.href;
		if (presenterCanvas) {
			QRCode.toCanvas(presenterCanvas, url);
		}
		if (modalCanvas) {
			QRCode.toCanvas(modalCanvas, url, { width: 512 });
		}
	});

	async function updateParticipant() {
		let res = await hub.updateParticipantData(participantName, avatarSettings);

		if (res) {
			addToast({ label: 'Successfully updated profile data', type: 'success' });
			return;
		}

		addToast({ label: 'Error updating profile data', type: 'error' });
	}

	const { startSession } = getContext<SessionContext>('session');
</script>

{#snippet participantsList()}
	<div class="card bg-base-100 shadow-md">
		<div class="card-body">
			<h2 class="card-title">
				<Users class="h-5 w-5" />
				Participants
				<div class="ml-1 badge badge-outline">{hub.state?.participants.length ?? 0}</div>
			</h2>
			{#if !hub.state?.participants.length}
				<p class="py-6 text-center text-sm opacity-50">Waiting for participants to join…</p>
			{:else}
				<div class="mt-2 grid grid-cols-1 gap-2 sm:grid-cols-2">
					{#each hub.state?.participants as participant}
						<div class="flex items-center gap-3 rounded-box bg-base-200 p-2">
							<div class="shrink-0">
								<CustomizableAvatar
									size={10}
									settings={participant.profilePicture} />
							</div>
							<span class="min-w-0 flex-1 truncate font-medium">
								{participant.name}
							</span>
						</div>
					{/each}
				</div>
			{/if}
		</div>
	</div>
{/snippet}

{#snippet customizationCard()}
	<div class="card bg-base-100 shadow-md">
		<div class="card-body">
			<h2 class="card-title">Your Profile</h2>

			<div class="mt-2 flex flex-col items-center">
				<AvatarCustomizer bind:settings={avatarSettings} />
			</div>
			<form class="mt-4" onsubmit={updateParticipant}>
				<fieldset class="fieldset">
					<legend class="fieldset-legend">Username</legend>
					<input
						type="text"
						class="input w-full"
						bind:value={participantName}
						maxlength="64" />
				</fieldset>
				<button class="btn mt-3 btn-block btn-outline btn-neutral">
					<Save class="h-4 w-4" />
					Save Changes
				</button>
			</form>
		</div>
	</div>
{/snippet}

<div class="navbar border-b border-base-300 bg-base-100 px-4">
	<div class="navbar-start gap-2">
		<span class="text-lg font-bold">Stimmti</span>
		<div class="divider mx-1 divider-horizontal h-5 self-center"></div>
		<span class="text-sm opacity-50">Session Lobby</span>
	</div>
	<div class="navbar-center font-bold opacity-80">
		{hub.state?.sessionName}
	</div>
	<div class="navbar-end">
		<ThemeToggle />
	</div>
</div>

<!-- PRESENTER VIEW -->
{#if hub.state?.role == ParticipantRole.Presenter}
	<div class="mx-auto w-full max-w-4xl space-y-4 p-4 md:p-6">
		<div class="card bg-base-100 shadow-md">
			<div class="card-body">
				<div class="flex flex-col items-center gap-6 md:flex-row">
					<button
						class="btn h-auto shrink-0 rounded-xl btn-ghost p-2"
						aria-label="Open fullscreen QR code"
						onclick={() => fullscreenQRModal?.showModal()}>
						<canvas
							bind:this={presenterCanvas}
							style="image-rendering: pixelated; width: 128px; height: 128px; display: block;">
						</canvas>
					</button>

					<div class="divider m-0 divider-vertical self-stretch md:divider-horizontal">
					</div>

					<div class="flex flex-1 flex-col gap-3 text-center md:text-left">
						<div>
							<p class="text-xs font-semibold tracking-widest uppercase opacity-50">
								Join at
							</p>
							<p class="text-base font-bold">{page.url.origin}/live</p>
						</div>
						<div class="divider my-0"></div>
						<div>
							<p class="text-xs font-semibold tracking-widest uppercase opacity-50">
								Room Code
							</p>
							<p class="font-mono text-5xl font-black tracking-widest text-primary">
								{params.RoomCode}
							</p>
						</div>
					</div>
				</div>
			</div>
		</div>

		<div class="card bg-base-100 shadow-md">
			<div
				class="card-body flex-col flex-wrap items-center justify-between gap-4 md:flex-row">
				<div class="flex min-w-0 items-center gap-3">
					<div class="shrink-0">
						<UserAvatar
							size={12}
							profilePictureUrl={hub.state?.presenter.profilePictureUrl} />
					</div>
					<div class="min-w-0">
						<p class="truncate text-lg font-bold">{hub.state?.presenter.displayName}</p>
						<p class="truncate text-base-content/60">
							@{hub.state?.presenter.userName}
						</p>
						<div class="mt-2 badge badge-sm badge-info">Presenter</div>
					</div>
				</div>
				<button class="btn w-full btn-lg btn-primary md:w-fit" onclick={startSession}>
					<Play />
					Start Session
				</button>
			</div>
		</div>

		{@render participantsList()}
	</div>

	<!-- PARTICIPANT VIEW -->
{:else if hub.state?.role == ParticipantRole.Participant}
	<div class="mx-auto w-full max-w-4xl p-4 md:p-6">
		<div class="grid gap-4 md:grid-cols-2">
			{@render customizationCard()}
			{@render participantsList()}
		</div>
	</div>
{/if}

<dialog class="modal modal-bottom md:modal-middle" bind:this={fullscreenQRModal}>
	<div class="modal-box flex w-full max-w-lg flex-col items-center gap-4 overflow-hidden p-6">
		<h3 class="shrink-0 text-lg font-bold">Scan to Join</h3>
		<canvas
			bind:this={modalCanvas}
			class="block min-h-0 flex-1"
			style="image-rendering: pixelated; aspect-ratio: 1 / 1; max-width: 100%; width: auto;">
		</canvas>
		<p class="shrink-0 text-center text-sm opacity-60">
			Or visit <span class="font-bold">{page.url.origin}/live</span> and enter room code
			<span class="font-mono font-bold text-primary">{params.RoomCode}</span>
		</p>
		<div class="modal-action w-full shrink-0">
			<form method="dialog" class="w-full">
				<button class="btn btn-block btn-outline">Close</button>
			</form>
		</div>
	</div>
	<form method="dialog" class="modal-backdrop">
		<button>close</button>
	</form>
</dialog>
