<script lang="ts">
	import type {
		GetSurveyResponseDto,
		PaginatedFolderListDto,
		PaginatedSurveyListDto,
	} from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { ChevronRight, Folder, Heart, Scroll } from '@lucide/svelte';
	import Pagination from './Pagination.svelte';
	import { addToast } from './Toast/Toast.svelte';

	type Props = {
		surveys: PaginatedSurveyListDto;
		folders: PaginatedFolderListDto;
		editingSurvey?: GetSurveyResponseDto | null;
	};

	let {
		surveys = $bindable(),
		folders = $bindable(),
		editingSurvey = $bindable(null),
	}: Props = $props();

	let surveyList = $derived(surveys.surveyListInfo ?? []);
	let folderList = $derived(folders.folderListInfo ?? []);

	const pageSize = 15;
	let currentPage = $state(1);

	// Hacky solution: partition pagination into folders and surveys in such a way that only the number of items specifier in pageSize is displayed
	let numFolderPages = $derived(Math.ceil(folders.folderCount / pageSize));

	let currentSurveyPage = $derived(currentPage - numFolderPages);

	let firstSurveyPage = $derived(numFolderPages);
	let numSurveysOnFirstPage = $derived(pageSize - (folders.folderCount % pageSize));

	// Update surveys and folders when currentPage changes
	let isInitalLoad = true;
	$effect(() => {
		currentPage;

		if (isInitalLoad) {
			isInitalLoad = false;

			surveys.surveyListInfo = surveys.surveyListInfo?.slice(
				0,
				folders.folderListInfo?.length ?? pageSize
			);
			return;
		}

		apiClient.api
			.v1SurveyFoldersList({ pageSize, currentPage })
			.then((res) => {
				if (res.status === 200) {
					folders = res.data;
				}
			})
			.catch((e) => {
				addToast({ label: 'Error loading folder: ' + e, type: 'error', icon: Folder });
			});

		if (currentPage >= firstSurveyPage) {
			apiClient.api
				.v1SurveyList({
					pageSize: currentSurveyPage === 0 ? numSurveysOnFirstPage : pageSize,
					currentPage: currentSurveyPage === 0 ? 1 : currentSurveyPage,
					skip: currentPage === firstSurveyPage ? 0 : numSurveysOnFirstPage,
				})
				.then((res) => {
					if (res.status === 200) {
						surveys = res.data;
					}
				})
				.catch((e) => {
					addToast({ label: 'Error loading folder: ' + e, type: 'error', icon: Folder });
				});
		} else {
			surveys.surveyListInfo = [];
		}
	});

	let draggedSurveyId = $state<string | null>(null);
	let hoveredFolderId = $state<string | null>(null);
	let hoveredRootArea = $state(false);

	function snapshotLibraryState() {
		return {
			surveys: surveyList.map((survey) => ({ ...survey })),
			folder: folderList.map((item) => ({
				...item,
				surveys: (item.surveys ?? []).map((survey) => ({ ...survey })),
			})),
		};
	}

	function takeSurveyFromLibrary(surveyId: string) {
		const rootIndex = surveyList.findIndex((survey) => survey.surveyId === surveyId);
		if (rootIndex !== -1) {
			const [survey] = surveyList.splice(rootIndex, 1);
			return survey;
		}

		for (const item of folders.folderListInfo ?? []) {
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
		const targetFolder = folderList.find((item) => item.folderId === folderId);
		if (!targetFolder) return false;

		targetFolder.surveys = [...(targetFolder.surveys ?? []), { ...survey, folderId }].sort(
			(left, right) => (left.title ?? '').localeCompare(right.title ?? '')
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
		hoveredRootArea = false;
	}

	function handleFolderDragOver(event: DragEvent, folderId: string) {
		event.preventDefault();
		hoveredFolderId = folderId;
		if (event.dataTransfer) event.dataTransfer.dropEffect = 'move';
	}

	function handleFolderDragLeave(folderId: string) {
		if (hoveredFolderId === folderId) hoveredFolderId = null;
	}

	function handleRootDragOver(event: DragEvent) {
		event.preventDefault();
		hoveredRootArea = true;
		if (event.dataTransfer) event.dataTransfer.dropEffect = 'move';
	}

	function handleRootDragLeave() {
		hoveredRootArea = false;
	}

	async function handleRootDrop(event: DragEvent) {
		event.preventDefault();
		const surveyId = draggedSurveyId ?? event.dataTransfer?.getData('application/x-survey-id');
		hoveredRootArea = false;
		draggedSurveyId = null;

		if (!surveyId) return;

		const existingSurvey =
			surveyList.find((s) => s.surveyId === surveyId) ??
			folderList.flatMap((item) => item.surveys ?? []).find((s) => s.surveyId === surveyId);
		if (!existingSurvey || existingSurvey.folderId == null) return;

		const previousState = snapshotLibraryState();
		const removedSurvey = takeSurveyFromLibrary(surveyId);
		if (!removedSurvey) return;

		surveyList = [...surveyList, { ...removedSurvey, folderId: null }].sort((a, b) =>
			(a.title ?? '').localeCompare(b.title ?? '')
		);

		try {
			await apiClient.api.v1SurveyPartialUpdate(surveyId, {
				title: removedSurvey.title,
				description: removedSurvey.description ?? null,
				removeFromFolder: true,
			});
		} catch (error) {
			console.error('Error removing survey from folder:', error);
			surveyList = previousState.surveys;
			folderList = previousState.folder;
		}
	}

	async function handleFolderDrop(event: DragEvent, folderId: string) {
		event.preventDefault();
		const surveyId = draggedSurveyId ?? event.dataTransfer?.getData('application/x-survey-id');
		hoveredFolderId = null;
		draggedSurveyId = null;

		if (!surveyId) return;

		const existingSurvey =
			surveyList.find((survey) => survey.surveyId === surveyId) ??
			folderList
				.flatMap((item) => item.surveys ?? [])
				.find((survey) => survey.surveyId === surveyId);
		if (!existingSurvey || existingSurvey.folderId === folderId) return;

		const previousState = snapshotLibraryState();
		const removedSurvey = takeSurveyFromLibrary(surveyId);
		if (!removedSurvey) return;

		const inserted = insertSurveyIntoFolder(folderId, removedSurvey);
		if (!inserted) {
			surveyList = previousState.surveys;
			folderList = previousState.folder;
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
			surveyList = previousState.surveys;
			folderList = previousState.folder;
		}
	}
</script>

{#if surveys.surveyCount === 0 && folders.folderCount === 0}
	<span class="font-bold text-base-content/60">No surveys yet</span>
{:else}
	<ul class="menu w-full rounded-box bg-base-200">
		{#each folderList as f, index (f.folderId)}
			<li>
				<details>
					<summary
						class={[
							[
								'text-accent',
								'text-info',
								'text-success',
								'text-warning',
								'text-error',
							][index % 5],
							hoveredFolderId === f.folderId &&
								'rounded-box ring-2 ring-base-content',
						]}
						ondragover={(event) => handleFolderDragOver(event, f.folderId)}
						ondragleave={() => handleFolderDragLeave(f.folderId)}
						ondrop={(event) => handleFolderDrop(event, f.folderId)}>
						<Folder size={16} />
						{f.name}
						<span class="ml-auto badge badge-sm">Items: {f.surveys?.length ?? 0}</span>
					</summary>
					<ul>
						{#each f.surveys ?? [] as survey (survey.surveyId)}
							<li>
								<button
									draggable="true"
									ondragstart={(event) => handleDragStart(event, survey.surveyId)}
									ondragend={handleDragEnd}
									onclick={() => (editingSurvey = survey)}
									class={[
										editingSurvey?.surveyId === survey.surveyId &&
											'bg-base-300',
										draggedSurveyId === survey.surveyId && 'opacity-100',
									]}>
									{#if survey.isFavorite}
										<Heart
											size={16}
											class="text-primary"
											fill="currentColor"
											strokeWidth="2" />
									{:else}
										<Scroll size={16} />
									{/if}
									{survey.title}
									<ChevronRight size={16} />
								</button>
							</li>
						{/each}
					</ul>
				</details>
			</li>
		{/each}
		<ul
			class={[
				'rounded-box transition-all',
				hoveredRootArea && draggedSurveyId != null && 'ring-2 ring-base-content',
			]}
			ondragover={handleRootDragOver}
			ondragleave={handleRootDragLeave}
			ondrop={handleRootDrop}>
			{#each surveyList as survey (survey.surveyId)}
				<li>
					<button
						draggable="true"
						ondragstart={(event) => handleDragStart(event, survey.surveyId)}
						ondragend={handleDragEnd}
						onclick={() => (editingSurvey = survey)}
						class={[
							editingSurvey?.surveyId === survey.surveyId && 'bg-base-300',
							draggedSurveyId === survey.surveyId && 'opacity-60',
						]}>
						{#if survey.isFavorite}
							<Heart
								size={16}
								class="text-primary"
								fill="currentColor"
								strokeWidth="2" />
						{:else}
							<Scroll size={16} />
						{/if}
						{survey.title}
						<ChevronRight size={16} />
					</button>
				</li>
			{/each}
		</ul>
	</ul>

	<ul>
		<div class="divider"></div>
		<Pagination
			numPages={Math.ceil((surveys.surveyCount + folders.folderCount) / pageSize)}
			bind:currentPage />
	</ul>
{/if}
