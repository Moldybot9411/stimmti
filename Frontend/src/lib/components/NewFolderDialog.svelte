<script lang="ts">
	import { HeartCrack, LoaderCircle, X } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';
	import { addToast } from './Toast/Toast.svelte';
	import { apiClient } from '$lib/apiClient';

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		onClose?: () => void | Promise<void>;
	};

	let { class: classes, style, ref = $bindable(), onClose }: Props = $props();

	let formRef: HTMLFormElement | null = $state(null);
	let isLoading = $state(false);

	let title = $state('');

	function reset() {
		formRef?.reset();
	}

	async function handleClose() {
		reset();
		await onClose?.();
	}

	function createFolder() {
		isLoading = true;

		apiClient.api
			.v1SurveyFoldersCreate({
				name: title,
			})
			.then((result) => {
				if (result.status === 200) {
                    
					ref?.close();
				}
			})
			.catch((error) => {
				addToast({
					type: 'error',
					label: `Folder Creation ran Into an error: ${error.message}`,
					icon: HeartCrack,
				});
			})
			.finally(() => {
				isLoading = false;
			});
	}
</script>

<dialog class={['modal', classes]} {style} bind:this={ref} onclose={handleClose}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm" disabled={isLoading}>
				<X />
			</button>
		</form>
		<h3 class="text-lg font-bold">New Folder</h3>
		<div class="p-4">
			<form
				class=""
				bind:this={formRef}
				onsubmit={async (e) => {
					e.preventDefault();
					await createFolder();
				}}
				onreset={() => {
					isLoading = false;
				}}>
				<fieldset class="fieldset">
					<legend class="fieldset-legend">Folder Name</legend>
					<input
						bind:value={title}
						type="text"
						class="input w-full"
						placeholder="My Folder"
						required />
				</fieldset>

				<div class="mt-4 flex flex-col gap-2">
					<button
						class="btn btn-secondary"
						type="reset"
						onclick={() => ref?.close()}
						disabled={isLoading}>
						Cancel
					</button>
					<button class="btn btn-primary" type="submit" disabled={isLoading}>
						{#if isLoading}
							<LoaderCircle class="animate-spin" />
						{/if}
						Create Folder
					</button>
				</div>
			</form>
		</div>
	</div>
</dialog>
