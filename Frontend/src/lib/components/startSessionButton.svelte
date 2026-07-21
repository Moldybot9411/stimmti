<script lang="ts">
	import { goto } from '$app/navigation';
	import type { GetSurveyResponseDto, ProblemDetails } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { Play, SquareKanban } from '@lucide/svelte';
	import axios from 'axios';
	import type { ClassValue } from 'clsx';

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		survey: GetSurveyResponseDto;
	};

	let { class: classes, style, ref = $bindable(), survey }: Props = $props();

	let isLoading = $state(false);

	function startSession() {
		isLoading = true;
		apiClient.api
			.v1SessionCreateSessionCreate({
				surveyId: survey?.surveyId,
				name: survey?.title,
			})
			.then((result) => {
				if (result.status === 200) {
					goto(`/live/${result.data.roomCode}`);
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

<button
	class={['btn btn btn-block max-w-full btn-primary ', classes]}
	{style}
	on:click={startSession}
	disabled={isLoading}>
	<Play size={20} /> Start Session
</button>
