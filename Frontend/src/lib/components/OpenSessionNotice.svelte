<script lang="ts">
	import type { GetOpenSessionsDto } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { SquareKanban, X } from '@lucide/svelte';
	import { addToast } from './Toast/Toast.svelte';

	type Props = {
		openSessions?: GetOpenSessionsDto[];
	};

	let { openSessions = [] }: Props = $props();

	let fixDialog: HTMLDialogElement | undefined;
	let loading = $state(false);
	let openSessionSelect = $state<{ checked: boolean; data: GetOpenSessionsDto }[]>([]);

	$effect(() => {
		openSessionSelect = openSessions.map((x) => ({ checked: false, data: x }));
	});

	function closeAll() {
		loading = true;

		apiClient.api
			.v1SessionCloseSessionBatchPartialUpdate(openSessions.map((x) => x.id))
			.then((res) => {
				if (res.status === 200) {
					addToast({ label: 'Closed all sessions', type: 'success', icon: SquareKanban });
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
			.then((res) => {
				if (res.status === 200) {
					addToast({
						label: 'Closed selected sessions',
						type: 'success',
						icon: SquareKanban,
					});
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

<div role="alert" class="alert flex gap-2 text-balance alert-warning">
	<SquareKanban class="shrink-0" />
	<span>
		Warning: You still have
		<span class="font-bold">{openSessions.length}</span>
		open sessions. Open sessions aren't shown in your library and are not represented in statistics.
	</span>

	<button class="btn btn-outline btn-neutral" onclick={() => fixDialog?.showModal()}>Fix</button>
</div>

<dialog class="modal" bind:this={fixDialog}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm">
				<X />
			</button>
		</form>

		<h3 class="text-lg font-bold">Fix open sessions</h3>

		<button class="btn mt-4 btn-block btn-warning" onclick={closeAll} disabled={loading}
			>Close All</button>

		<div class="mt-4 flex flex-col gap-2">
			{#each openSessionSelect as session}
				<div class="flex items-center justify-between gap-2 p-2">
					<label class="cursor-pointer truncate font-bold">
						<input type="checkbox" class="checkbox" bind:checked={session.checked} />

						{session.data.name}
					</label>

					<span class="text-base-content/60">
						Opened:
						<span class="badge badge-outline badge-neutral">
							{Intl.DateTimeFormat(undefined, {
								dateStyle: 'medium',
								timeStyle: 'short',
							}).format(new Date(session.data.openedAt))}
						</span>
						<br />
						Room Code: {session.data.roomCode}
					</span>
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
