<script lang="ts">
	import { goto } from '$app/navigation';
	import { HeartCrack, LoaderCircle, X } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';
	import { addToast } from './Toast/Toast.svelte';
	import { apiClient } from '$lib/apiClient';
	import { QuestionTypeEnum } from '$lib/api';
	import { createEventDispatcher } from 'svelte';
	const dispatch = createEventDispatcher();
	const maxChoiceAnswers = 8;

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		surveyId: string;
		onClose?: () => void | Promise<void>;
	};

	let { class: classes, style, ref = $bindable(), surveyId, onClose }: Props = $props();
	let formRef: HTMLFormElement | null = $state(null);
	let isLoading = $state(false);

	let name = $state('');
	let description = $state('');
	let questionType = $state(QuestionTypeEnum.SingleChoice);
	let isQuestionTypeDropdownOpen = $state(false);
	let minValue = $state(1);
	let maxValue = $state(10);
	let maxWords = $state(3);
	let answerInputs = $state(['', '']);

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
	}

	async function createQuestion() {
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
			.v1QuestionsCreate({
				name,
				description,
				surveyId,
				isArchived: false,
				questionType,
				answers:
					questionType === QuestionTypeEnum.SingleChoice ||
					questionType === QuestionTypeEnum.MultipleChoice
						? parsedAnswers
						: [],
				minValue: questionType === QuestionTypeEnum.NumberScale ? minValue : 0,
				maxValue: questionType === QuestionTypeEnum.NumberScale ? maxValue : 0,
				maxWords: questionType === QuestionTypeEnum.WordCloud ? maxWords : 0,
				orderNumber: undefined,
			})
			.then((result) => {
				if (result.status === 200) {
					dispatch('created');
					ref?.close();
                    
				}
			})
			.catch((error) => {
				addToast({
					type: 'error',
					label: `Survey Creation ran Into an error: ${error.message}`,
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
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm" disabled={isLoading}>
				<X />
			</button>
		</form>
		<h3 class="text-lg font-bold">New Question</h3>
		<div class="p-4">
			<form
				class=""
				bind:this={formRef}
				onsubmit={async (e) => {
					e.preventDefault();
					await createQuestion();
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

				<fieldset class="fieldset">
					<legend class="fieldset-legend">Question Type</legend>
					<div class={['dropdown dropdown-start w-full', isQuestionTypeDropdownOpen && 'dropdown-open']}>
						<button
							tabindex="0"
							type="button"
							onclick={() => {
								isQuestionTypeDropdownOpen = !isQuestionTypeDropdownOpen;
							}}
							class="btn btn-outline w-full justify-start">
							{questionType}
						</button>
						<ul class="dropdown-content menu bg-base-100 rounded-box z-50 w-52 p-2 shadow-lg">
							{#each Object.values(QuestionTypeEnum) as type}
								<li>
									<button
										type="button"
										class={questionType === type ? 'active' : ''}
										onclick={() => {
											questionType = type as QuestionTypeEnum;
											isQuestionTypeDropdownOpen = false;
										}}>
										{type}
									</button>
								</li>
							{/each}
						</ul>
					</div>
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
						disabled={isLoading}>
						Cancel
					</button>
					<button class="btn btn-primary" type="submit" disabled={isLoading}>
						{#if isLoading}
							<LoaderCircle class="animate-spin" />
						{/if}
						Create Question
					</button>
				</div>
			</form>
		</div>
	</div>
</dialog>
