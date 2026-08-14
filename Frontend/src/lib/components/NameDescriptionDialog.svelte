<script lang="ts">
	import { LoaderCircle, X } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		title: string;
		buttonText: string;
		loading?: boolean;
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		onSubmit?: (name: string, description: string) => void;
		name?: string;
		description?: string;
	};

	let {
		title,
		buttonText,
		loading = $bindable(false),
		class: classes,
		style,
		ref = $bindable(),
		onSubmit,
		name = $bindable(''),
		description = $bindable(''),
	}: Props = $props();

	let formRef: HTMLFormElement | null = $state(null);

	function reset() {
		formRef?.reset();
	}
</script>

<dialog class={['modal', classes]} {style} bind:this={ref} onclose={reset}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm" disabled={loading}>
				<X />
			</button>
		</form>
		<h3 class="text-lg font-bold">{title}</h3>
		<div class="p-4">
			<form
				bind:this={formRef}
				onsubmit={() => {
					onSubmit?.(name, description);
				}}>
				<fieldset class="fieldset">
					<legend class="fieldset-legend">Title</legend>
					<input
						bind:value={name}
						type="text"
						class="input w-full"
						placeholder="My Title"
						maxlength={255}
						required />
				</fieldset>

				<fieldset class="fieldset">
					<legend class="fieldset-legend">Description</legend>
					<input
						bind:value={description}
						type="text"
						class="input w-full"
						placeholder="My Description"
						maxlength={2048} />
				</fieldset>

				<div class="mt-4 flex flex-col gap-2">
					<button class="btn btn-primary" type="submit" disabled={loading}>
						{#if loading}
							<LoaderCircle class="animate-spin" />
						{/if}
						{buttonText}
					</button>
				</div>
			</form>
		</div>
	</div>
</dialog>
