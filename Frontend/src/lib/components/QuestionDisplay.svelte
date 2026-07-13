<script lang="ts">
	import type { AnswerDisplayDto, AnswerOptionDto } from '$lib/wsClient/Backend.Dto';
	import { ParticipantRole, QuestionTypeEnum } from '$lib/wsClient/Backend.Models.Enums';
	import MultipleChoice, { type Option } from './MultipleChoice.svelte';
	import NumberScale from './NumberScale.svelte';
	import SingleChoice from './SingleChoice.svelte';
	import LoadingScreen from './LoadingScreen.svelte';
	import AnswerVisualizer from './AnswerVisualizer.svelte';
	import { addToast } from './Toast/Toast.svelte';
	import { TextCursorInput } from '@lucide/svelte';

	type Props = {
		questionName?: string;
		questionDescription?: string;
		role?: ParticipantRole;
		questionType?: QuestionTypeEnum;
		choiceOptions?: Option[];
		numWordCloudInputs?: number;
		scaleMinValue?: number;
		scaleMaxValue?: number;
		finishedAnsering?: boolean;
		participantCount?: number;

		answers?: AnswerDisplayDto;

		onNextQuestion?: () => void;
		onSubmitAnswer?: (
			answerOptions?: AnswerOptionDto[],
			wordCloudTexts?: string[],
			text?: string,
			value?: number
		) => void;
	};

	let {
		questionName,
		questionDescription,
		role,
		questionType,
		choiceOptions = $bindable([]),
		numWordCloudInputs,
		scaleMinValue,
		scaleMaxValue,
		finishedAnsering,
		participantCount,
		answers,
		onNextQuestion,
		onSubmitAnswer,
	}: Props = $props();

	let freeTextValue: string = $state('');
	let wordCloudValues: string[] = $state([]);
	let numberScaleValue: number = $state(0);

	function submitAnswer() {
		switch (questionType) {
			case QuestionTypeEnum.MultipleChoice:
				onSubmitAnswer?.(
					choiceOptions?.filter((x) => x.checked).map((x) => x.answerOption)
				);
				break;
			case QuestionTypeEnum.SingleChoice:
				const firstMatch = choiceOptions?.find((x) => x.checked)?.answerOption;

				onSubmitAnswer?.(firstMatch !== undefined ? [firstMatch] : []);
				break;
			case QuestionTypeEnum.NumberScale:
				onSubmitAnswer?.(undefined, undefined, undefined, numberScaleValue);
				break;
			case QuestionTypeEnum.WordCloud:
				if (wordCloudValues.some((el) => el.trim().split(' ').length > 1)) {
					addToast({
						label: 'Only one word per field allowed',
						type: 'error',
						icon: TextCursorInput,
					});

					return;
				}

				onSubmitAnswer?.(undefined, wordCloudValues);
				break;
			case QuestionTypeEnum.FreeText:
				onSubmitAnswer?.(undefined, undefined, freeTextValue);
				break;
		}
	}
</script>

<h1 class="mx-auto mt-8 w-fit px-4 text-center text-5xl font-bold text-balance">
	{questionName}
</h1>

{#if questionDescription}
	<p class="mx-auto mt-4 w-fit px-4 text-center text-3xl opacity-80">
		{questionDescription}
	</p>
{/if}

<div class="divider"></div>

{#if role === ParticipantRole.Participant}
	{#if finishedAnsering}
		<LoadingScreen text="Waiting for host to continue" />
	{:else}
		{#if questionType === QuestionTypeEnum.MultipleChoice}
			<MultipleChoice bind:options={choiceOptions} />
		{:else if questionType === QuestionTypeEnum.NumberScale}
			<div class="card w-full bg-base-100 md:max-w-120">
				<div class="card-body w-full p-12 md:p-6">
					<NumberScale
						minValue={scaleMinValue}
						maxValue={scaleMaxValue}
						bind:value={numberScaleValue} />
				</div>
			</div>
		{:else if questionType === QuestionTypeEnum.SingleChoice}
			<SingleChoice bind:options={choiceOptions} />
		{:else if questionType === QuestionTypeEnum.WordCloud}
			<div class="flex w-full flex-col gap-2 md:max-w-120">
				<span class="fieldset-legend text-lg">Describe in one word per input</span>
				{#each Array(numWordCloudInputs) as _, index}
					<input
						type="text"
						class="input w-full min-w-0"
						placeholder={`Word ${index + 1}`}
						bind:value={wordCloudValues[index]} />
				{/each}
			</div>
		{:else if questionType === QuestionTypeEnum.FreeText}
			<fieldset class="mx-auto fieldset w-full md:max-w-120">
				<legend class="fieldset-legend text-lg">Enter your thoughts</legend>
				<textarea class="textarea w-full" maxlength={256} bind:value={freeTextValue}
				></textarea>
				<p>{freeTextValue.length}/256</p>
			</fieldset>
		{/if}
		<div class="mt-16 flex justify-center">
			<button class="btn btn-primary btn-xl" onclick={submitAnswer}>Submit</button>
		</div>
	{/if}
{/if}

{#if role === ParticipantRole.Presenter}
	<AnswerVisualizer
		{answers}
		{questionType}
		{choiceOptions}
		{scaleMinValue}
		{scaleMaxValue}
		{participantCount} />

	<button
		class="btn fixed right-4 bottom-4 shadow btn-primary btn-xl"
		onclick={() => onNextQuestion?.()}>
		Continue
	</button>
{/if}
