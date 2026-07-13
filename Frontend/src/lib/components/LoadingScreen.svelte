<script lang="ts">
	import { LoaderCircle } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';
	import { fade, fly, slide } from 'svelte/transition';

	type Props = {
		text?: string;
		class?: ClassValue;
		style?: string;
	};

	let { text = 'Loading', class: classes, style }: Props = $props();
</script>

<div
	class={['fixed inset-0 z-50 flex flex-col items-center justify-center bg-primary', classes]}
	{style}
	in:slide={{
		axis: 'x',
	}}
	out:slide={{ axis: 'x' }}>
	<div class="text-6xl font-bold text-white md:text-[10rem]">Stimmti</div>
	<div class="mt-4 flex gap-2 text-xl font-bold text-white">
		<LoaderCircle class="animate-spin" />
		{text}
	</div>
</div>
