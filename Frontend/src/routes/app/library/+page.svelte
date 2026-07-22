<script lang="ts">
	import StartSessionButton from '$lib/components/startSessionButton.svelte';
	import { goto, invalidateAll, replaceState } from '$app/navigation';
	import { scrollIntoViewOnMount } from '$lib/actions/scrollaction.js';
	import type { GetSurveyResponseDto } from '$lib/api';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import {
		Archive,
		BadgePlus,
		ChartNoAxesCombined,
		Cog,
		Form,
		Heart,
		Plus,
		Scroll,
		Trash,
		SquareKanban,
		X,
	} from '@lucide/svelte';
	import SessionList from '$lib/components/SessionList.svelte';
	import SurveyList from '$lib/components/SurveyList.svelte';
	import type { View } from './+page.js';
	import { page } from '$app/state';
	import NewFolderDialog from '$lib/components/NewFolderDialog.svelte';
	import { apiClient } from '$lib/apiClient.js';

	let { data } = $props();

	let menuTabs = [
		{ label: 'Surveys', icon: Form },
		{ label: 'Sessions', icon: ChartNoAxesCombined },
	];

	let activeViewId = $state<View>((page.url.searchParams.get('view') ?? 'surveys') as View);

	let editingSurvey = $state<GetSurveyResponseDto | null>(null);
	let newSurveyDialogRef: HTMLDialogElement | undefined = $state();
	let newFolderRef: HTMLDialogElement | undefined = $state();

	function switchView(viewId: View) {
		const url = new URL(page.url);
		url.searchParams.set('view', viewId);
		history.replaceState(history.state, '', url);
		activeViewId = viewId;
	}

	async function deleteSurvey(surveyId: string) {
		await apiClient.api.v1SurveyDelete(surveyId);
		if (editingSurvey?.surveyId === surveyId) {
			editingSurvey = null;
		}
		await invalidateAll();
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
				class="btn mt-2 truncate text-nowrap btn-secondary btn-outline"
				onclick={() => newFolderRef?.showModal()}>
				New Folder
			</button>
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
					
					{#if editingSurvey.description}
						<p>Description: {editingSurvey.description}</p>
					{/if}
					<fieldset
						class="fieldset w-full rounded-box border border-base-300 bg-base-100 p-4">
						<legend class="fieldset-legend">Quick Options</legend>
						<label class="label">
							<button class="btn btn-ghost btn-sm ">
								<Heart size={20} class="mr-2 text-primary" /> Favorite
							</button>
							
						</label>
						<label class="label">
							<button class="btn btn-ghost btn-sm " onclick={() => goto(`/app/surveys/${editingSurvey!.surveyId}`)}>
								<Cog size={20} class="mr-2 text-gray-700" /> Settings
							</button>
						</label>

						<label class="label">
							<button class="btn btn-ghost btn-sm " onclick={() => deleteSurvey(editingSurvey!.surveyId)}>
								<Trash size={20} class="mr-2 text-red-700" /> Delete
							</button>
						</label>
					</fieldset>
				</div>

				<div class="card-actions flex-col">
					<StartSessionButton survey={editingSurvey} />
				</div>
			</div>
		</div>
	{/if}
</div>

<NewSurveyDialog bind:ref={newSurveyDialogRef} 
onClose={async () => {
		await invalidateAll();
	}} />
<NewFolderDialog bind:ref={newFolderRef} 
	onClose={async () => {
		await invalidateAll();
	}}/>