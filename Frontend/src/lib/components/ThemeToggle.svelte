<script lang="ts">
	import { themeManager } from '$lib/Theme.svelte';
	import type { ClassValue } from 'svelte/elements';
	import { Moon, Sun } from '@lucide/svelte';

	type Props = {
		class?: ClassValue;
		style?: string;
	};

	let { class: classes, style, ...others }: Props = $props();
</script>

<label class={['toggle text-base-content', classes]} {style} {...others}>
	<input
		type="checkbox"
		class="theme-controller"
		aria-label="Switch Theme"
		checked={themeManager.theme === 'dark'}
		onchange={() => themeManager.toggle()} />

	<Sun size={16} />

	<Moon size={16} />
</label>
