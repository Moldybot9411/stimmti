<script lang="ts" module>
	import { BodyProfileEnum, ColorProfileEnum, FaceProfileEnum, HatProfileEnum } from '$lib/api';
	import type { ClassValue } from 'svelte/elements';

	import body01 from '$lib/assets/Body01.svg';
	import body02 from '$lib/assets/Body02.svg';
	import body03 from '$lib/assets/Body03.svg';
	import body04 from '$lib/assets/Body04.svg';
	import body05 from '$lib/assets/Body05.svg';

	import face01 from '$lib/assets/Face01.svg';
	import face02 from '$lib/assets/Face02.svg';
	import face03 from '$lib/assets/Face03.svg';
	import face04 from '$lib/assets/Face04.svg';
	import face05 from '$lib/assets/Face05.svg';

	import hat01 from '$lib/assets/Hat01.svg';
	import hat02 from '$lib/assets/Hat02.svg';
	import hat03 from '$lib/assets/Hat03.svg';
	import hat04 from '$lib/assets/Hat04.svg';
	import hat05 from '$lib/assets/Hat05.svg';

	export type CustomizableAvatarSettings = {
		body: BodyProfileEnum;
		color: ColorProfileEnum;
		face: FaceProfileEnum;
		hat: HatProfileEnum;
	};
</script>

<script lang="ts">
	type Props = {
		size?: number;
		settings: CustomizableAvatarSettings;
		class?: ClassValue;
		style?: string;
	};

	let { size = 10, settings, class: classes, style }: Props = $props();

	const bodyConfig = {
		[BodyProfileEnum.Body01]: { src: body01, alt: 'Body01' },
		[BodyProfileEnum.Body02]: { src: body02, alt: 'Body02' },
		[BodyProfileEnum.Body03]: { src: body03, alt: 'Body03' },
		[BodyProfileEnum.Body04]: { src: body04, alt: 'Body04' },
		[BodyProfileEnum.Body05]: { src: body05, alt: 'Body05' },
	};

	const faceConfig = {
		[FaceProfileEnum.Face01]: { src: face01, alt: 'Face01' },
		[FaceProfileEnum.Face02]: { src: face02, alt: 'Face02' },
		[FaceProfileEnum.Face03]: { src: face03, alt: 'Face03' },
		[FaceProfileEnum.Face04]: { src: face04, alt: 'Face04' },
		[FaceProfileEnum.Face05]: { src: face05, alt: 'Face05' },
	};

	const hatConfig = {
		[HatProfileEnum.Hat01]: { src: hat01, alt: 'Hat01' },
		[HatProfileEnum.Hat02]: { src: hat02, alt: 'Hat02' },
		[HatProfileEnum.Hat03]: { src: hat03, alt: 'Hat03' },
		[HatProfileEnum.Hat04]: { src: hat04, alt: 'Hat04' },
		[HatProfileEnum.Hat05]: { src: hat05, alt: 'Hat05' },
	};

	let currentBody = $derived(bodyConfig[settings.body]);
	let currentFace = $derived(faceConfig[settings.face]);
	let currentHat = $derived(hatConfig[settings.hat]);
</script>

<div class={['avatar', classes]} {style}>
	<div
		class={[
			'rounded-full bg-base-100',
			settings.color === ColorProfileEnum.Blue && 'bg-blue-600 dark:bg-blue-400',
			settings.color === ColorProfileEnum.Green && 'bg-green-600 dark:bg-green-400',
			settings.color === ColorProfileEnum.Purple && 'bg-purple-600 dark:bg-purple-400',
			settings.color === ColorProfileEnum.Red && 'bg-red-600 dark:bg-red-400',
			settings.color === ColorProfileEnum.Yellow && 'bg-yellow-600 dark:bg-yellow-400',
		]}
		style="width: {size / 4}rem;">
		<div class="relative h-full w-full">
			<img src={currentBody.src} alt={currentBody.alt} class="absolute top-0 left-0" />
			<img src={currentFace.src} alt={currentFace.alt} class="absolute top-0 left-0" />
			<img src={currentHat.src} alt={currentHat.alt} class="absolute top-0 left-0" />
		</div>
	</div>
</div>
