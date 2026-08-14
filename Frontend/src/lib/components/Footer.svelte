<script lang="ts">
	import { themeManager } from '$lib/Theme.svelte';

	type FooterSize = 'default' | 'small';

	let { size = 'default' }: { size?: FooterSize } = $props();
	const isSmall = $derived(size === 'small');
	const logoSrc = $derived(
		themeManager.theme === 'light' ? '/stimmti-logo.svg' : '/stimmti-logo-light.svg'
	);
</script>

<footer
	class={[
		'flex w-full flex-col items-center justify-center rounded-t-box border-t text-center text-base-content sm:flex-row sm:justify-between',
		isSmall
			? 'gap-3 border-base-300 bg-base-100 p-4 sm:items-center'
			: 'gap-8 border-base-100 bg-base-200 p-10 sm:items-start sm:text-left',
	]}>
	{#if !isSmall}
		<aside class="flex w-full flex-col items-center sm:w-auto sm:items-start">
			<a href="/" aria-label="Go to homepage">
				<img
					src={logoSrc}
					alt="Stimmti Logo"
					class="h-20 w-auto max-w-[12rem] object-contain object-center sm:h-24 sm:max-w-[26rem] sm:object-left" />
			</a>

			<p>
				Creating surveys for everyone,
				<br />
				whether professional, for fun, or to gather opinions.
			</p>
		</aside>

		<nav
			class="flex w-full flex-col items-center text-center sm:w-auto sm:items-start sm:text-left">
			<h6 class="footer-title">Services</h6>
			<a class="link link-hover" href="/help">Help</a>
		</nav>
		<nav
			class="flex w-full flex-col items-center text-center sm:w-auto sm:items-start sm:text-left">
			<h6 class="footer-title">Legal</h6>
			<a class="link link-hover" href="/privacypolicy">Privacy policy</a>
			<a href="/legalnotice" class="link link-hover">Legal notice</a>
		</nav>
	{:else}
		<aside class="w-full text-center sm:w-auto sm:text-left">
			<p class="text-xs sm:text-sm">
				Creating surveys for everyone, whether professional, for fun, or to gather opinions.
			</p>
		</aside>
		<nav class="flex w-full flex-col items-center gap-2 sm:w-auto sm:flex-row sm:justify-end">
			<a class="btn btn-ghost btn-sm" href="/help">Help</a>
			<a class="btn btn-ghost btn-sm" href="/privacypolicy">Privacy</a>
			<a class="btn btn-ghost btn-sm" href="/legalnotice">Legal</a>
		</nav>
	{/if}
</footer>
