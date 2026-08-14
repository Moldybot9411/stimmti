<script lang="ts">
	import { Check, HeartCrack, LoaderCircle, X } from '@lucide/svelte';
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
		questionId?: string | null;
		onSave?: (questionData: GetQuestionTemplateResponseDto) => void;
		onCancel?: () => void;
	};

	let {
		class: classes,
		style,
		ref = $bindable(),
		onSave = () => {},
		onCancel = () => {},
		questionId,
	}: Props = $props();

	let formRef: HTMLFormElement | null = $state(null);
	let isLoading = $state(false);
	let isLoadingData = $state(false);

	let currentQuestionData: GetQuestionTemplateResponseDto = $state(getDefaultQuestion());

	function getDefaultQuestion(): GetQuestionTemplateResponseDto {
		return {
			name: '',
			description: '',
			questionType: QuestionTypeEnum.SingleChoice,
			minValue: 1,
			maxValue: 10,
			answers: ['', ''],
		};
	}

	$effect(() => {
		if (currentQuestionData.minValue! >= currentQuestionData.maxValue!) {
			currentQuestionData.minValue = currentQuestionData.maxValue! - 1;
		}
	});

	// Load question data when questionId changes
	$effect(() => {
		if (!questionId) return;

		isLoadingData = true;
		apiClient.api
			.v1QuestionsDetail(questionId)
			.then((result) => {
				if (result.data) {
					currentQuestionData = result.data;

					if (!currentQuestionData.answers || currentQuestionData.answers.length === 0) {
						currentQuestionData.answers = ['', ''];
					} else {
						currentQuestionData.answers = [...currentQuestionData.answers, ''];
					}
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

	function handleAnswerInput(index: number) {
		const answers = currentQuestionData.answers ?? [];
		const isLastField = index === answers.length - 1;

		if (!isLastField || !answers[index]?.trim()) return;

		const hasOtherEmptyField = answers.slice(0, -1).some((a) => a.trim().length === 0);

		if (!hasOtherEmptyField && answers.length < maxChoiceAnswers) {
			currentQuestionData.answers = [...answers, ''];
		}
	}

	function reset() {
		formRef?.reset();
		isLoading = false;
		currentQuestionData = getDefaultQuestion();
	}

	function handleClose() {
		ref?.close();
		onCancel?.();
	}

	async function updateQuestion() {
		if (!questionId) return;

		const payload = { ...currentQuestionData };
		payload.answers = (payload.answers ?? []).map((x) => x.trim()).filter((x) => x.length > 0);

		if (
			(payload.questionType === QuestionTypeEnum.SingleChoice ||
				payload.questionType === QuestionTypeEnum.MultipleChoice) &&
			payload.answers.length < 2
		) {
			addToast({
				type: 'error',
				label: 'Please provide at least 2 answer options',
				icon: HeartCrack,
			});
			return;
		}

		if (
			payload.questionType === QuestionTypeEnum.NumberScale &&
			(payload.minValue ?? 1) >= (payload.maxValue ?? 10)
		) {
			addToast({
				type: 'error',
				label: 'Min value must be smaller than max value',
				icon: HeartCrack,
			});
			return;
		}

		if (payload.questionType === QuestionTypeEnum.WordCloud && (payload.maxWords ?? 0) <= 0) {
			addToast({
				type: 'error',
				label: 'Max words must be greater than 0',
				icon: HeartCrack,
			});
			return;
		}

		isLoading = true;
		try {
			const updatedResult = await apiClient.api.v1QuestionsPartialUpdate(questionId, payload);
			const newQuestionId = updatedResult.data;

			const fetchResult = await apiClient.api.v1QuestionsDetail(newQuestionId);
			const updatedData = fetchResult.data;

			addToast({
				type: 'success',
				label: 'Question updated successfully',
				icon: Check,
			});

			onSave(updatedData);
			ref?.close();
		} catch (error) {
			const errorMsg = error instanceof Error ? error.message : 'Unknown error';
			addToast({
				type: 'error',
				label: `Failed to update question: ${errorMsg}`,
				icon: HeartCrack,
			});
		} finally {
			isLoading = false;
		}
	}
</script>

<dialog class={['modal', classes]} {style} bind:this={ref} onclose={reset}>
	<div class="modal-box">
		<form method="dialog">
			<button
				class="btn absolute top-2 right-2 btn-ghost btn-sm"
				disabled={isLoading || isLoadingData}
				onclick={handleClose}>
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
						<input
							bind:value={currentQuestionData.name}
							type="text"
							class="input w-full"
							placeholder="My Question"
							required />
					</fieldset>

					<fieldset class="fieldset">
						<legend class="fieldset-legend">Question Description</legend>
						<input
							bind:value={currentQuestionData.description}
							type="text"
							class="input w-full"
							placeholder="My Description" />
					</fieldset>

					{#if currentQuestionData.questionType === QuestionTypeEnum.SingleChoice || currentQuestionData.questionType === QuestionTypeEnum.MultipleChoice}
						<fieldset class="fieldset">
							<legend class="fieldset-legend">Answers</legend>
							<div class="flex flex-col gap-2">
								{#each currentQuestionData.answers ?? [] as _, index}
									<input
										bind:value={currentQuestionData.answers![index]}
										oninput={() => handleAnswerInput(index)}
										type="text"
										class="input w-full"
										placeholder={`Answer ${index + 1}`} />
								{/each}
							</div>
							<div class="label overflow-auto">
								<span class="label-text-alt"
									>A new field is created while typing in the last field when no
									other empty field exists, up to 8 answers.</span>
							</div>
						</fieldset>
					{:else if currentQuestionData.questionType === QuestionTypeEnum.NumberScale}
						<div class="grid grid-cols-2 gap-2">
							<fieldset class="fieldset">
								<legend class="fieldset-legend">Min Value</legend>
								<input
									bind:value={currentQuestionData.minValue}
									type="number"
									class="input w-full"
									placeholder="Min Value" />
							</fieldset>
							<fieldset class="fieldset">
								<legend class="fieldset-legend">Max Value</legend>
								<input
									bind:value={currentQuestionData.maxValue}
									type="number"
									class="input w-full"
									placeholder="Max Value" />
							</fieldset>
						</div>
					{:else if currentQuestionData.questionType === QuestionTypeEnum.WordCloud}
						<fieldset class="fieldset">
							<legend class="fieldset-legend">Maximum Answers</legend>
							<input
								bind:value={currentQuestionData.maxWords}
								type="number"
								class="input w-full"
								placeholder="Max Words" />
						</fieldset>
					{/if}

					<div class="mt-4 flex flex-col gap-2">
						<button
							class="btn btn-secondary"
							type="reset"
							onclick={handleClose}
							disabled={isLoading || isLoadingData}>
							Cancel
						</button>
						<button
							class="btn btn-primary"
							type="submit"
							disabled={isLoading || isLoadingData}>
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
