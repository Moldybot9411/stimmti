<script lang="ts">
	import { goto } from '$app/navigation';
	import type { ProblemDetails } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import HideScrollbar from '$lib/components/HideScrollbar.svelte';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { SessionConnection } from '$lib/signalr.svelte';
	import { ChevronLeft, GitMergeConflict, QrCode } from '@lucide/svelte';
	import axios from 'axios';
	import { onMount } from 'svelte';

	let hub = new SessionConnection();
	let roomCode = $state('');
	$effect(() => {
		roomCode = roomCode.toUpperCase();
	});

	onMount(() => {
		hub.init();
	});

	async function checkSession() {
		if (!roomCode.trim()) addToast({ label: 'No session code provided', type: 'error' });

		try {
			await apiClient.api.v1SessionCheckSessionList({ roomCode });

			goto(`/live/${roomCode.trim()}`);
		} catch (e) {
			if (axios.isAxiosError<ProblemDetails>(e)) {
				let data = e.response?.data;

				if (data?.title && data?.detail) {
					addToast({
						label: `${data.title}: ${data.detail}`,
						type: 'error',
						icon: GitMergeConflict,
					});
				}
			} else {
				addToast({ label: 'Unknown error occured', type: 'error' });
			}
		}
	}
</script>

<HideScrollbar />

<button
	class="btn absolute top-4 left-4 z-10 btn-ghost"
	aria-label="Navigate Back"
	onclick={() => history.back()}>
	<ChevronLeft />
</button>

<div class="flex h-screen w-full items-center justify-center">
	<div class="flex w-fit min-w-96 flex-col items-center justify-center gap-4">
		<div class="card w-full bg-base-100 shadow-sm card-md">
			<div class="card-body">
				<h2 class="mx-auto card-title">Join Session</h2>

				<form class="flex flex-col justify-center" onsubmit={checkSession}>
					<span class="mb-2 font-bold">Room Code</span>

					<label class="otp otp-lg">
						<span></span>
						<span></span>
						<span></span>
						<span></span>
						<span></span>
						<span></span>
						<span></span>
						<input type="text" maxlength="7" required bind:value={roomCode} />
					</label>

					<button class="btn mt-4 btn-block btn-primary" type="submit">Join</button>
				</form>
			</div>
		</div>
	</div>
</div>
