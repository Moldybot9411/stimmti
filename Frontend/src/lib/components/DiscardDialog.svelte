<script lang="ts">
	import { LoaderCircle, Trash, X } from '@lucide/svelte';

	type Props = {
		title: string;
		description: string;
		isDeleting?: boolean;
		onDeleteConfirm?: () => void;
		ref?: HTMLDialogElement;
	};

	let { title, description, isDeleting, onDeleteConfirm, ref = $bindable() }: Props = $props();
</script>

<dialog class="modal" bind:this={ref}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm" disabled={isDeleting}>
				<X />
			</button>
		</form>
		<h3 class="mb-4 pr-10 text-lg font-bold">{title}</h3>

		<span>
			{description}
		</span>

		<div class="divider"></div>

		<div class="flex items-center justify-end gap-2">
			<button
				class="btn btn-outline btn-secondary"
				onclick={() => ref?.close()}
				disabled={isDeleting}>
				Cancel
			</button>
			<button class="btn btn-error" onclick={onDeleteConfirm} disabled={isDeleting}>
				{#if isDeleting}
					<LoaderCircle class="animate-spin" />
				{:else}
					<Trash />
				{/if}
				Permanently Delete
			</button>
		</div>
	</div>
</dialog>
