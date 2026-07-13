<script lang="ts">
	import type { AnswerDisplayDto } from '$lib/wsClient/Backend.Dto';
	import { QuestionTypeEnum } from '$lib/wsClient/Backend.Models.Enums';
	import { slide } from 'svelte/transition';
	import type { Option } from './MultipleChoice.svelte';
	import WordCloud from './WordCloud.svelte';
	import NumberScaleVisualizer from './NumberScaleVisualizer.svelte';
	import { Quote } from '@lucide/svelte';
	import { flip } from 'svelte/animate';
	import ChoiceVisualizer from './ChoiceVisualizer.svelte';

	type Props = {
		answers?: AnswerDisplayDto;
		questionType?: QuestionTypeEnum;
		participantCount?: number;
		choiceOptions?: Option[];
		scaleMinValue?: number;
		scaleMaxValue?: number;
	};

	let {
		answers,
		questionType,
		participantCount,
		choiceOptions = [],
		scaleMinValue,
		scaleMaxValue,
	}: Props = $props();
</script>

<div class="mb-12 flex items-center gap-2 self-start text-sm text-base-content/50">
	<span>Participants:</span>
	<div class="badge badge-outline badge-sm text-base-content/70">
		{answers?.totalParticipantsAnswered ?? 0} / {participantCount ?? 0}
	</div>
</div>

{#if questionType === QuestionTypeEnum.SingleChoice || questionType === QuestionTypeEnum.MultipleChoice}
	<ChoiceVisualizer {answers} {choiceOptions} />
{:else if questionType === QuestionTypeEnum.WordCloud}
	<WordCloud {answers} />
{:else if questionType === QuestionTypeEnum.FreeText}
	<div class="grid w-full grid-cols-1 gap-4 sm:grid-cols-2 md:max-w-5xl">
		{#each answers?.freeTextResults ?? [] as answer (answer.id)}
			<div
				class="card bg-base-100 text-balance wrap-anywhere text-base-content shadow"
				in:slide
				animate:flip={{ duration: 400 }}>
				<div class="card-body whitespace-pre-wrap">
					<Quote class="fill-base-content text-base-content/0" />
					<p class="text-lg leading-relaxed italic">
						{answer.text}
					</p>
				</div>
			</div>
		{/each}
	</div>
{:else if questionType === QuestionTypeEnum.NumberScale}
	<NumberScaleVisualizer {answers} {scaleMinValue} {scaleMaxValue} />
{/if}
