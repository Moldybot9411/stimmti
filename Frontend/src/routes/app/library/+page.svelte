<script lang="ts">
	import StartSessionButton from '$lib/components/startSessionButton.svelte';
	import { goto } from '$app/navigation';
	import { scrollIntoViewOnMount } from '$lib/actions/scrollaction.js';
	import type {
		GetSurveyResponseDto,
		PaginatedFolderListDto,
		PaginatedSessionListDto,
		PaginatedSurveyListDto,
	} from '$lib/api';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import {
		BadgePlus,
		ChartNoAxesCombined,
		Cog,
		Form,
		Heart,
		Scroll,
		Trash,
		X,
	} from '@lucide/svelte';
	import SessionList from '$lib/components/SessionList.svelte';
	import SurveyList from '$lib/components/SurveyList.svelte';
	import type { View } from './+page.js';
	import { page } from '$app/state';
	import NewFolderDialog from '$lib/components/NewFolderDialog.svelte';
	import { apiClient } from '$lib/apiClient.js';

	let { data } = $props();
	let surveys: PaginatedSurveyListDto = $state({ surveyCount: 0, surveyListInfo: [] });
	let folders: PaginatedFolderListDto = $state({ folderCount: 0, folderListInfo: [] });
	let sessions: PaginatedSessionListDto = $state({ sessionCount: 0, sessionListInfo: [] });

	let isLoading = $state(false);

	$effect(() => {
		isLoading = true;
		Promise.all([data.surveys, data.folder, data.sessions])
			.then((res) => {
				surveys = res[0];
				folders = res[1];
				sessions = res[2];
			})
			.finally(() => (isLoading = false));
	});

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

	function removeSurveyFromLibrary(surveyId: string) {
		if (surveys.surveyListInfo) {
			const rootIndex = surveys.surveyListInfo.findIndex(
				(survey) => survey.surveyId === surveyId
			);
			if (rootIndex !== -1) {
				return surveys.surveyListInfo.splice(rootIndex, 1)[0];
			}
		}

		if (folders.folderListInfo) {
			for (const folder of folders.folderListInfo) {
				const nestedSurveys = folder.surveys ?? [];
				const nestedIndex = nestedSurveys.findIndex(
					(survey) => survey.surveyId === surveyId
				);
				if (nestedIndex !== -1) {
					const [survey] = nestedSurveys.splice(nestedIndex, 1);
					folder.surveys = nestedSurveys;
					return survey;
				}
			}
		}

		return null;
	}

	async function deleteSurvey(surveyId: string) {
		removeSurveyFromLibrary(surveyId);

		await apiClient.api.v1SurveyDelete(surveyId);
		if (editingSurvey?.surveyId === surveyId) {
			editingSurvey = null;
		}
	}

	async function favoriteCurrentSurvey() {
		if (!editingSurvey) return;

		const newFavoriteState = !editingSurvey.isFavorite;
		editingSurvey.isFavorite = newFavoriteState;

		await apiClient.api.v1SurveyToggleFavoriteCreate(editingSurvey.surveyId);
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
				class="btn mt-2 truncate btn-outline text-nowrap btn-secondary"
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

			{#if isLoading}
				<ul class="menu w-full gap-2 rounded-box">
					<li class="skeleton p-2">
						<div class="skeleton-text">Loading...</div>
					</li>
				</ul>
			{:else if activeViewId === 'surveys'}
				<SurveyList bind:surveys bind:folders bind:editingSurvey />
			{:else if activeViewId === 'sessions'}
				<SessionList sessionData={sessions} />
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

				<div class="flex flex-col gap-2 p-4">
					{#if editingSurvey.description}
						<p>Description: {editingSurvey.description}</p>
					{/if}
					{#if editingSurvey.questionAmount !== undefined}
						<div class="badge badge-outline badge-neutral">
							{editingSurvey.questionAmount} Questions
						</div>
					{/if}

					<div class="divider my-0"></div>

					<button class="btn justify-start btn-sm" onclick={favoriteCurrentSurvey}>
						<Heart
							size={20}
							class="text-primary"
							strokeWidth="2"
							fill={editingSurvey.isFavorite ? 'currentColor' : 'transparent'} />
						Favorite
					</button>

					<button
						class="btn justify-start btn-sm"
						onclick={() => goto(`/app/surveys/${editingSurvey!.surveyId}`)}>
						<Cog size={20} class="text-base-content" /> Settings
					</button>

					<button
						class="btn justify-start btn-sm"
						onclick={() => deleteSurvey(editingSurvey!.surveyId)}>
						<Trash size={20} class="text-error" /> Delete
					</button>

					<div class="divider my-0"></div>
				</div>

				<div class="card-actions flex-col">
					<StartSessionButton survey={editingSurvey} />
				</div>
			</div>
		</div>
	{/if}
</div>

<NewSurveyDialog bind:ref={newSurveyDialogRef} />
<NewFolderDialog
	bind:ref={newFolderRef}
	onCreate={(el) => {
		folders.folderListInfo?.push(el);
		folders.folderListInfo?.sort((a, b) => a.name!.localeCompare(b.name!));
	}} />
