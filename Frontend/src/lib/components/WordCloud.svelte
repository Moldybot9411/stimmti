<script lang="ts">
	import type { AnswerDisplayDto as WsAnswerDisplayDto } from '$lib/wsClient/Backend.Dto';
	import type { AnswerDisplayDto as APIAnswerDisplayDto } from '$lib/api';
	import * as d3 from 'd3';

	type Props = {
		answers?: WsAnswerDisplayDto | APIAnswerDisplayDto;
	};

	let { answers }: Props = $props();

	interface WordNode extends d3.SimulationNodeDatum {
		text: string;
		count: number;
		size: number;
		r: number;
	}

	let incomingData = $derived(answers?.wordCloudResults ?? []);

	let width = 800;
	let height = 500;

	let renderNodes = $state<WordNode[]>([]);

	let simulation: d3.Simulation<WordNode, undefined>;
	let colorScale = d3.scaleOrdinal(d3.schemeCategory10);

	$effect(() => {
		const d3Nodes: WordNode[] = incomingData.map((word) => {
			const size = word.count * 8 + 12;

			const radius = Math.max(size, word.text.length * size * 0.25);

			const existing = renderNodes.find((n) => n.text === word.text);

			return existing
				? { ...existing, count: word.count, size, r: radius }
				: { ...word, size, r: radius, x: width / 2, y: height / 2 };
		});

		if (!simulation) {
			simulation = d3
				.forceSimulation<WordNode>(d3Nodes)
				.force('center', d3.forceCenter(width / 2, height / 2).strength(0.05))
				.force(
					'collide',
					d3
						.forceCollide<WordNode>()
						.radius((d) => d.r + 2)
						.iterations(3)
				)
				.on('tick', () => {
					renderNodes = [...simulation.nodes()];
				});
		} else {
			simulation.nodes(d3Nodes);
			simulation.alpha(0.3).restart();
		}
	});
</script>

<svg viewBox="0 0 800 500" class="h-auto w-full max-w-200">
	{#each renderNodes as node (node.text)}
		<text
			x={node.x}
			y={node.y}
			font-size="{node.size}px"
			fill={colorScale(node.text)}
			text-anchor="middle"
			dominant-baseline="central">
			{node.text}
		</text>
	{/each}
</svg>
