<script lang="ts">
	import { goto, invalidateAll, replaceState } from '$app/navigation';
	import BackButton from '$lib/components/BackButton.svelte';
	import HideScrollbar from '$lib/components/HideScrollbar.svelte';
	import Register from '$lib/components/Register.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import { ChevronLeft, CircleCheckBig } from '@lucide/svelte';

	let registrationFinished = $state(false);

	let progressValue = $state(100);

	$effect(() => {
		if (!registrationFinished) return;

		const interval = setInterval(() => {
			if (progressValue > 0) {
				progressValue -= 1;
			} else {
				clearInterval(interval);

				goto('/app', { invalidateAll: true });
			}
		}, 50);

		return () => clearInterval(interval);
	});
</script>

<HideScrollbar />

<ThemeToggle class="fixed top-4 right-4" />

<BackButton class="fixed m-4" />

<div class="flex min-h-screen w-full items-center justify-center">
	{#if !registrationFinished}
		<Register onregisterfinish={() => (registrationFinished = true)} />
	{:else}
		<div class="card w-96 bg-base-100 shadow-sm card-lg">
			<div class="card-body">
				<h2 class="card-title text-success">
					<CircleCheckBig />
					Registration Successful
				</h2>

				<progress class="progress w-full progress-success" value={progressValue} max="100"
				></progress>
			</div>
		</div>
	{/if}
</div>
