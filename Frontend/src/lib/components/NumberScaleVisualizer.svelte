<script lang="ts">
	import type {
		AnswerDisplayDto as WsAnswerDisplayDto,
		NumberResultDto,
	} from '$lib/wsClient/Backend.Dto';
	import type { AnswerDisplayDto as APIAnswerDisplayDto } from '$lib/api';
	import * as d3 from 'd3';

	type Props = {
		answers?: WsAnswerDisplayDto | APIAnswerDisplayDto;
		scaleMinValue?: number;
		scaleMaxValue?: number;
		scaleToMax?: boolean;
	};

	let { answers, scaleMinValue = 1, scaleMaxValue = 10, scaleToMax = true }: Props = $props();

	let incomingData = $derived(answers?.numberResults ?? []);
	let average = $derived.by(() => {
		let sum = 0;
		let numAnswers = 0;

		incomingData.forEach((el) => {
			sum += el.count * el.value;
			numAnswers += el.count;
		});

		if (numAnswers === 0) return scaleMinValue;
		return Math.round((sum / numAnswers) * 10) / 10;
	});
	let averageInPercent = $derived(
		((average - scaleMinValue) / (scaleMaxValue - scaleMinValue)) * 100
	);

	const width = 400;
	const height = 50;

	let containerDiv: HTMLDivElement | undefined = $state();
	let windowHeight = $state(0);
	let topOffset = $state(0);
	$effect(() => {
		if (containerDiv) {
			topOffset = containerDiv.getBoundingClientRect().top;
		}
	});

	let chartData = $derived.by(() => {
		const fullData = [];
		for (let i = scaleMinValue; i <= scaleMaxValue; i++) {
			const match = incomingData.find((a) => a.value === i);
			fullData.push({
				value: i,
				count: match ? match.count : 0,
			});
		}
		return fullData;
	});

	let xScale = $derived(
		d3.scaleLinear().domain([scaleMinValue, scaleMaxValue]).range([0, width])
	);

	let yScale = $derived(
		d3
			.scaleLinear()
			.domain([0, Math.max(...chartData.map((d) => d.count), 1) + 0.05])
			.range([height, 0])
	);

	let areaGenerator = $derived(
		d3
			.area<NumberResultDto>()
			.x((d) => xScale(d.value))
			.y0(height)
			.y1((d) => yScale(d.count))
			.curve(d3.curveBasis)
	);

	let lineGenerator = $derived(
		d3
			.line<NumberResultDto>()
			.x((d) => xScale(d.value))
			.y((d) => yScale(d.count))
			.curve(d3.curveBasis)
	);

	let areaPath = $derived(areaGenerator(chartData) ?? '');
	let linePath = $derived(lineGenerator(chartData) ?? '');
</script>

<svelte:window bind:innerHeight={windowHeight} />
<div
	bind:this={containerDiv}
	style={scaleToMax ? 'height: {windowHeight - topOffset}px;' : ''}
	class="flex w-full items-center justify-center md:max-w-200">
	<div class="flex w-full gap-2">
		<span class="self-end text-lg font-bold">{scaleMinValue}</span>

		<div class="flex w-full flex-col gap-2">
			<svg
				{width}
				{height}
				viewBox="0 0 {width} {height}"
				preserveAspectRatio="none"
				class="w-full">
				<path
					d={areaPath}
					stroke="none"
					class="fill-primary transition-all duration-500 ease-out"
					opacity="0.15" />

				<path
					d={linePath}
					fill="none"
					class="stroke-primary transition-all duration-500 ease-out"
					stroke-width="2" />
			</svg>

			<div class="relative flex h-8 w-full items-center">
				<div
					class={[
						'absolute h-4 w-full rounded-full transition-colors duration-500',
						averageInPercent < 33.33 && 'bg-error',
						averageInPercent >= 33.33 && averageInPercent < 66.66 && 'bg-warning',
						averageInPercent >= 66.66 && 'bg-success',
					]}>
				</div>

				<div
					class={[
						'absolute top-1/2 flex items-center justify-center rounded-full border-4 border-base-200 px-3 py-1 text-sm font-bold transition-all duration-500',
						averageInPercent < 33.33 && 'bg-error text-error-content',
						averageInPercent >= 33.33 &&
							averageInPercent < 66.66 &&
							'bg-warning text-warning-content',
						averageInPercent >= 66.66 && 'bg-success text-success-content',
					]}
					style="left: {averageInPercent}%; transform: translate(-50%, -50%);">
					{average}
				</div>
			</div>
		</div>

		<span class="self-end text-lg font-bold">{scaleMaxValue}</span>
	</div>
</div>
