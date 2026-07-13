<script lang="ts">
	import './layout.css';
	import { onMount } from 'svelte';
	import { themeManager } from '$lib/Theme.svelte';
	import { loginUser } from '$lib/authStore.svelte';
	import Toast from '$lib/components/Toast/Toast.svelte';
	import Footer from '$lib/components/Footer.svelte';
	import { page } from '$app/state';

	let { children, data } = $props();
	const isMinimalFooter = $derived(page.url.pathname.startsWith('/app'));
	const faviconHref = $derived(
		themeManager.theme === 'light' ? '/stimmti-logo.svg' : '/stimmti-logo-light.svg'
	);

	onMount(() => {
		themeManager.init();
	});

	$effect(() => {
		if (!data.user) return;

		loginUser(data.user);
	});
</script>

<svelte:head>
	<link rel="icon" type="image/svg+xml" href={faviconHref} />
</svelte:head>

<div class="flex min-h-screen flex-col">
	<div class="flex-1">
		{@render children()}
	</div>

	<Footer size={isMinimalFooter ? 'small' : 'default'} />
</div>

<Toast />