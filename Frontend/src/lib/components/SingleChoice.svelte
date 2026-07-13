<script lang="ts">
	import type { ClassValue } from 'svelte/elements';
	import type { Option } from '$lib/components/MultipleChoice.svelte';

	type Props = {
		options?: Option[];
		value?: number;
		class?: ClassValue;
		style?: string;
	};

	let { options = $bindable([]), class: classes, style }: Props = $props();

	let radioName = crypto.randomUUID();
	let value: number = $state(-1);

	$effect(() => {
		if (value >= 0 && value < options.length) {
			options.forEach((x, index) => (x.checked = index === value));
		}
	});
</script>

<div class={['flex w-full flex-col items-center gap-2 md:max-w-120', classes]} {style}>
	{#each options as option, index}
		<label
			class={[
				'flex w-full cursor-pointer gap-2 rounded-box bg-base-100 p-2',
				option.checked && 'outline outline-primary',
			]}>
			<input
				type="radio"
				class="radio radio-primary"
				name={radioName}
				value={index}
				bind:group={value} />
			{option.answerOption.description}
		</label>
	{/each}
</div>
