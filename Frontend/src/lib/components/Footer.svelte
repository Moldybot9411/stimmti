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
		isSmall
			? 'footer items-center border-t border-base-300 bg-base-100 p-4 text-base-content sm:footer-horizontal'
			: 'footer border-t border-base-100 bg-base-200 p-10 text-base-content sm:footer-horizontal',
	].join(' ')}>
	{#if !isSmall}
		<aside>
			<a href="/" aria-label="Go to homepage">
				<img
					src={logoSrc}
					alt="Stimmti Logo"
					class="max-h-20 w-full max-w-[26rem] object-contain object-left" />
			</a>

			<p>
				Creating surveys for everyone,
				<br />
				whether professional, for fun, or to gather opinions.
			</p>
		</aside>

		<nav>
			<h6 class="footer-title">Services</h6>
			<a class="link link-hover" href="/help">Help</a>
		</nav>
		<nav>
			<h6 class="footer-title">Legal</h6>
			<a class="link link-hover" href="/privacypolicy">Privacy policy</a>
			<a href="/legalnotice" class="link link-hover">Legal notice</a>
		</nav>
	{:else}
		<aside class="grid-flow-col items-center gap-3">
			<p class="text-xs sm:text-sm">
				Creating surveys for everyone, whether professional, for fun, or to gather opinions.
			</p>
		</aside>
		<nav class="grid-flow-col gap-2 md:place-self-center md:justify-self-end">
			<a class="btn btn-ghost btn-sm" href="/help">Help</a>
			<a class="btn btn-ghost btn-sm" href="/privacypolicy">Privacy</a>
			<a class="btn btn-ghost btn-sm" href="/legalnotice">Legal</a>
		</nav>
	{/if}
</footer>
