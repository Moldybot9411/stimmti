<script lang="ts">
	import { QuestionTypeEnum, type GetQuestionTemplateResponseDto } from '$lib/api.js';
	import { apiClient } from '$lib/apiClient.js';
	import { GripVertical, Plus } from '@lucide/svelte';
	import type { PageData } from './$types.js';
	import NewQuestionDialog from '$lib/components/NewQuestionDialog.svelte';
	import { page } from '$app/state';

	const id = $derived(page.params.id);	
	if (!id) {
		throw new Error('Survey ID fehlt');
	}
	let { data }: { data: PageData } = $props();
	let newQuestionDialogRef: HTMLDialogElement | undefined = $state();
	let questions = $state<GetQuestionTemplateResponseDto[]>([...(data.questions as GetQuestionTemplateResponseDto[])]);
	let draggedIndex = $state<number | null>(null);
	let dragOverIndex = $state<number | null>(null);

	function handleDragStart(event: DragEvent, index: number) {
		draggedIndex = index;
		event.dataTransfer!.effectAllowed = 'move';
	}

	function handleDragOver(event: DragEvent, index: number) {
		event.preventDefault();
		event.dataTransfer!.dropEffect = 'move';
		dragOverIndex = index;
	}

	function handleDragLeave() {
		dragOverIndex = null;
	}

	async function handleDrop(event: DragEvent, targetIndex: number) {
		event.preventDefault();
		dragOverIndex = null;

		if (draggedIndex === null || draggedIndex === targetIndex) {
			draggedIndex = null;
			return;
		}

		// Optimistic reorder locally
		const prev = [...questions];
		const [moved] = questions.splice(draggedIndex, 1);
		questions.splice(targetIndex, 0, moved);
		draggedIndex = null;

		try {
			await apiClient.api.v1QuestionsOrderPartialUpdate(moved.id!, {
				orderNumber: targetIndex + 1,
			});
		} catch (e) {
			console.error('Failed to update question order', e);
			questions = prev;
		}
	}

	function handleDragEnd() {
		draggedIndex = null;
		dragOverIndex = null;
	}
</script>

<div class="mb-8 flex w-full flex-col items-center gap-2">
	<h1 class="text-center text-4xl font-bold">Survey Questions</h1>
	<p class="text-center opacity-80">{questions.length} question(s)</p>
</div>

<ul class="timeline timeline-vertical timeline-snap-icon max-md:timeline-compact">
	{#each questions as question, index}
		<li
			draggable="true"
			ondragstart={(e) => handleDragStart(e, index)}
			ondragover={(e) => handleDragOver(e, index)}
			ondragleave={handleDragLeave}
			ondrop={(e) => handleDrop(e, index)}
			ondragend={handleDragEnd}
			class={[
				'transition-opacity',
				draggedIndex === index && 'opacity-30',
				dragOverIndex === index && draggedIndex !== index && 'ring-2 ring-primary rounded-box',
			]}>
			<div class="timeline-middle">
				<svg
					xmlns="http://www.w3.org/2000/svg"
					viewBox="0 0 20 20"
					fill="currentColor"
					class="h-5 w-5">
					<path
						fill-rule="evenodd"
						d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z"
						clip-rule="evenodd" />
				</svg>
			</div>

			<div class="timeline-start mb-10 md:text-end">
				<div>Question {index + 1}</div>
				<div class="timeline-box min-w-150 w-full rounded-lg border border-base-300 bg-base-100 p-4 shadow-sm">
					<div class="mb-1 flex items-center justify-between gap-2">
						<div class="text-lg font-bold">{question.name} - {question.questionType}</div>
						<GripVertical class="cursor-grab text-base-content/40" />
					</div>
					<div class="text-sm opacity-80">{question.description}</div>

					{#if question.questionType === QuestionTypeEnum.NumberScale}
						<div>Scale: {question.minValue ?? 1} - {question.maxValue ?? 10}</div>
					{:else if question.questionType === QuestionTypeEnum.WordCloud}
						<div>Max words: {question.maxWords ?? 0}</div>
					{:else if question.answers?.length}
						<div class="grid grid-cols-2 gap-2 direction-row">
							{#each question.answers as answer}
								<div class="flex min-w-75 justify-center rounded border p-2">{answer}</div>
							{/each}
						</div>
					{/if}
				</div>
			</div>
			<hr />
		</li>
	{/each}

	<li>
		<div class="timeline-middle mb-10 md:text-end">
			<svg
				xmlns="http://www.w3.org/2000/svg"
				viewBox="0 0 20 20"
				fill="currentColor"
				class="h-5 w-5">
				<path
					fill-rule="evenodd"
					d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z"
					clip-rule="evenodd" />
			</svg>
		</div>
		<div class="timeline-start mb-10 md:text-end">
			<button class="btn btn-outline btn-primary btn-sm" onclick={() => newQuestionDialogRef?.showModal()}>
				<Plus class="mr-2 h-4 w-4" />
				Add Question
			</button>
		</div>
	</li>
</ul>
<NewQuestionDialog surveyId={id} bind:ref={newQuestionDialogRef} />
