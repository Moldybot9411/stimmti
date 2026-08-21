<script lang="ts">
	import { ChevronLeft } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		class?: ClassValue;
		style?: string;
	};

	let { class: classes, style }: Props = $props();
</script>

<button
	class={['btn btn-lg', classes]}
	{style}
	aria-label="Navigate Back"
	onclick={() => history.back()}>
	<ChevronLeft />
</button>
