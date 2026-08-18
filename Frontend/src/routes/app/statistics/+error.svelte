<script lang="ts">
	import { page } from '$app/state';
	import Error from '$lib/components/Error.svelte';
</script>

<Error statusCode={page.status} statusMessage={page.error?.message} />
