<script lang="ts">
	import { QuestionTypeEnum } from '$lib/api.js';
	import ChoiceVisualizer from '$lib/components/ChoiceVisualizer.svelte';
	import type { Option } from '$lib/components/MultipleChoice.svelte';
	import NumberScaleVisualizer from '$lib/components/NumberScaleVisualizer.svelte';
	import WordCloud from '$lib/components/WordCloud.svelte';
	import { Quote } from '@lucide/svelte';

	let { data } = $props();
</script>

<div class="flex w-full flex-col items-center gap-2">
	<h1 class="text-center text-4xl font-bold">{data.questionResults.name}</h1>
	{#if data.questionResults.description}
		<h2 class="text-center text-2xl font-bold opacity-80 md:max-w-120">
			{data.questionResults.description}
		</h2>
	{/if}
</div>

<div class="divider mb-8">{new Date(data.questionResults.openedAt).toDateString()}</div>

<div class="flex w-full flex-col items-center gap-4">
	{#each data.questionResults.questions as question}
		{@const questionType = question.questionTemplateDto.questionType}
		<div class="collapse-arrow collapse w-full border border-base-300 bg-base-100">
			<input type="checkbox" name="question-accordion" />
			<div class="collapse-title text-xl font-bold">
				{question.questionTemplateDto.name}
			</div>
			<div class="collapse-content">
				<span class="opcaity-80 text-sm">
					({question.answerDisplayDto.totalParticipantsAnswered} participant/s answered)
				</span>
				<div class="divider"></div>

				{#if questionType === QuestionTypeEnum.MultipleChoice || questionType === QuestionTypeEnum.SingleChoice}
					<ChoiceVisualizer
						answers={question.answerDisplayDto}
						choiceOptions={(question.questionTemplateDto.answerOptions?.map((el) => {
							return { answerOption: el, checked: undefined };
						}) as Option[]) ?? []} />
				{:else if questionType === QuestionTypeEnum.FreeText}
					<div class="flex w-full justify-center">
						<div class="grid w-full grid-cols-1 gap-2 md:grid-cols-2 xl:grid-cols-4">
							{#each question.answerDisplayDto.freeTextResults as answer}
								<div
									class="card bg-base-200 text-balance wrap-anywhere text-base-content shadow">
									<div class="card-body whitespace-pre-wrap">
										<Quote class="fill-base-content text-base-content/0" />
										<p class="text-lg leading-relaxed italic">
											{answer.text}
										</p>
									</div>
								</div>
							{/each}
						</div>
					</div>
				{:else if questionType === QuestionTypeEnum.NumberScale}
					<div class="flex justify-center">
						<NumberScaleVisualizer
							answers={question.answerDisplayDto}
							scaleMinValue={question.questionTemplateDto.minValue ?? 1}
							scaleMaxValue={question.questionTemplateDto.maxValue ?? 10}
							scaleToMax={true} />
					</div>
				{:else if questionType === QuestionTypeEnum.WordCloud}
					<div class="flex w-full justify-center">
						<WordCloud answers={question.answerDisplayDto} />
					</div>
				{/if}
			</div>
		</div>
	{/each}
</div>
