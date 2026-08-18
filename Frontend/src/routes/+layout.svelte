<script lang="ts">
	import './layout.css';
	import { onMount } from 'svelte';
	import { themeManager } from '$lib/Theme.svelte';
	import { loginUser } from '$lib/authStore.svelte';
	import Toast from '$lib/components/Toast/Toast.svelte';
	import Footer from '$lib/components/Footer.svelte';
	import { page } from '$app/state';

	let { children, data } = $props();
	const isMinimalFooter = $derived(
		page.url.pathname.startsWith('/app') ||
			page.url.pathname.startsWith('/live') ||
			page.url.pathname.startsWith('/login') ||
			page.url.pathname.startsWith('/register')
	);

	onMount(() => {
		themeManager.init();
	});

	$effect(() => {
		if (!data.user) return;

		loginUser(data.user);
	});
</script>

<div class="flex min-h-screen flex-col">
	<main class="flex-1">
		{@render children()}
	</main>

	<Footer size={isMinimalFooter ? 'small' : 'default'} />
</div>

<Toast />
