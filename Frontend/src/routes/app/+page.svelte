<script lang="ts">
	import { authStore } from '$lib/authStore.svelte';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import SessionCard from '$lib/components/SessionCard.svelte';
	import {
		Activity,
		ArrowRight,
		BadgePlus,
		BadgeQuestionMark,
		BookDashed,
		CirclePlus,
		LoaderCircle,
		MessagesSquare,
		X,
		Zap,
		type LucideIcon,
	} from '@lucide/svelte';
	import { createQuickPollTemplate } from '$lib/templates/quickPollTemplate.js';
	import { createFeedbackFormTemplate } from '$lib/templates/feedbackFormTemplate.js';
	import { createTeamPulseTemplate } from '$lib/templates/teamPulseTemplate.js';
	import { apiClient } from '$lib/apiClient.js';
	import { goto } from '$app/navigation';
	import { addToast } from '$lib/components/Toast/Toast.svelte';

	let { data } = $props();

	let templateLoading = $state(false);
	let templates = [
		{
			label: 'Quick Poll',
			description: 'A simple single-question poll to gauge immediate reactions.',
			icon: Zap,
			createFunction: createQuickPollTemplate,
		},
		{
			label: 'Feedback Form',
			description:
				'Collect detailed Feedback after a meeting or an event with 10 insightful questions.',
			icon: MessagesSquare,
			createFunction: createFeedbackFormTemplate,
		},
		{
			label: 'Team Pulse',
			description: 'Check in on team morale, workload, and project confidence',
			icon: Activity,
			createFunction: createTeamPulseTemplate,
		},
	];

	let newSurveyDialogRef: HTMLDialogElement | undefined = $state();
</script>

<div
	class="mt-4 flex w-full flex-col items-center justify-between gap-2 md:mt-16 md:flex-row md:px-16">
	<div class="z-1 max-w-full md:max-w-240">
		<h2 class="truncate text-4xl font-bold">
			Welcome Back,<br />{authStore.user?.displayName}
		</h2>
		<p class="text-lg text-secondary">Gather Insights like never before!</p>
	</div>

	<button
		class="btn w-fit shrink-0 btn-lg btn-primary"
		onclick={() => newSurveyDialogRef?.showModal()}>
		<BadgePlus />
		CREATE NEW SURVEY
	</button>
</div>

<h3 class="mt-16 mb-2 text-2xl font-bold">Quick Start</h3>

<div class="flex gap-4 overflow-auto">
	{#each templates as template}
		{@const Icon = template.icon}

		<div class="card max-w-70 min-w-70 bg-base-100 shadow-sm card-md md:max-w-96 md:min-w-96">
			<div class="card-body">
				<div class="w-fit rounded-sm bg-accent p-2 text-accent-content">
					<Icon />
				</div>
				<h2 class="card-title">{template.label}</h2>
				<p>
					{template.description}
				</p>
				<div class="mt-auto card-actions">
					<button
						disabled={templateLoading}
						class="btn btn-block btn-outline btn-accent"
						onclick={() => {
							templateLoading = true;

							template
								.createFunction()
								.then((res) => {
									goto(`/app/surveys/${res}`);
								})
								.catch((e) => {
									addToast({
										label: 'Error instancing template: ' + e,
										type: 'error',
										icon: BookDashed,
									});
								})
								.finally(() => (templateLoading = false));
						}}>
						{#if templateLoading}
							<LoaderCircle class="animate-spin" />
						{:else}
							<CirclePlus />
						{/if}
						Create
					</button>
				</div>
			</div>
		</div>
	{/each}
</div>

<div class="mt-16 mb-2 flex gap-2">
	<h3 class="text-2xl font-bold">Recent Sessions</h3>
	<a class="btn btn-outline btn-secondary btn-sm" href="/app/library?view=sessions">
		<ArrowRight size={16} />
		View All
	</a>
</div>

<div class="flex gap-4 overflow-auto">
	{#await data.recentSessions}
		{#each new Array(8)}
			<div class="min-h-50 min-w-70 skeleton rounded-box md:min-w-96"></div>
		{/each}
	{:then recentSessionData}
		{#if recentSessionData.sessionListInfo?.length ?? 0 > 0}
			{#each recentSessionData.sessionListInfo as session}
				<SessionCard
					sessionId={session.id}
					title={session.name ?? undefined}
					participantCount={session.participantCount}
					runAt={new Date(session.openedAt)} />
			{/each}
		{:else}
			<div
				class="flex min-h-50 min-w-70 items-center justify-center rounded-box bg-base-100 text-lg font-bold md:min-w-96">
				No recent sessions yet
			</div>
		{/if}
	{/await}
</div>

<NewSurveyDialog bind:ref={newSurveyDialogRef} />
