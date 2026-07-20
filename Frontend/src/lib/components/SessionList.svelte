<script lang="ts">
	import type { PaginatedSessionListDto } from '$lib/api';
	import { ChevronRight, SquareKanban, Users } from '@lucide/svelte';
	import Pagination from './Pagination.svelte';
	import { apiClient } from '$lib/apiClient';
	import { addToast } from './Toast/Toast.svelte';

	type Props = {
		sessionData?: PaginatedSessionListDto;
	};

	let { sessionData = { sessionCount: 0, sessionListInfo: [] } }: Props = $props();

	const itemsPerPage = 15;
	let currentPage = $state(1);
	let loading = $state(false);

	function onPageChange(index: number) {
		loading = true;
		apiClient.api
			.v1SessionGetSessionListList({ pageSize: itemsPerPage, currentPage: index })
			.then((res) => {
				if (res.status === 200) {
					sessionData = res.data;
				}
			})
			.catch(() => {
				addToast({ label: 'Error fetching sessions', type: 'error' });
			})
			.finally(() => (loading = false));
	}
</script>

{#if sessionData.sessionCount === 0}
	<span class="font-bold text-base-content/60">No sessions yet</span>
{:else}
	<ul class="menu w-full gap-2 rounded-box" data-sveltekit-preload-data="tap">
		{#if loading}
			{#each new Array(itemsPerPage)}
				<li>
					<div
						class="flex skeleton flex-col items-start gap-2 p-4 active:bg-base-200 md:flex-row md:items-center md:justify-between md:p-2">
						<div class="flex w-full items-center gap-2 md:w-auto">
							<SquareKanban class="shrink-0" size={20} />

							<div class="h-5.25 min-w-20 skeleton truncate pr-4"></div>
						</div>

						<div class="flex w-full items-center justify-between gap-4 md:justify-end">
							<div class="flex flex-wrap items-center gap-2">
								<span
									class="badge min-w-15 skeleton badge-soft badge-sm badge-neutral"
								></span>

								<span class="badge min-w-15 skeleton badge-sm"></span>

								<span
									class="badge min-w-15 skeleton badge-soft badge-sm badge-accent"
								></span>
							</div>

							<ChevronRight size={16} class="shrink-0 text-base-content" />
						</div>
					</div>
				</li>
			{/each}
		{:else}
			{#each sessionData?.sessionListInfo as session}
				<li>
					<a
						href="/app/sessions/{session.id}"
						class="flex flex-col items-start gap-2 p-4 active:bg-base-200 md:flex-row md:items-center md:justify-between md:p-2">
						<div class="flex w-full items-center gap-2 md:w-auto">
							<SquareKanban class="shrink-0" size={20} />

							<span class="truncate pr-4 text-base-content">
								{session.name}
							</span>
						</div>

						<div class="flex w-full items-center justify-between gap-4 md:justify-end">
							<div class="flex flex-wrap items-center gap-2">
								<span class="badge badge-soft badge-sm badge-neutral">
									<Users size={12} />
									{session.participantCount}
								</span>

								<span class="badge badge-sm">
									{Intl.DateTimeFormat(undefined, { dateStyle: 'medium' }).format(
										new Date(session.openedAt)
									)}
								</span>

								<span class="badge badge-soft badge-sm badge-accent">
									{Intl.DateTimeFormat(undefined, { timeStyle: 'short' }).format(
										new Date(session.openedAt)
									)}
								</span>
							</div>

							<ChevronRight size={16} class="shrink-0 text-base-content" />
						</div>
					</a>
				</li>
			{/each}
		{/if}
	</ul>

	<ul>
		<div class="divider"></div>
		<Pagination
			numPages={Math.ceil(sessionData?.sessionCount / itemsPerPage)}
			bind:currentPage
			onpagechange={onPageChange} />
	</ul>
{/if}
