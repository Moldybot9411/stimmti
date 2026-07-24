<script lang="ts">
	import type { GetFolderResponseDto, GetSurveyResponseDto } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { ChevronRight, Folder, Heart, Scroll } from '@lucide/svelte';

	type Props = {
		surveys: GetSurveyResponseDto[];
		folder: GetFolderResponseDto[];
		editingSurvey?: GetSurveyResponseDto | null;
	};

	let {
		surveys: initialSurveys,
		folder: initialFolder,
		editingSurvey = $bindable(null),
	}: Props = $props();

	let surveys = $state<GetSurveyResponseDto[]>([]);
	let folder = $state<GetFolderResponseDto[]>([]);
	let draggedSurveyId = $state<string | null>(null);
	let hoveredFolderId = $state<string | null>(null);
	let hoveredRootArea = $state(false);

	$effect(() => {
		surveys = initialSurveys.map((survey) => ({ ...survey }));
		folder = initialFolder.map((item) => ({
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
			surveys.find((s) => s.surveyId === surveyId) ??
			folder.flatMap((item) => item.surveys ?? []).find((s) => s.surveyId === surveyId);
		if (!existingSurvey || existingSurvey.folderId == null) return;

		const previousState = snapshotLibraryState();
		const removedSurvey = takeSurveyFromLibrary(surveyId);
		if (!removedSurvey) return;

		surveys = [...surveys, { ...removedSurvey, folderId: null }].sort((a, b) =>
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
			surveys = previousState.surveys;
			folder = previousState.folder;
		}
	}

	async function handleFolderDrop(event: DragEvent, folderId: string) {
		event.preventDefault();
		const surveyId = draggedSurveyId ?? event.dataTransfer?.getData('application/x-survey-id');
		hoveredFolderId = null;
		draggedSurveyId = null;

		if (!surveyId) return;

		const existingSurvey =
			surveys.find((survey) => survey.surveyId === surveyId) ??
			folder
				.flatMap((item) => item.surveys ?? [])
				.find((survey) => survey.surveyId === surveyId);
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
</script>

<ul class="menu w-full gap-2 rounded-box">
	{#each folder as f, index (f.folderId)}
		<li>
			<details>
				<summary
					class={[
						['text-accent', 'text-info', 'text-success', 'text-warning', 'text-error'][index % 5],
						hoveredFolderId === f.folderId && 'ring-2 ring-base-content rounded-box',
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
									editingSurvey?.surveyId === survey.surveyId && 'bg-base-300',
									draggedSurveyId === survey.surveyId && 'opacity-100',
								]}>
								{#if survey.isFavorite}
									<Heart size={16} class="mr-2 text-primary" fill="currentColor" strokeWidth="2" />
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
	<li
		class={[
			'rounded-box transition-all',
			hoveredRootArea && draggedSurveyId != null && 'ring-2 ring-base-content',
		]}
		ondragover={handleRootDragOver}
		ondragleave={handleRootDragLeave}
		ondrop={handleRootDrop}>
		<ul class="ml-0">
			{#each surveys as survey (survey.surveyId)}
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
									<Heart size={16} class="mr-2 text-primary" fill="currentColor" strokeWidth="2" />
								{:else}
									<Scroll size={16} />
								{/if}
						{survey.title}
						<ChevronRight size={16} />
					</button>
				</li>
			{/each}
		</ul>
	</li>
</ul>
