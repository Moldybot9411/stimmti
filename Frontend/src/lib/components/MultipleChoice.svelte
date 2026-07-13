<script lang="ts" module>
	import type { AnswerOptionDto } from '$lib/wsClient/Backend.Dto';

	export type Option = {
		answerOption: AnswerOptionDto;
		checked?: boolean;
	};
</script>

<script lang="ts">
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		options?: Option[];
		class?: ClassValue;
		style?: string;
	};

	let { options = $bindable([]), class: classes, style }: Props = $props();
</script>

<div class={['flex w-full flex-col items-center gap-2 md:max-w-120', classes]} {style}>
	{#each options as option}
		<label
			class={[
				'flex w-full cursor-pointer gap-2 rounded-box bg-base-100 p-2',
				option.checked && 'outline outline-primary',
			]}>
			<input
				type="checkbox"
				class="checkbox checkbox-primary"
				bind:checked={option.checked} />
			{option.answerOption.description}
		</label>
	{/each}
</div>
