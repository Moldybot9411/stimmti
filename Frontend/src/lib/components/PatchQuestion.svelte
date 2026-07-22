<script lang="ts">
	import { HeartCrack, LoaderCircle, X } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';
	import { addToast } from './Toast/Toast.svelte';
	import { apiClient } from '$lib/apiClient';
	import { QuestionTypeEnum, type GetQuestionTemplateResponseDto } from '$lib/api';
	const maxChoiceAnswers = 8;

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		surveyId: string;
		onClose?: () => void | Promise<void>;
		questionId?: string | null;
	};

	let { class: classes, style, ref = $bindable(), surveyId, onClose, questionId }: Props = $props();
	let formRef: HTMLFormElement | null = $state(null);
	let isLoading = $state(false);
	let isLoadingData = $state(false);

	let name = $state('');
	let description = $state('');
	let questionType = $state(QuestionTypeEnum.SingleChoice);
	let isQuestionTypeDropdownOpen = $state(false);
	let minValue = $state(1);
	let maxValue = $state(10);
	let maxWords = $state(3);
	let answerInputs = $state(['', '']);

	// Load question data when questionId changes
	$effect(() => {
		if (!questionId) return;

		isLoadingData = true;
		apiClient.api
			.v1QuestionsDetail(questionId)
			.then((result) => {
				if (result.data) {
					const q = result.data;
					name = q.name || '';
					description = q.description || '';
					questionType = q.questionType || QuestionTypeEnum.SingleChoice;
					minValue = q.minValue || 1;
					maxValue = q.maxValue || 10;
					maxWords = q.maxWords || 3;
					answerInputs = q.answers && q.answers.length > 0 ? [...q.answers, ''] : ['', ''];
				}
			})
			.catch((error: unknown) => {
				console.error('Failed to load question:', error);
				addToast({
					type: 'error',
					label: 'Failed to load question data',
					icon: HeartCrack,
				});
			})
			.finally(() => {
				isLoadingData = false;
			});
	});

	function addAnswerFieldIfNeeded(index: number) {
		if (answerInputs[index]?.trim().length === 0) {
			return;
		}

		const isLastField = index === answerInputs.length - 1;
		if (isLastField && answerInputs.length < maxChoiceAnswers) {
			answerInputs = [...answerInputs, ''];
		}
	}

	function reset() {
		formRef?.reset();
		isQuestionTypeDropdownOpen = false;
		answerInputs = ['', ''];
		name = '';
		description = '';
		questionType = QuestionTypeEnum.SingleChoice;
		minValue = 1;
		maxValue = 10;
		maxWords = 3;
	}

	async function updateQuestion() {
		if (!questionId) return;

		const parsedAnswers = answerInputs
			.map((x) => x.trim())
			.filter((x) => x.length > 0);

		if (
			(questionType === QuestionTypeEnum.SingleChoice ||
				questionType === QuestionTypeEnum.MultipleChoice) &&
			parsedAnswers.length < 2
		) {
			addToast({
				type: 'error',
				label: 'Please provide at least 2 answers for choice questions',
				icon: HeartCrack,
			});
			return;
		}

		if (questionType === QuestionTypeEnum.NumberScale && minValue >= maxValue) {
			addToast({
				type: 'error',
				label: 'Min value must be smaller than max value',
				icon: HeartCrack,
			});
			return;
		}

		if (questionType === QuestionTypeEnum.WordCloud && maxWords <= 0) {
			addToast({
				type: 'error',
				label: 'Max words must be greater than 0',
				icon: HeartCrack,
			});
			return;
		}

		isLoading = true;
		await apiClient.api
			.v1QuestionsPartialUpdate(questionId, {
				name,
				description,
				answers:
					questionType === QuestionTypeEnum.SingleChoice ||
					questionType === QuestionTypeEnum.MultipleChoice
						? parsedAnswers
						: [],
				minValue: questionType === QuestionTypeEnum.NumberScale ? minValue : undefined,
				maxValue: questionType === QuestionTypeEnum.NumberScale ? maxValue : undefined,
				maxWords: questionType === QuestionTypeEnum.WordCloud ? maxWords : undefined,
			} as any)
			.then(() => {
				addToast({
					type: 'success',
					label: 'Question updated successfully',
					icon: HeartCrack,
				});
				ref?.close();
			})
			.catch((error: unknown) => {
				const errorMsg = error instanceof Error ? error.message : 'Unknown error';
				addToast({
					type: 'error',
					label: `Failed to update question: ${errorMsg}`,
					icon: HeartCrack,
				});
			})
			.finally(() => {
				isLoading = false;
			});
	}
</script>

<dialog
	class={['modal', classes]}
	{style}
	bind:this={ref}
	onclose={() => {
		reset();
		onClose?.();
	}}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm" disabled={isLoading || isLoadingData}>
				<X />
			</button>
		</form>
		<h3 class="text-lg font-bold">Edit Question</h3>
		<div class="p-4">
			{#if isLoadingData}
				<div class="flex justify-center p-8">
					<LoaderCircle class="animate-spin" size={32} />
				</div>
			{:else}
				<form
					class=""
					bind:this={formRef}
					onsubmit={async (e) => {
						e.preventDefault();
						await updateQuestion();
					}}
					onreset={() => {
						isLoading = false;
					}}>
					<fieldset class="fieldset">
						<legend class="fieldset-legend">Question Title</legend>
						<input bind:value={name} type="text" class="input w-full" placeholder="My Question" required />
					</fieldset>

					<fieldset class="fieldset">
						<legend class="fieldset-legend">Question Description</legend>
						<input bind:value={description} type="text" class="input w-full" placeholder="My Description" />
					</fieldset>
                    
					{#if questionType === QuestionTypeEnum.SingleChoice || questionType === QuestionTypeEnum.MultipleChoice}
						<fieldset class="fieldset">
							<legend class="fieldset-legend">Answers</legend>
							<div class="flex flex-col gap-2">
								{#each answerInputs as _, index}
									<input
										bind:value={answerInputs[index]}
										onblur={() => addAnswerFieldIfNeeded(index)}
										type="text"
										class="input w-full"
										placeholder={`Answer ${index + 1}`} />
								{/each}
							</div>
							<div class="label">
								<span class="label-text-alt">Neue Felder entstehen beim Raus-Tabben, maximal 8 Antworten.</span>
							</div>
						</fieldset>
					{:else if questionType === QuestionTypeEnum.NumberScale}
						<div class="grid grid-cols-2 gap-2">
							<fieldset class="fieldset">
								<legend class="fieldset-legend">Min Value</legend>
								<input bind:value={minValue} type="number" class="input w-full" placeholder="Min Value" />
							</fieldset>
							<fieldset class="fieldset">
								<legend class="fieldset-legend">Max Value</legend>
								<input bind:value={maxValue} type="number" class="input w-full" placeholder="Max Value" />
							</fieldset>
						</div>
					{:else if questionType === QuestionTypeEnum.WordCloud}
						<fieldset class="fieldset">
							<legend class="fieldset-legend">Max Words</legend>
							<input bind:value={maxWords} type="number" class="input w-full" placeholder="Max Words" />
						</fieldset>
					{:else if questionType === QuestionTypeEnum.FreeText}
						<fieldset class="fieldset">
							<legend class="fieldset-legend">Maximal Inputs</legend>
							<input bind:value={maxWords} type="number" class="input w-full" placeholder="Maximal Inputs" />
						</fieldset>
					{/if}

					<div class="mt-4 flex flex-col gap-2">
						<button
							class="btn btn-secondary"
							type="reset"
							onclick={() => ref?.close()}
							disabled={isLoading || isLoadingData}>
							Cancel
						</button>
						<button class="btn btn-primary" type="submit" disabled={isLoading || isLoadingData}>
							{#if isLoading}
								<LoaderCircle class="animate-spin" />
							{/if}
							Update Question
						</button>
					</div>
				</form>
			{/if}
		</div>
	</div>
</dialog>
