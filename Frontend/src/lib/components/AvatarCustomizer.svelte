<script lang="ts">
	import type { ClassValue } from 'svelte/elements';
	import CustomizableAvatar, {
		type CustomizableAvatarSettings,
	} from './CustomizableAvatar.svelte';
	import { BodyProfileEnum, ColorProfileEnum, FaceProfileEnum, HatProfileEnum } from '$lib/api';
	import { ChevronLeft, ChevronRight } from '@lucide/svelte';

	type Props = {
		size?: number;
		settings?: CustomizableAvatarSettings;
		class?: ClassValue;
		style?: string;
	};

	let {
		size = 40,
		settings = $bindable({
			body: BodyProfileEnum.Body01,
			face: FaceProfileEnum.Face01,
			hat: HatProfileEnum.Hat01,
			color: ColorProfileEnum.Blue,
		}),
		class: classes,
		style,
	}: Props = $props();

	function cycleEnum<T>(currentValue: T, enumObject: Record<string, T>, direction: -1 | 1) {
		const values = Object.keys(enumObject) as T[];
		const currentIndex = values.indexOf(currentValue);

		const nextIndex = (currentIndex + direction + values.length) % values.length;

		return values[nextIndex];
	}
</script>

<div class="flex items-center gap-2">
	<div class="flex flex-col">
		<button
			class="btn btn-ghost"
			aria-label="Change Hat"
			onclick={() => (settings.hat = cycleEnum(settings.hat, HatProfileEnum, -1))}>
			<ChevronLeft />
		</button>
		<button
			class="btn btn-ghost"
			aria-label="Change Face"
			onclick={() => (settings.face = cycleEnum(settings.face, FaceProfileEnum, -1))}>
			<ChevronLeft />
		</button>
		<button
			class="btn btn-ghost"
			aria-label="Change Body"
			onclick={() => (settings.body = cycleEnum(settings.body, BodyProfileEnum, -1))}>
			<ChevronLeft />
		</button>
		<button
			class="btn btn-ghost"
			aria-label="Change Color"
			onclick={() => (settings.color = cycleEnum(settings.color, ColorProfileEnum, -1))}>
			<ChevronLeft />
		</button>
	</div>

	<CustomizableAvatar {settings} {size} />

	<div class="flex flex-col">
		<button
			class="btn btn-ghost"
			aria-label="Change Hat"
			onclick={() => (settings.hat = cycleEnum(settings.hat, HatProfileEnum, 1))}>
			<ChevronRight />
		</button>
		<button
			class="btn btn-ghost"
			aria-label="Change Face"
			onclick={() => (settings.face = cycleEnum(settings.face, FaceProfileEnum, 1))}>
			<ChevronRight />
		</button>
		<button
			class="btn btn-ghost"
			aria-label="Change Body"
			onclick={() => (settings.body = cycleEnum(settings.body, BodyProfileEnum, 1))}>
			<ChevronRight />
		</button>
		<button
			class="btn btn-ghost"
			aria-label="Change Color"
			onclick={() => (settings.color = cycleEnum(settings.color, ColorProfileEnum, 1))}>
			<ChevronRight />
		</button>
	</div>
</div>
