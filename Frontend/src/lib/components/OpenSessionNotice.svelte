<script lang="ts">
	import type { GetOpenSessionsDto } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { SquareKanban, X } from '@lucide/svelte';
	import { addToast } from './Toast/Toast.svelte';

	type Props = {
		openSessions?: GetOpenSessionsDto[];
	};

	let { openSessions = [] }: Props = $props();
	// svelte-ignore state_referenced_locally
	var localSessions = $state(openSessions);

	let fixDialog: HTMLDialogElement | undefined = $state();
	let loading = $state(false);
	let openSessionSelect = $state<{ checked: boolean; data: GetOpenSessionsDto }[]>([]);

	$effect(() => {
		openSessionSelect = localSessions.map((x) => ({ checked: false, data: x }));
	});

	function closeAll() {
		loading = true;

		apiClient.api
			.v1SessionCloseSessionBatchPartialUpdate(localSessions.map((x) => x.id))
			.then(async (res) => {
				if (res.status === 200) {
					addToast({ label: 'Closed all sessions', type: 'success', icon: SquareKanban });
					localSessions = [];
					fixDialog?.close();
				}
			})
			.catch((err) => {
				addToast({
					label: 'Error closing all sessions: ' + err,
					type: 'error',
					icon: SquareKanban,
				});
			})
			.finally(() => (loading = false));
	}

	function closeSelected() {
		loading = true;

		let selected = openSessionSelect.filter((x) => x.checked === true).map((x) => x.data.id);

		apiClient.api
			.v1SessionCloseSessionBatchPartialUpdate(selected)
			.then(async (res) => {
				if (res.status === 200) {
					addToast({
						label: 'Closed selected sessions',
						type: 'success',
						icon: SquareKanban,
					});

					localSessions = localSessions.filter(
						(session) => !selected.includes(session.id)
					);

					if (localSessions.length === 0) {
						fixDialog?.close();
					}
				}
			})
			.catch((err) => {
				addToast({
					label: 'Error closing selected sessions: ' + err,
					type: 'error',
					icon: SquareKanban,
				});
			})
			.finally(() => (loading = false));
	}
</script>

{#if localSessions.length > 0}
	<div role="alert" class="mt-2 alert flex gap-2 text-balance alert-warning">
		<SquareKanban class="shrink-0" />
		<span>
			Warning: You still have
			<span class="font-bold">{localSessions.length}</span>
			open sessions. Open sessions aren't shown in your library and are not represented in statistics.
		</span>

		<button class="btn btn-neutral" onclick={() => fixDialog?.showModal()}> Fix </button>
	</div>

	<dialog class="modal" bind:this={fixDialog}>
		<div class="modal-box">
			<form method="dialog">
				<button class="btn absolute top-2 right-2 btn-ghost btn-sm">
					<X />
				</button>
			</form>

			<h3 class="text-lg font-bold">Fix open sessions</h3>

			<button class="btn mt-4 btn-block btn-warning" onclick={closeAll} disabled={loading}>
				Close All
			</button>

			<div class="mt-4 flex flex-col gap-2">
				{#each openSessionSelect as session}
					<div class="flex items-center justify-between gap-2 p-2">
						<label class="cursor-pointer truncate font-bold">
							<input
								type="checkbox"
								class="checkbox"
								bind:checked={session.checked} />

							{session.data.name}
						</label>

						<div class="flex flex-col gap-2 text-base-content/60">
							<span>
								Opened:
								<span class="badge badge-outline badge-secondary">
									{Intl.DateTimeFormat(undefined, {
										dateStyle: 'medium',
										timeStyle: 'short',
									}).format(new Date(session.data.openedAt))}
								</span>
							</span>

							<span>
								Room Code:
								<a
									data-sveltekit-preload-data="tap"
									class="link"
									href="/live/{session.data.roomCode}">
									{session.data.roomCode}
								</a>
							</span>
						</div>
					</div>

					<div class="divider my-0 last:hidden"></div>
				{/each}
			</div>

			<div class="mt-4 flex justify-end gap-2">
				<button class="btn btn-outline btn-secondary" onclick={() => fixDialog?.close()}>
					Cancel
				</button>
				<button
					class="btn btn-primary"
					disabled={!openSessionSelect.some((x) => x.checked === true)}
					onclick={closeSelected}>
					Close Selected
				</button>
			</div>
		</div>
	</dialog>
{/if}
