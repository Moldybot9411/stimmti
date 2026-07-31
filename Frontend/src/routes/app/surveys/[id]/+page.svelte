<script lang="ts">
	import { QuestionTypeEnum, type GetQuestionTemplateResponseDto } from '$lib/api.js';
	import { apiClient } from '$lib/apiClient.js';
	import { Trash, Cog, GripVertical, Plus } from '@lucide/svelte';
	import NewQuestionDialog from '$lib/components/NewQuestionDialog.svelte';
	import PatchQuestion from '$lib/components/PatchQuestion.svelte';
	import { dndzone, type DndEvent } from 'svelte-dnd-action';

	let { data } = $props();
	const id = $derived(data.surveyId);

	let newQuestionDialogRef: HTMLDialogElement | undefined = $state();
	let patchQuestionDialogRef: HTMLDialogElement | undefined = $state();
	let editingQuestionId: string | null = $state(null);

	let questions: GetQuestionTemplateResponseDto[] = $state([]);
	$effect(() => {
		questions = data.questions;
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
	function handleDndConsider(e: CustomEvent<DndEvent<GetQuestionTemplateResponseDto>>) {
		questions = e.detail.items;
	}

	async function handleDndFinalize(e: CustomEvent<DndEvent<GetQuestionTemplateResponseDto>>) {
		const previousQuestions = [...questions];

		questions = e.detail.items;

		const movedItemId = e.detail.info.id;
		const newIndex = questions.findIndex((q) => q.id === movedItemId);

		if (newIndex !== -1) {
			try {
				await apiClient.api.v1QuestionsOrderPartialUpdate(movedItemId, {
					orderNumber: newIndex + 1,
				});
			} catch (error) {
				console.error('Failed to update question order', error);
				questions = previousQuestions;
			}
		}
	}
</script>

<div class="mb-8 flex w-full flex-col items-center gap-2">
	<h1 class="text-center text-4xl font-bold">Survey Questions</h1>
	<p class="text-center opacity-80">{questions.length} question(s)</p>
</div>

{#if questions.length == 0}
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
	<div class="mx-auto flex w-fit flex-col items-start pl-4">
		<ul
			class="timeline timeline-vertical timeline-compact timeline-snap-icon"
			use:dndzone={{
				items: questions,
				flipDurationMs: 300,
				delayTouchStart: 300,
				dropTargetStyle: {},
			}}
			onconsider={handleDndConsider}
			onfinalize={handleDndFinalize}>
			{#each questions as question, index (question.id)}
				<li class="w-fit transition-opacity outline-none">
					{#if index > 0}
						<hr />
					{/if}

					<div class="timeline-middle min-h-8 rounded-full border-4 border-base-300 p-1">
					</div>

					<div class="timeline-end mb-4">
						<div class="flex min-h-10 items-center font-bold">Question {index + 1}</div>
						<div
							class="grid w-fit grid-cols-[1fr_auto_auto] items-start gap-4 timeline-box rounded-lg border border-base-300 bg-base-100 p-4 shadow-sm">
							<div class="align-center flex items-center justify-center opacity-40">
								<GripVertical class="cursor-grab text-base-content/40" />
							</div>
							<div class="flex flex-col gap-1 text-balance wrap-anywhere">
								<div class="content-start text-left text-lg font-bold">
									{question.name} - {question.questionType}
								</div>

								{#if question.description}
									<div class="mb-2 text-left text-sm opacity-80">
										{question.description}
									</div>
								{/if}

								{#if question.questionType === QuestionTypeEnum.NumberScale}
									<div class="text-left">
										Scale: {question.minValue ?? 1} - {question.maxValue ?? 10}
									</div>
								{:else if question.questionType === QuestionTypeEnum.WordCloud}
									<div class="text-left">
										Answers Amount: {question.maxWords ?? 0}
									</div>
								{:else if question.answers?.length}
									<div
										class="direction-row grid grid-cols-1 gap-2 text-accent-content md:grid-cols-2">
										{#if question.answers.length > 8}
											<div class="col-span-2 text-left text-sm opacity-80">
												Note: Only the first 8 answers are displayed
											</div>
										{/if}
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
								<button
									class="btn btn-ghost btn-error btn-xs"
									onclick={() => deleteQuestion(question.id!)}
									aria-label="Delete Question">
									<Trash size={20} />
								</button>

								<button
									class="btn btn-ghost btn-secondary btn-xs"
									onclick={() => {
										editingQuestionId = question.id!;
										patchQuestionDialogRef?.showModal();
									}}
									aria-label="Question Settings">
									<Cog size={20} />
								</button>
							</div>
						</div>
					</div>

					<hr />
				</li>
			{/each}
		</ul>

		<ul class="timeline timeline-vertical timeline-compact">
			<li>
				<hr />

				<div class="timeline-middle min-h-8 rounded-full border-4 border-base-300 p-1">
				</div>

				<div class="timeline-end mt-2">
					<button
						class="btn btn-outline btn-primary btn-sm"
						onclick={() => newQuestionDialogRef?.showModal()}>
						<Plus class="shrink-0" size={20} />
						Add Question
					</button>
				</div>
			</li>
		</ul>
	</div>
{/if}

<NewQuestionDialog
	surveyId={id!}
	bind:ref={newQuestionDialogRef}
	onCreated={(question) => {
		questions.push(question);
	}} />

<PatchQuestion
	surveyId={id}
	questionId={editingQuestionId}
	bind:ref={patchQuestionDialogRef}
	onSave={(res) => {
		questions = questions.map((x) => (x.id === editingQuestionId ? res : x));

		editingQuestionId = null;
	}}
	onCancel={() => {
		editingQuestionId = null;
	}} />
