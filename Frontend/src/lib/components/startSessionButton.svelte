<script lang="ts">
    import { goto } from '$app/navigation';
    import type { GetSurveyResponseDto } from '$lib/api';
    import { apiClient } from '$lib/apiClient';
    import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { Play, HeartCrack } from '@lucide/svelte';
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
                addToast({
                    type: 'error',
                    label: `Session Creation ran Into an error: ${error.message}`,
                    icon: HeartCrack,
                });
            })
            .finally(() => {
                isLoading = false;
            });
    }

</script>
<button class={['btn btn-primary max-w-full btn btn-block ', classes]} {style} on:click={startSession} disabled={isLoading}>
    <Play size={20} /> Start Session
</button>