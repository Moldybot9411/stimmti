<script lang="ts">
	import { themeManager } from '$lib/Theme.svelte';
	import { LoaderCircle } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';
	import { fade, fly, slide } from 'svelte/transition';

	type Props = {
		text?: string;
		class?: ClassValue;
		style?: string;
	};

	let { text = 'Loading', class: classes, style }: Props = $props();

	const logoSrc = $derived(
		themeManager.theme === 'light'
			? '/stimmti-logo-notagline.svg'
			: '/stimmti-logo-notagline-light.svg'
	);
</script>

<div
	class={[
		'fixed inset-0 z-50 flex flex-col items-center justify-center bg-base-100 px-4',
		classes,
	]}
	{style}
	in:slide={{
		axis: 'x',
	}}
	out:slide={{ axis: 'x' }}>
	<img src={logoSrc} alt="Logo" class="w-70 md:w-150" />
	<div
		class="mt-4 flex max-w-full flex-wrap items-center justify-center gap-2 text-center text-xl font-bold">
		<LoaderCircle class="shrink-0 animate-spin" />
		<span>{text}</span>
	</div>
</div>
