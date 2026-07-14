<script lang="ts">
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		statusCode?: number;
		statusMessage?: string;
		class?: ClassValue;
		style?: string;
	};

	let { statusCode, statusMessage, class: classes, style }: Props = $props();
</script>

<div
	class={['flex h-full flex-col items-center justify-center gap-4 text-center', classes]}
	{style}>
	<h1 class="text-6xl font-bold text-error">{statusCode}</h1>

	<p class="text-xl">{statusMessage}</p>

	<div class="flex gap-2">
		<a href="/app/library" class="btn mt-4 btn-outline">Back to Library</a>
		<a href="/app" class="btn mt-4 btn-primary">Back to Dashboard</a>
	</div>
</div>
