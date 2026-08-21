<script lang="ts">
	import {
		type ProblemDetails,
		type GetSurveyResponseDto,
		type PaginatedFolderListDto,
		type PaginatedSurveyListDto,
	} from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import {
		ChevronRight,
		EllipsisVertical,
		File,
		Folder,
		Heart,
		Pen,
		Scroll,
		Trash,
	} from '@lucide/svelte';
	import Pagination from './Pagination.svelte';
	import { addToast } from './Toast/Toast.svelte';
	import { untrack } from 'svelte';
	import RenameDialog from './RenameDialog.svelte';
	import axios from 'axios';
	import DiscardDialog from './DiscardDialog.svelte';

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

	let renameDialogRef: HTMLDialogElement | undefined = $state();
	let editingFolderId: string | null = $state(null);
	let editingFolderName: string | null = $state(null);

	let discardDialogRef: HTMLDialogElement | undefined = $state();

	// Update surveys and folders when currentPage changes
	let isInitalLoad = true;
	$effect(() => {
		currentPage;

		untrack(() => {
			if (isInitalLoad) {
				isInitalLoad = false;

				const folderCount = folders.folderListInfo?.length ?? 0;
				const surveyCountToShow = Math.max(0, pageSize - folderCount);

				surveys.surveyListInfo = surveys.surveyListInfo?.slice(0, surveyCountToShow);
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
						skip:
							currentPage === firstSurveyPage || folders.folderCount === 0
								? 0
								: numSurveysOnFirstPage,
					})
					.then((res) => {
						if (res.status === 200) {
							surveys = res.data;
						}
					})
					.catch((e) => {
						addToast({
							label: 'Error loading folder: ' + e,
							type: 'error',
							icon: Folder,
						});
					});
			} else {
				surveys.surveyListInfo = [];
			}
		});
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

	function renameFolder(newName: string, oldName: string, folderId: string) {
		let renamingFolder = folderList.find((e) => e.folderId === folderId);
		if (renamingFolder) renamingFolder.name = newName;
		folderList.sort((a, b) => a.name?.localeCompare(b.name ?? '') ?? 0);

		apiClient.api
			.v1SurveyFoldersPartialUpdate(folderId, { name: newName })
			.then((res) => {
				if (res.status === 200) {
					if (renamingFolder) renamingFolder.name = res.data;
					console.log(res.data);

					addToast({
						label: 'Folder renamed successfully',
						type: 'success',
						icon: Folder,
					});
				}
			})
			.catch((e) => {
				if (renamingFolder) renamingFolder.name = oldName;

				if (axios.isAxiosError<ProblemDetails>(e)) {
					let message = e.response?.data.detail ?? 'Unkown Error';

					addToast({
						label: 'Error renaming folder: ' + message,
						type: 'error',
						icon: Folder,
					});
				}
			});
	}

	function deleteFolder(folderId: string) {
		let folderBackup = folderList.find((e) => e.folderId === folderId);
		if (!folderBackup) return;

		folderList = folderList.filter((e) => e.folderId !== folderId);

		apiClient.api
			.v1SurveyFoldersDelete(folderId)
			.then((res) => {
				if (res.status === 200) {
					addToast({
						label: 'Folder deleted successfully',
						icon: Folder,
						type: 'success',
					});
				}
			})
			.catch((e) => {
				folderList = [...folderList, folderBackup];
				folderList.sort((a, b) => a.name?.localeCompare(b.name ?? '') ?? 0);

				if (axios.isAxiosError<ProblemDetails>(e)) {
					let message = e.response?.data.detail ?? 'Unkown error';

					addToast({
						label: 'Error deleting folder: ' + message,
						type: 'error',
						icon: File,
					});
				}
			});
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
						<div class="flex items-center gap-1">
							<button
								class="btn btn-xs"
								aria-label="Options"
								popovertarget={`folder-popover-${index}`}
								style={`anchor-name:--folder-popover-${index}`}
								onclick={() => {
									editingFolderId = f.folderId;
									editingFolderName = f.name;
								}}>
								<EllipsisVertical size={10} />
							</button>

							<ul
								class="menu dropdown dropdown-end w-52 rounded-box bg-base-100 shadow-sm before:hidden"
								popover
								id={`folder-popover-${index}`}
								style={`position-anchor:--folder-popover-${index}`}>
								<li>
									<button
										class="btn justify-start btn-sm"
										aria-label="Options"
										onclick={() => {
											renameDialogRef?.showModal();
										}}>
										<Pen size={16} class="text-warning" />
										Rename
									</button>
								</li>
								<li class="mt-1">
									<button
										class="btn justify-start btn-sm"
										aria-label="Options"
										onclick={() => {
											discardDialogRef?.showModal();
										}}>
										<Trash size={16} class="text-error" />
										Delete
									</button>
								</li>
							</ul>

							<span class="ml-auto badge hidden badge-sm md:block">
								Items: {f.surveys?.length ?? 0}
							</span>
						</div>
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

<RenameDialog
	bind:ref={renameDialogRef}
	initialName={editingFolderName ?? ''}
	onRename={(newName) => {
		renameFolder(newName, editingFolderName ?? '', editingFolderId ?? '');
		renameDialogRef?.close();
	}} />

<DiscardDialog
	bind:ref={discardDialogRef}
	title={`Are you sure you want to delete the folder "${editingFolderName}"`}
	description="The folder, including every survey inside it, will be *permanently* deleted"
	onDeleteConfirm={() => {
		deleteFolder(editingFolderId ?? '');
		discardDialogRef?.close();
	}} />
