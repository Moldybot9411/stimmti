import type { LucideIcon } from '@lucide/svelte';

export interface IToastElement {
	id: string;
	label: string;
	icon?: LucideIcon;
	type?: 'success' | 'warning' | 'info' | 'error';
}
