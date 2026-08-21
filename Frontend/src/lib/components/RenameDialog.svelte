<script lang="ts">
	import { LoaderCircle, TextInitial, X } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		class?: ClassValue;
		style?: string;
		ref?: HTMLDialogElement;
		onRename?: (name: string) => void;
		initialName: string;
	};

	let { class: classes, style, ref = $bindable(), onRename, initialName }: Props = $props();

	let formRef: HTMLFormElement | null = $state(null);
	let name = $state('');
	$effect(() => {
		name = initialName;
	});

	function reset() {
		name = initialName;
	}
</script>

<dialog class={['modal', classes]} {style} bind:this={ref} onclose={reset}>
	<div class="modal-box">
		<form method="dialog">
			<button class="btn absolute top-2 right-2 btn-ghost btn-sm">
				<X />
			</button>
		</form>
		<h3 class="text-lg font-bold">Rename</h3>
		<div class="p-4">
			<form
				bind:this={formRef}
				onsubmit={() => {
					onRename?.(name);
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

				<div class="mt-4 flex flex-col gap-2">
					<button class="btn btn-primary" type="submit"> Apply </button>
				</div>
			</form>
		</div>
	</div>
</dialog>
