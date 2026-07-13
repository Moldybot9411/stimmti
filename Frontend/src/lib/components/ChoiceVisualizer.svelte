<script lang="ts">
	import type { AnswerDisplayDto as WsAnswerDisplayDto } from '$lib/wsClient/Backend.Dto';
	import type { AnswerDisplayDto as APIAnswerDisplayDto } from '$lib/api';
	import type { Option } from './MultipleChoice.svelte';

	type Props = {
		answers?: WsAnswerDisplayDto | APIAnswerDisplayDto;
		choiceOptions?: Option[];
	};

	let { answers, choiceOptions = [] }: Props = $props();

	const totalAnswers = $derived(
		answers?.choiceResults?.reduce((acc, curr) => acc + (curr.count ?? 0), 0) ?? 0
	);
</script>

<div class="flex w-full flex-col gap-4">
	{#each choiceOptions as item}
		{@const answerItem = answers?.choiceResults?.find(
			(x) => x.answerOption.id === item.answerOption.id
		)}
		{@const percentage =
			totalAnswers === 0 ? 0 : ((answerItem?.count ?? 0) / totalAnswers) * 100}

		<div>
			<div class="mb-2 flex items-center justify-between gap-2">
				<div>
					<span class="text-lg font-bold">
						{item.answerOption.description}
					</span>

					<div class="ml-2 badge badge-outline">{answerItem?.count ?? 0}</div>
				</div>

				<span class="opcaity-80">
					({Math.round(percentage * 10) / 10}%)
				</span>
			</div>

			<div
				class="relative min-h-16 w-full overflow-hidden rounded-box border border-base-content/10 bg-base-100 shadow-md">
				<div
					class={[
						'absolute left-0 h-full w-full transition-all',
						percentage < 33.33 && 'bg-error',
						percentage >= 33.33 && percentage < 66.66 && 'bg-warning',
						percentage >= 66.66 && 'bg-success',
					]}
					style="transform: translateX(calc(-100% + {percentage}%));">
				</div>
			</div>
		</div>
	{/each}
</div>
