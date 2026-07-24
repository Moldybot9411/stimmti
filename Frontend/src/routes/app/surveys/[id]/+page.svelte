<script lang="ts">
	import { QuestionTypeEnum, type GetQuestionTemplateResponseDto } from '$lib/api.js';
	import { apiClient } from '$lib/apiClient.js';
	import { Trash, Cog, GripVertical, Plus, HeartCrack } from '@lucide/svelte';
	import type { PageData } from './$types.js';
	import NewQuestionDialog from '$lib/components/NewQuestionDialog.svelte';
	import { page } from '$app/state';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { invalidateAll } from '$app/navigation';
	import PatchQuestion from '$lib/components/PatchQuestion.svelte';

	const id = $derived(page.params.id);
	$effect(() => {
		if (!id) {
			throw new Error('Survey ID fehlt');
		}
	});
	let { data }: { data: PageData } = $props();
	let newQuestionDialogRef: HTMLDialogElement | undefined = $state();
	let patchQuestionDialogRef: HTMLDialogElement | undefined = $state();
	let editingQuestionId: string | null = $state(null);
	let questions = $state<GetQuestionTemplateResponseDto[]>([]);
	$effect(() => {
		questions = [...(data.questions as GetQuestionTemplateResponseDto[])];
	});
	let draggedIndex = $state<number | null>(null);
	let dragOverIndex = $state<number | null>(null);

	function deleteQuestion(questionId: string) {
		questions = questions.filter((q) => q.id !== questionId);
		apiClient.api.v1QuestionsDelete(questionId).catch((e) => {
			console.error('Failed to delete question', e);
		});
	}

	// Drag and drop handlers for reordering questions
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
		} finally {
			invalidateAll();
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
{#if questions.length==0}
	<div class="text-center text-lg opacity-80">Add your first question</div>
	<div class="timeline-start mb-10 md:text-end">
			<button
				class="btn btn-outline btn-primary btn-sm"
				onclick={() => newQuestionDialogRef?.showModal()}>
				<Plus class="mr-2 h-4 w-4" />
				Add Question
			</button>
		</div>
	
{:else}

	
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
				dragOverIndex === index &&
					draggedIndex !== index &&
					'rounded-box ring-2 ring-primary',
			]}>
			<div class="timeline-left md:text-end">
				<svg
					xmlns="http://www.w3.org/2000/svg"
					viewBox="0 0 20 20"
					fill="currentColor"
					class="mb-6 h-10 w-10">
					<path
						fill-rule="evenodd"
						d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z"
						clip-rule="evenodd" />
				</svg>
			</div>
			<check> </check>

			<div class="timeline-start mb-10 md:text-end">
				<div>Question {index + 1}</div>
				<div
					class="grid w-fit min-w-100 grid-cols-[1fr_auto_auto] items-start gap-4 timeline-box rounded-lg border border-base-300 bg-base-100 p-4 shadow-sm">
					<!-- Left: Content -->
					<div class="flex flex-col gap-1">
						<div class="content-start text-left text-lg font-bold">
							{question.name} - {question.questionType}
						</div>
						<div class="overflow-wrap text-left text-sm break-words opacity-80">
							Description: {question.description}
						</div>

						{#if question.questionType === QuestionTypeEnum.NumberScale}
							<div class="text-left">
								Scale: {question.minValue ?? 1} - {question.maxValue ?? 10}
							</div>
						{:else if question.questionType === QuestionTypeEnum.WordCloud}
							<div class="text-left">Answers Amount: {question.maxWords ?? 0}</div>
						{:else if question.answers?.length}
							<div class="direction-row grid grid-cols-2 gap-2">
								{#each question.answers as answer}
									<div class="flex justify-center rounded bg-accent p-2">
										{answer}
									</div>
								{/each}
							</div>
						{/if}
					</div>
					<!-- Middle: Buttons (top-right) -->
					<div class="flex gap-2">
						<Trash onclick={() => deleteQuestion(question.id!)} color="red" size={20} />
						<Cog onclick={() => { editingQuestionId = question.id!; patchQuestionDialogRef?.showModal(); }} color="gray" size={20} />
					</div>
					<!-- Right: Grip Icon (middle-right) -->
					<div class="align-center flex items-center justify-center opacity-40">
						<GripVertical class="cursor-grab text-base-content/40" />
					</div>
				</div>
			</div>

			<hr />
		</li>
	{/each}

	<li>
		<div class="timeline-left mb-10 md:text-end">
			<svg
				xmlns="http://www.w3.org/2000/svg"
				viewBox="0 0 20 20"
				fill="currentColor"
				class="h-10 w-10">
				<path
					fill-rule="evenodd"
					d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.857-9.809a.75.75 0 00-1.214-.882l-3.483 4.79-1.88-1.88a.75.75 0 10-1.06 1.061l2.5 2.5a.75.75 0 001.137-.089l4-5.5z"
					clip-rule="evenodd" />
			</svg>
		</div>
		<div class="timeline-start mb-10 md:text-end">
			<button
				class="btn btn-outline btn-primary btn-sm"
				onclick={() => newQuestionDialogRef?.showModal()}>
				<Plus class="mr-2 h-4 w-4" />
				Add Question
			</button>
		</div>
	</li>
</ul>
{/if}
<NewQuestionDialog
	surveyId={id!}
	bind:ref={newQuestionDialogRef}
	onClose={async () => {
		await invalidateAll();
	}} />

<PatchQuestion
	surveyId={id!}
	questionId={editingQuestionId}
	bind:ref={patchQuestionDialogRef}
	onClose={async () => {
		editingQuestionId = null;
		await invalidateAll();
	}} />