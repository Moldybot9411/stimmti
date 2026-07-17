<script lang="ts">
	import NavigationBar from '$lib/components/NavigationBar.svelte';
	import OpenSessionNotice from '$lib/components/OpenSessionNotice.svelte';
	import { SquareKanban } from '@lucide/svelte';

	let { children, data } = $props();
</script>

<NavigationBar />

{#await data.openSessions then openSessions}
	{#if openSessions.length > 0}
		<OpenSessionNotice {openSessions} />
	{/if}
{/await}

<div class="p-4">
	{@render children()}
</div>
