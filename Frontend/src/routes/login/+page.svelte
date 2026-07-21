<script lang="ts">
	import { goto } from '$app/navigation';
	import Login from '$lib/components/Login.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { ChevronLeft } from '@lucide/svelte';
</script>

<ThemeToggle class="fixed top-4 right-4" />

<button
	class="btn fixed mt-4 mb-4 ml-4 btn-lg"
	aria-label="Navigate Back"
	onclick={() => history.back()}>
	<ChevronLeft />
</button>

<div class="flex min-h-screen w-full items-center justify-center">
	<Login onloginfinished={() => goto('/app', { invalidateAll: true })} />
</div>
