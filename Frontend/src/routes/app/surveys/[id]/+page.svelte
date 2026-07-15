<script lang="ts">
	import { QuestionTypeEnum, type GetQuestionTemplateResponseDto } from '$lib/api.js';
	import { Plus } from '@lucide/svelte';

	let { data } = $props<{ data: { questions: GetQuestionTemplateResponseDto[] } }>();
</script>

<div class="mb-8 flex w-full flex-col items-center gap-2">
	<h1 class="text-center text-4xl font-bold">Survey Questions</h1>
	<p class="text-center opacity-80">{data.questions.length} question(s)</p>
</div>

<ul class="timeline timeline-vertical timeline-snap-icon max-md:timeline-compact">
	{#each data.questions as question , index}
		<li>
			<div class="timeline-middle ">
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
				<div >Question {index} </div>
				<div class="timeline-box min-w-150 w-full rounded-lg border border-base-300 bg-base-100 p-4 shadow-sm">
					<div class="text-lg font-bold">{question.name} - {question.questionType}</div>
					<div class="text-sm opacity-80">{question.description}</div>

				{#if question.questionType === QuestionTypeEnum.NumberScale}
					<div>Scale: {question.minValue ?? 1} - {question.maxValue ?? 10}</div>
				{:else if question.questionType === QuestionTypeEnum.WordCloud}
					<div>Max words: {question.maxWords ?? 0}</div>
				{:else if question.answers?.length}
				
				<div class="grid direction-row grid-cols-2 gap-2">
					{#each question.answers as answer}
						<div class="p-2 min-w-75 border rounded flex justify-center">{answer}</div>
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
			<button class="btn btn-outline btn-primary btn-sm">
				<Plus class="mr-2 h-4 w-4" />
				Add Question
			</button>
		</div>
	</li>
</ul>
