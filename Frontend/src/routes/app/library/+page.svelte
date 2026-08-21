<script lang="ts">
	import StartSessionDialog from '$lib/components/StartSessionDialog.svelte';
	import { goto } from '$app/navigation';
	import { scrollIntoViewOnMount } from '$lib/actions/scrollaction.js';
	import type {
		GetSurveyResponseDto,
		PaginatedFolderListDto,
		PaginatedSessionListDto,
		PaginatedSurveyListDto,
		ProblemDetails,
	} from '$lib/api';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import {
		BadgePlus,
		ChartNoAxesCombined,
		Cog,
		Form,
		Heart,
		Pen,
		Play,
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
	import RenameDialog from '$lib/components/RenameDialog.svelte';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import axios from 'axios';
	import DiscardDialog from '$lib/components/DiscardDialog.svelte';

	let { data } = $props();
	let surveys: PaginatedSurveyListDto = $state({ surveyCount: 0, surveyListInfo: [] });
	let folders: PaginatedFolderListDto = $state({ folderCount: 0, folderListInfo: [] });
	let sessions: PaginatedSessionListDto = $state({ sessionCount: 0, sessionListInfo: [] });

	let isLoading = $state(false);
	let isDeleting = $state(false);

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

	let startSessionDialogRef: HTMLDialogElement | undefined = $state();
	let renameDialogRef: HTMLDialogElement | undefined = $state();
	let discardDialogRef: HTMLDialogElement | undefined = $state();

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

	function renameSurveyLocally(name: string, surveyId: string) {
		if (surveys.surveyListInfo) {
			let renamingSurvey = surveys.surveyListInfo.find((e) => e.surveyId === surveyId);
			if (renamingSurvey) renamingSurvey.title = name;
		}
		if (folders.folderListInfo) {
			for (const folder of folders.folderListInfo) {
				var nestedSurveys = folder.surveys ?? [];
				var renamingSurvey = nestedSurveys.find((e) => e.surveyId === surveyId);
				if (renamingSurvey) renamingSurvey.title = name;
			}
		}
	}

	function deleteSurvey(surveyId: string) {
		isDeleting = true;

		if (editingSurvey?.surveyId === surveyId) {
			editingSurvey = null;
		}

		apiClient.api
			.v1SurveyDelete(surveyId)
			.then((res) => {
				if (res.status === 200) {
					removeSurveyFromLibrary(surveyId);

					addToast({
						label: 'Survey deleted successfully',
						type: 'success',
						icon: Scroll,
					});

					isDeleting = false;
					discardDialogRef?.close();
				}
			})
			.catch((e) => {
				isDeleting = false;
				discardDialogRef?.close();

				if (axios.isAxiosError<ProblemDetails>(e)) {
					var message = e.response?.data.detail ?? 'Unknown Error';

					addToast({
						label: 'Error deleting Survey: ' + message,
						type: 'error',
						icon: Scroll,
					});
				}
			});
	}

	async function favoriteCurrentSurvey() {
		if (!editingSurvey) return;

		const newFavoriteState = !editingSurvey.isFavorite;
		editingSurvey.isFavorite = newFavoriteState;

		await apiClient.api.v1SurveyToggleFavoriteCreate(editingSurvey.surveyId);
	}

	function renameSurvey(newName: string, oldName: string, surveyId: string) {
		renameSurveyLocally(newName, surveyId);

		apiClient.api
			.v1SurveyPartialUpdate(surveyId, { title: newName })
			.then((res) => {
				console.log(res);
				if (res.status === 200) {
					addToast({ label: 'Survey updated', type: 'success', icon: Pen });
				}
			})
			.catch((e) => {
				renameSurveyLocally(oldName, surveyId);

				if (axios.isAxiosError<ProblemDetails>(e)) {
					let message = e.response?.data.detail ?? 'Unknown Error';

					addToast({
						label: 'Error updating Survey: ' + message,
						type: 'error',
						icon: Pen,
					});
				}
			});
	}
</script>

<div class="flex w-full flex-col gap-4 md:flex-row">
	<ul class="menu h-fit w-full rounded-box bg-base-100 shadow-sm md:sticky md:top-4 md:flex-1">
		<li>
			<h2 class="menu-title">Library</h2>
			<ul>
				{#each menuTabs as tab}
					<li
						class={[
							'rounded-box',
							activeViewId === tab.label.toLowerCase() ? 'menu-active' : '',
						]}>
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
			class="card h-fit min-w-0 flex-3 bg-base-100 shadow-sm card-md"
			use:scrollIntoViewOnMount={editingSurvey.surveyId}>
			<div class="card-body">
				<div class="flex justify-between">
					<h2 class="card-title min-w-0 justify-between">
						<Scroll class="shrink-0" />
						<span class="min-w-0 truncate">{editingSurvey.title}</span>
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
						onclick={() => renameDialogRef?.showModal()}>
						<Pen size={20} class="text-warning" stroke-width="2" />
						Rename
					</button>

					<button
						class="btn justify-start btn-sm"
						onclick={() => goto(`/app/surveys/${editingSurvey!.surveyId}`)}>
						<Cog size={20} class="text-base-content" /> Settings
					</button>

					<button
						class="btn justify-start btn-sm"
						onclick={() => discardDialogRef?.showModal()}>
						<Trash size={20} class="text-error" /> Delete
					</button>

					<div class="divider my-0"></div>
				</div>

				<div class="card-actions flex-col">
					<button
						class={'btn btn-block max-w-full btn-primary '}
						onclick={() => startSessionDialogRef?.showModal()}>
						<Play size={20} /> Start Session
					</button>
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

{#if editingSurvey}
	{@const survey = editingSurvey}

	<StartSessionDialog
		bind:ref={startSessionDialogRef}
		{survey}
		onCreated={(roomCode) => goto(`/live/${roomCode}`)} />

	<RenameDialog
		bind:ref={renameDialogRef}
		initialName={editingSurvey.title!}
		onRename={(newName: string) => {
			renameSurvey(newName, survey.title!, survey.surveyId);
			renameDialogRef?.close();
		}} />

	<DiscardDialog
		bind:ref={discardDialogRef}
		title={`Are you sure you want to delete the Survey "${survey.title}"?`}
		description="The survey will be permanently deleted, but existing Sessions of this Survey stay untouched."
		onDeleteConfirm={() => deleteSurvey(survey.surveyId)}
		{isDeleting} />
{/if}
