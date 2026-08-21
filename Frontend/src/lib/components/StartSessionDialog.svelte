<script lang="ts">
	import type { GetSurveyResponseDto, ProblemDetails } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { LoaderCircle, Play, SquareKanban, X } from '@lucide/svelte';
	import axios from 'axios';
	import type { ClassValue } from 'clsx';

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		survey: GetSurveyResponseDto;
		onCreated?: (roomCode: string) => void;
	};

	let { class: classes, style, ref = $bindable(), survey, onCreated }: Props = $props();

	let isLoading = $state(false);

	let name = $state('');
	let description = $state('');
	$effect(() => {
		name = survey.title!;
	});

	function reset() {
		name = survey.title!;
		description = '';
	}

	function startSession() {
		isLoading = true;
		apiClient.api
			.v1SessionCreateSessionCreate({
				surveyId: survey?.surveyId,
				name,
				description,
			})
			.then((result) => {
				if (result.status === 200) {
					onCreated?.(result.data.roomCode!);
				}
			})
			.catch((error) => {
				if (axios.isAxiosError<ProblemDetails>(error)) {
					const title = error.response?.data.title ?? 'Unknown Error';

					addToast({
						type: 'error',
						label: `Error creating session: ${title}`,
						icon: SquareKanban,
					});
				}
			})
			.finally(() => {
				isLoading = false;
			});
	}
</script>

<dialog class={['modal', classes]} {style} bind:this={ref} onclose={reset}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm" disabled={isLoading}>
				<X />
			</button>
		</form>
		<h3 class="text-lg font-bold">Start Session</h3>
		<div class="p-4">
			<form onsubmit={startSession}>
				<fieldset class="fieldset">
					<legend class="fieldset-legend">Name</legend>
					<input
						type="text"
						class="input w-full"
						required
						bind:value={name}
						placeholder={survey.title}
						maxlength="255" />
				</fieldset>

				<fieldset class="fieldset">
					<legend class="fieldset-legend">Description</legend>
					<input
						type="text"
						class="input w-full"
						bind:value={description}
						placeholder="My Description"
						maxlength="2048" />
				</fieldset>

				<div class="divider"></div>

				<button
					type="submit"
					class={'btn btn-block w-full btn-primary'}
					disabled={isLoading}>
					{#if isLoading}
						<LoaderCircle class="animate-spin" />
					{:else}
						<Play size={20} />
					{/if}
					Start Session
				</button>
			</form>
		</div>
	</div>
</dialog>
