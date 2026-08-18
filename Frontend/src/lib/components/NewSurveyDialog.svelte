<script lang="ts">
	import { goto } from '$app/navigation';
	import { HeartCrack, LoaderCircle, X } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';
	import { addToast } from './Toast/Toast.svelte';
	import { apiClient } from '$lib/apiClient';

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
	};

	let { class: classes, style, ref = $bindable() }: Props = $props();

	let formRef: HTMLFormElement | null = $state(null);
	let isLoading = $state(false);

	let title = $state('');
	let description = $state('');

	function reset() {
		formRef?.reset();
	}

	function handleClose() {
		reset();
	}

	function createSurvey() {
		isLoading = true;

		apiClient.api
			.v1SurveyCreate({
				title,
				description,
			})
			.then((result) => {
				if (result.status === 200) {
					goto(`/app/surveys/${result.data.surveyId}`);
				}
			})
			.catch((error) => {
				addToast({
					type: 'error',
					label: `Survey Creation ran Into an error: ${error.message}`,
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
		<h3 class="text-lg font-bold">New Survey</h3>
		<div class="p-4">
			<form
				class=""
				bind:this={formRef}
				onsubmit={(e) => {
					e.preventDefault();
					createSurvey();
				}}
				onreset={() => {
					isLoading = false;
				}}>
				<fieldset class="fieldset">
					<legend class="fieldset-legend">Survey Title</legend>
					<input
						bind:value={title}
						type="text"
						class="input w-full"
						placeholder="My Survey"
						maxlength={255}
						required />
				</fieldset>

				<fieldset class="fieldset">
					<legend class="fieldset-legend">Survey Description</legend>
					<input
						bind:value={description}
						type="text"
						class="input w-full"
						placeholder="My Description"
						maxlength={2048} />
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
						Create Survey
					</button>
				</div>
			</form>
		</div>
	</div>
</dialog>
