<script lang="ts">
    import StartSessionButton from '$lib/components/startSessionButton.svelte';
	import { goto } from '$app/navigation';
	import { scrollIntoViewOnMount } from '$lib/actions/scrollaction.js';
	import type { GetFolderResponseDto, GetSurveyResponseDto } from '$lib/api';
	import { apiClient } from '$lib/apiClient.js';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import Pagination from '$lib/components/Pagination.svelte';
	import {
		Archive,
		BadgePlus,
		ChartNoAxesCombined,
		ChevronRight,
		Cog,
		Folder,
		Form,
		Play,
		Plus,
		Scroll,
		X,
	} from '@lucide/svelte';

	let { data } = $props();

	let menuTabs = [
		{ label: 'surveys', icon: Form },
		{ label: 'Sessions', icon: ChartNoAxesCombined },
		{ label: 'Templates', icon: Plus },
		{ label: 'Archive', icon: Archive },
	];

	let activeViewId = $derived(data.currentView);

	let surveys = $state<GetSurveyResponseDto[]>([]);
	let folder = $state<GetFolderResponseDto[]>([]);
	let editingItem = $state<string | null>(null);
	let draggedSurveyId = $state<string | null>(null);
	let hoveredFolderId = $state<string | null>(null);
	let editingSurvey = $derived(
		surveys.find((s) => s.surveyId === editingItem) ??
			folder.flatMap((f) => f.surveys ?? []).find((s) => s.surveyId === editingItem) ??
			null
	);
	let newSurveyDialogRef: HTMLDialogElement | undefined = $state();

	$effect(() => {
		surveys = data.surveys.map((survey) => ({ ...survey }));
		folder = data.folder.map((item) => ({
			...item,
			surveys: (item.surveys ?? []).map((survey) => ({ ...survey })),
		}));
	});

	function snapshotLibraryState() {
		return {
			surveys: surveys.map((survey) => ({ ...survey })),
			folder: folder.map((item) => ({
				...item,
				surveys: (item.surveys ?? []).map((survey) => ({ ...survey })),
			})),
		};
	}

	function takeSurveyFromLibrary(surveyId: string) {
		const rootIndex = surveys.findIndex((survey) => survey.surveyId === surveyId);
		if (rootIndex !== -1) {
			const [survey] = surveys.splice(rootIndex, 1);
			return survey;
		}

		for (const item of folder) {
			const nestedSurveys = item.surveys ?? [];
			const nestedIndex = nestedSurveys.findIndex((survey) => survey.surveyId === surveyId);
			if (nestedIndex !== -1) {
				const [survey] = nestedSurveys.splice(nestedIndex, 1);
				item.surveys = nestedSurveys;
				return survey;
			}
		}

		return null;
	}

	function insertSurveyIntoFolder(folderId: string, survey: GetSurveyResponseDto) {
		const targetFolder = folder.find((item) => item.folderId === folderId);
		if (!targetFolder) return false;

		targetFolder.surveys = [...(targetFolder.surveys ?? []), { ...survey, folderId }].sort((left, right) =>
			(left.title ?? '').localeCompare(right.title ?? '')
		);
		return true;
	}

	function handleDragStart(event: DragEvent, surveyId: string) {
		draggedSurveyId = surveyId;
		event.dataTransfer?.setData('text/plain', surveyId);
		event.dataTransfer?.setData('application/x-survey-id', surveyId);
		event.dataTransfer?.setDragImage(event.currentTarget as Element, 16, 16);
		event.dataTransfer!.effectAllowed = 'move';
	}

	function handleDragEnd() {
		draggedSurveyId = null;
		hoveredFolderId = null;
	}

	function handleFolderDragOver(event: DragEvent, folderId: string) {
		event.preventDefault();
		hoveredFolderId = folderId;
		if (event.dataTransfer) event.dataTransfer.dropEffect = 'move';
	}

	function handleFolderDragLeave(folderId: string) {
		if (hoveredFolderId === folderId) hoveredFolderId = null;
	}

	async function handleFolderDrop(event: DragEvent, folderId: string) {
		console.log('handleFolderDrop', { folderId, draggedSurveyId, hoveredFolderId });
		event.preventDefault();
		const surveyId = draggedSurveyId ?? event.dataTransfer?.getData('application/x-survey-id');
		hoveredFolderId = null;
		draggedSurveyId = null;

		if (!surveyId) return;

		const existingSurvey =
			surveys.find((survey) => survey.surveyId === surveyId) ??
			folder.flatMap((item) => item.surveys ?? []).find((survey) => survey.surveyId === surveyId);
		if (!existingSurvey || existingSurvey.folderId === folderId) return;

		const previousState = snapshotLibraryState();
		const removedSurvey = takeSurveyFromLibrary(surveyId);
		if (!removedSurvey) return;

		const inserted = insertSurveyIntoFolder(folderId, removedSurvey);
		if (!inserted) {
			surveys = previousState.surveys;
			folder = previousState.folder;
			return;
		}

		try {
			await apiClient.api.v1SurveyPartialUpdate(surveyId, {
				title: removedSurvey.title,
				description: removedSurvey.description ?? null,
				folderId,
			});
		} catch (error) {
			console.error('Error moving survey to folder:', error);
			surveys = previousState.surveys;
			folder = previousState.folder;
		}
	}

	function getFolderSummaryClass(index: number, folderId: string) {
		return [
			hoveredFolderId === folderId ? 'bg-base-200' : null,
		];
	}
</script>

<div class="flex w-full flex-col gap-4 md:flex-row">
	<ul class="menu h-fit w-full bg-base-100 shadow-sm md:sticky md:top-4 md:flex-1">
		<li>
			<h2 class="menu-title">Library</h2>
			<ul>
				{#each menuTabs as tab}
					<li class={activeViewId === tab.label.toLowerCase() ? 'menu-active' : ''}>
						<a href="?view={tab.label.toLowerCase()}">
							<tab.icon size={16} />
							{tab.label}
						</a>
					</li>
				{/each}
			</ul>
			<button class="btn mt-2 btn-primary" onclick={() => newSurveyDialogRef?.showModal()}>
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
				<ul class="menu w-full rounded-box">
					{#each folder as f, index (f.folderId)}
						<li>
							<details>
								<summary
									class={[
										'text-accent',
										'text-info',
										'text-success',
										'text-warning',
										'text-error',
									][index % 5]}
									ondragover={(event) => handleFolderDragOver(event, f.folderId)}
									ondragleave={() => handleFolderDragLeave(f.folderId)}
									ondrop={(event) => handleFolderDrop(event, f.folderId)}>
									
									<Folder size={16} />
									{f.name}
									<span class="badge badge-sm ml-auto">Items: {f.surveys?.length ?? 0}</span>
								</summary>
								<ul>
									{#each f.surveys ?? [] as survey (survey.surveyId)}
										<li>
											<button
												draggable="true"
												ondragstart={(event) => handleDragStart(event, survey.surveyId)}
												ondragend={handleDragEnd}
												onclick={() => (editingItem = survey.surveyId)}
												class={[
													editingItem === survey.surveyId &&'bg-base-300',
													draggedSurveyId === survey.surveyId && 'opacity-100',
												]}>
												<Scroll size={16} />
												{survey.title}
												<ChevronRight size={16} />
											</button>
										</li>
									{/each}
								</ul>
							</details>
						</li>
					{/each}
					{#each surveys as survey (survey.surveyId)}
						<li>
							<button
								draggable="true"
								ondragstart={(event) => handleDragStart(event, survey.surveyId)}
								ondragend={handleDragEnd}
								onclick={() => (editingItem = survey.surveyId)}
								class={[
									editingItem === survey.surveyId && 'bg-base-300',
									draggedSurveyId === survey.surveyId && 'opacity-60',
								]}>
								<Scroll size={16} />
								{survey.title}
								<ChevronRight size={16} />
							</button>
						</li>
					{/each}
				</ul>


			{:else if activeViewId === 'sessions'}
				<p>Here will be the Sessions</p>
			{:else if activeViewId === 'templates'}
				<p>Here will be the Templates</p>
			{:else if activeViewId === 'archive'}
				<p>Here will be the Archive</p>
			{/if}
		</div>
	</div>

	{#if editingSurvey}
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
						class="btn btn-ghost btn-sm btn-neutral"
						onclick={() => (editingItem = null)}
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
						class="btn btn-block btn-outline btn-sm btn-secondary"
						onclick={() => goto(`/app/surveys/${editingSurvey!.surveyId}`)}
						><Cog size={20} /> Full Settings</button>
					<StartSessionButton survey={editingSurvey}  />
				</div>
			</div>
		</div>
	{/if}
</div>

<NewSurveyDialog bind:ref={newSurveyDialogRef} />