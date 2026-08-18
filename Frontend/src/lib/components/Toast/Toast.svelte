<script lang="ts" module>
	import { slide } from 'svelte/transition';
	import type { IToastElement } from './IToastElement';

	let toasts: IToastElement[] = $state([]);

	type IToastCreate = Omit<IToastElement, 'id'>;

	export function addToast(item: IToastCreate) {
		const newItem = {
			id: crypto.randomUUID(),
			...item,
		};

		toasts.push(newItem);

		setTimeout(() => {
			toasts = toasts.filter((element) => element.id !== newItem.id);
		}, 3000);
	}

	export function resetToasts() {
		toasts = [];
	}
</script>

<script>
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		class?: ClassValue;
		style?: string;
	};

	let { class: classes, style }: Props = $props();
</script>

<div
	class={[
		'pointer-events-none fixed bottom-0 w-full min-w-100 md:right-2 md:bottom-2 md:w-auto md:max-w-200',
		classes,
	]}
	{style}>
	<div class="flex flex-col">
		{#each toasts as element}
			<div transition:slide class="pt-2">
				<div
					class={[
						'alert flex gap-2 last:mb-0',
						element.type === 'success' && 'alert-success',
						element.type === 'warning' && 'alert-warning',
						element.type === 'error' && 'alert-error',
						element.type === 'info' && 'alert-info',
						!element.type && 'alert-info',
					]}>
					{#if element.icon}
						{@const Icon = element.icon}
						<Icon size={24} class="min-w-6 self-start" />
					{/if}
					{element.label}
				</div>
			</div>
		{/each}
	</div>
</div>
