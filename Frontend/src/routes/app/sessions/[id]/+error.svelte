<script lang="ts">
	import { page } from '$app/state';
</script>

<div class="flex h-full flex-col items-center justify-center gap-4 text-center">
	<h1 class="text-6xl font-bold text-error">{page.status}</h1>

	<p class="text-xl">{page.error?.message}</p>

	<div class="flex gap-2">
		<a href="/app/library" class="btn mt-4 btn-outline">Back to Library</a>
		<a href="/app" class="btn mt-4 btn-primary">Back to Dashboard</a>
	</div>
</div>
