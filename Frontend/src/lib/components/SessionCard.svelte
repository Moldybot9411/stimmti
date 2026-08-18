<script lang="ts">
	import { goto } from '$app/navigation';
	import { ChartNoAxesCombined, User } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		sessionId?: string;
		title?: string;
		participantCount?: number;
		runAt?: Date;
		class?: ClassValue;
		style?: string;
	};

	let { sessionId, title, participantCount, runAt, class: classes, style }: Props = $props();

	let color = $state('bg-primary');
	let btn_color = $state('btn-primary');

	$effect.pre(() => {
		if (!title) return;

		const colors = [
			'bg-primary',
			'bg-accent',
			'bg-info',
			'bg-success',
			'bg-warning',
			'bg-error',
		];

		const btn_colors = [
			'btn-primary',
			'btn-accent',
			'btn-info',
			'btn-success',
			'btn-warning',
			'btn-error',
		];

		let hash = 0;
		for (let i = 0; i < title.length; i++) {
			hash = title.charCodeAt(i) + ((hash << 5) - hash);
			const index = Math.abs(hash) % colors.length;
			color = colors[index];
			btn_color = btn_colors[index];
		}
	});
</script>

<div
	class={['card min-w-70 overflow-hidden bg-base-100 shadow-sm card-md md:min-w-96', classes]}
	{style}>
	<div class={['min-h-2 w-full rounded-b-sm', color]}></div>

	<div class="card-body">
		<div class="card-title flex-col items-start gap-0">
			{title}
		</div>

		<div class="divider m-0"></div>

		{#if participantCount !== undefined || runAt}
			<div class="mt-auto flex justify-between gap-2">
				{#if participantCount !== undefined}
					<div class="flex flex-col items-start">
						Participants
						<kbd class="kbd w-fit">{participantCount}</kbd>
					</div>
				{/if}

				{#if runAt}
					<div class="flex flex-col items-end">
						Run at
						<kbd class="kbd w-fit">
							{Intl.DateTimeFormat(undefined, { dateStyle: 'medium' }).format(runAt)}
						</kbd>
					</div>
				{/if}
			</div>
		{/if}

		<button
			class={['btn btn-block shadow', btn_color]}
			onclick={() => goto(`/app/sessions/${sessionId}`)}>
			<ChartNoAxesCombined />
			Open
		</button>
	</div>
</div>
