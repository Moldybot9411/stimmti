<script lang="ts">
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		numPages: number;
		currentPage: number;
		onpagechange?: (page: number) => void;
		class?: ClassValue;
		style?: string;
	};

	let {
		numPages = 1,
		currentPage = $bindable(1),
		onpagechange,
		class: classes,
		style,
	}: Props = $props();
</script>

{#snippet pageButton(index: number)}
	<button
		class={['btn join-item btn-sm', currentPage === index + 1 && 'btn-neutral']}
		onclick={() => {
			onpagechange?.(index + 1);
			currentPage = index + 1;
		}}>
		{index + 1}
	</button>
{/snippet}

<div class={['join', classes]} {style}>
	{#if numPages < 7}
		{#each Array(numPages) as _, index}
			{@render pageButton(index)}
		{/each}
	{:else}
		{@render pageButton(0)}

		{#if currentPage > numPages - 3}
			<button class="btn join-item btn-sm">...</button>
			{@render pageButton(numPages - 4)}
			{@render pageButton(numPages - 3)}
			{@render pageButton(numPages - 2)}
		{:else if currentPage < 5}
			{@render pageButton(1)}
			{@render pageButton(2)}
			{@render pageButton(3)}
			{@render pageButton(4)}
			<button class="btn join-item btn-sm">...</button>
		{:else}
			<button class="btn join-item btn-sm">...</button>
			{@render pageButton(currentPage - 2)}
			{@render pageButton(currentPage - 1)}
			{@render pageButton(currentPage)}
			<button class="btn join-item btn-sm">...</button>
		{/if}

		{@render pageButton(numPages - 1)}
	{/if}
</div>
