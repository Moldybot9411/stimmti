<script lang="ts">
	import StartSessionButton from '$lib/components/startSessionButton.svelte';
	import { goto, replaceState } from '$app/navigation';
	import { scrollIntoViewOnMount } from '$lib/actions/scrollaction.js';
	import type { GetSurveyResponseDto } from '$lib/api';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import {
		Archive,
		BadgePlus,
		ChartNoAxesCombined,
		Cog,
		Form,
		Plus,
		Scroll,
		X,
	} from '@lucide/svelte';
	import SessionList from '$lib/components/SessionList.svelte';
	import SurveyList from '$lib/components/SurveyList.svelte';
	import type { View } from './+page.js';
	import { page } from '$app/state';

	let { data } = $props();

	let menuTabs = [
		{ label: 'Surveys', icon: Form },
		{ label: 'Sessions', icon: ChartNoAxesCombined },
	];

	let activeViewId = $state<View>((page.url.searchParams.get('view') ?? 'surveys') as View);

	let editingSurvey = $state<GetSurveyResponseDto | null>(null);
	let newSurveyDialogRef: HTMLDialogElement | undefined = $state();

	function switchView(viewId: View) {
		const url = new URL(page.url);
		url.searchParams.set('view', viewId);
		history.replaceState(history.state, '', url);
		activeViewId = viewId;
	}
</script>

<div class="flex w-full flex-col gap-4 md:flex-row">
	<ul class="menu h-fit w-full bg-base-100 shadow-sm md:sticky md:top-4 md:flex-1">
		<li>
			<h2 class="menu-title">Library</h2>
			<ul>
				{#each menuTabs as tab}
					<li class={activeViewId === tab.label.toLowerCase() ? 'menu-active' : ''}>
						<button onclick={() => switchView(tab.label.toLowerCase() as View)}>
							<tab.icon size={16} />
							{tab.label}
						</button>
					</li>
				{/each}
			</ul>
			<button
				class="btn mt-2 truncate text-nowrap btn-primary"
				onclick={() => newSurveyDialogRef?.showModal()}>
				<BadgePlus />
				New Survey
			</button>
		</li>
	</ul>

	<div class="card flex-5 bg-base-100 shadow-sm card-md">
		<div class="card-body">
			<h2 class="card-title">
				{activeViewId.charAt(0).toUpperCase() + activeViewId.slice(1)}
			</h2>

			{#if activeViewId === 'surveys'}
				{#await Promise.all([data.surveys, data.folder])}
					<ul class="menu w-full gap-2 rounded-box">
						<li class="skeleton p-2">
							<div class="skeleton-text">Loading Surveys...</div>
						</li>
					</ul>
				{:then [surveys, folder]}
					<SurveyList {surveys} {folder} bind:editingSurvey />
				{/await}
			{:else if activeViewId === 'sessions'}
				{#await data.sessions}
					<ul class="menu w-full gap-2 rounded-box">
						<li class="skeleton p-2">
							<div class="skeleton-text">Loading Sessions...</div>
						</li>
					</ul>
				{:then sessionData}
					<SessionList {sessionData} />
				{/await}
			{/if}
		</div>
	</div>

	{#if editingSurvey && activeViewId === 'surveys'}
		<div
			class="card h-fit flex-3 bg-base-100 shadow-sm card-md"
			use:scrollIntoViewOnMount={editingSurvey.surveyId}>
			<div class="card-body">
				<div class="flex justify-between">
					<h2 class="card-title justify-between">
						<Scroll />
						{editingSurvey.title}
					</h2>
					<button
						class="btn btn-ghost btn-neutral btn-sm"
						onclick={() => (editingSurvey = null)}
						aria-label="Close">
						<X size={16} />
					</button>
				</div>

				<div class="p-4">
					Here will be quick settings that will be saved immediatly

					<fieldset
						class="fieldset w-full rounded-box border border-base-300 bg-base-100 p-4">
						<legend class="fieldset-legend">Some Quick Options</legend>
						<label class="label">
							<input type="checkbox" checked class="toggle" />
							Some
						</label>

						<label class="label">
							<input type="checkbox" class="toggle" />
							Other
						</label>
					</fieldset>
				</div>

				<div class="card-actions flex-col">
					<button
						class="btn btn-block btn-outline btn-secondary btn-sm"
						onclick={() => goto(`/app/surveys/${editingSurvey!.surveyId}`)}
						><Cog size={20} /> Full Settings</button>
					<StartSessionButton survey={editingSurvey} />
				</div>
			</div>
		</div>
	{/if}
</div>

<NewSurveyDialog bind:ref={newSurveyDialogRef} />