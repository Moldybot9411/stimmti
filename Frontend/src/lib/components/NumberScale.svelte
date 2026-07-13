<script lang="ts">
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		minValue?: number;
		maxValue?: number;
		value?: number;
		class?: ClassValue;
		style?: string;
	};

	let {
		minValue = 0,
		maxValue = 10,
		value = $bindable(),
		class: classes,
		style,
	}: Props = $props();

	$effect(() => {
		value = minValue;
	});

	let percent = $derived((value ?? minValue - minValue) / (maxValue - minValue));
</script>

<div class={['flex w-full flex-col items-center gap-4', classes]}>
	<span
		class={[
			'text-3xl font-bold transition-colors',
			percent < 0.33 && 'text-error',
			percent >= 0.33 && percent < 0.66 && 'text-warning',
			percent >= 0.66 && 'text-success',
		]}>{value}</span>

	<div class="flex w-full items-center gap-4">
		<span class="text-lg font-bold">
			{minValue}
		</span>
		<input
			{style}
			type="range"
			min={minValue}
			max={maxValue}
			bind:value
			class={[
				'range w-full range-accent transition-colors',
				percent < 0.33 && 'range-error',
				percent >= 0.33 && percent < 0.66 && 'range-warning',
				percent >= 0.66 && 'range-success',
			]} />
		<span class="text-lg font-bold">
			{maxValue}
		</span>
	</div>
</div>
