<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';
	import { onMount } from 'svelte';
	import { themeManager } from '$lib/Theme.svelte';
	import { loginUser } from '$lib/authStore.svelte';
	import Toast from '$lib/components/Toast/Toast.svelte';

	let { children, data } = $props();

	onMount(() => {
		themeManager.init();
	});

	$effect.pre(() => {
		if (!data.user) return;

		loginUser(data.user);
	});
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>
{@render children()}

<Toast />

<footer class="footer sm:footer-horizontal bg-base-200 text-base-content p-10">
	<aside>
		<a href="/" aria-label="Go to homepage"> 
			<img src="stimmti-logo-light.svg" alt="Stimmti Logo" class="w-120" /> 
		</a>
	  <p>
		<br />
		Creating surveys for everyone, whether professional, 
		<br />
		for fun, or to gather opinions.
	  </p>
	</aside>
	<nav>
	  <h6 class="footer-title">Services</h6>
	  <a class="link link-hover">Help</a>
	</nav>
	<nav>
	  <h6 class="footer-title">Company</h6>
	  <a class="link link-hover">About us</a>
	</nav>
	<nav>
	  <h6 class="footer-title">Legal</h6>
	  <a class="link link-hover" href="/privacypolicy">Privacy policy</a>
	  <a href="/legalnotice" class="link link-hover">Legal Notice</a>
	</nav>
  </footer>