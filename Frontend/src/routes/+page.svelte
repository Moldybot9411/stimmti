<script>
	import { authStore } from '$lib/authStore.svelte';
	import ThemeToggle from '$lib/components/ThemeToggle.svelte';
	import {
		BookDashed,
		CircleCheck,
		CircleQuestionMark,
		Flashlight,
		MessageSquareQuote,
		NotepadText,
		Presentation,
		Users,
	} from '@lucide/svelte';
	import singlechoice from '$lib/assets/SingleChoice.svg';
	import multiplechoice from '$lib/assets/MultipleChoice.svg';
	import wordcloud from '$lib/assets/WordCloud.svg';
	import numberscale from '$lib/assets/NumberScale.svg';
	import freetext from '$lib/assets/FreeText.svg';
	import { descending } from 'd3';

	const features = [
		{
			color: 'info',
			icon: Flashlight,
			title: 'Structured Data',
			body: 'Answers and Statistics from Anonymous users are displayed in an organized way.',
		},
		{
			color: 'success',
			icon: MessageSquareQuote,
			title: 'Gather Live Feedback',
			body: 'Collect detailed, qualitative feedback after events or initiatives with structured questionnaires.',
		},
		{
			color: 'warning',
			icon: BookDashed,
			title: 'Quick Templates',
			body: 'Choose from our library of handcrafted templates to gauge quick results.',
		},
	];

	const questionTypes = [
		{
			src: singlechoice,
			alt: 'SingleChoice Display',
			title: 'Single Choice',
			description: 'A participant chan choose one of multiple answer options',
		},
		{
			src: multiplechoice,
			alt: 'MultipleChoice Display',
			title: 'Multiple Choice',
			description: 'A participant chan choose multiple answer options',
		},
		{
			src: wordcloud,
			alt: 'WordCloud Display',
			title: 'Word Cloud',
			description: 'Singular words get grouped and displayed in a visual word cloud',
		},
		{
			src: numberscale,
			alt: 'NumberScale Display',
			title: 'Number Scale',
			description: 'Display numerical votes as a distributional graph',
		},
		{
			src: freetext,
			alt: 'FreeText Display',
			title: 'Freetext',
			description: 'Participants submit text providing detailed feedback',
		},
	];
</script>

<div class="navbar bg-base-100 shadow-sm">
	<div class="flex-1">
		<span class="px-2 text-xl font-extrabold">Stimmti</span>
	</div>
	<div class="flex items-center">
		<ThemeToggle />

		<div class="divider mr-0 ml-2 divider-horizontal py-2"></div>

		<ul class="menu menu-horizontal gap-2">
			<li>
				<a class="btn btn-outline btn-neutral" href="/register">Register</a>
			</li>

			<li>
				<a class="btn btn-primary" href="/login">Login</a>
			</li>
		</ul>
	</div>
</div>

<div
	class="mx-auto mt-4 flex max-w-[calc(100%-2rem)] flex-col items-center justify-center gap-8 rounded-lg bg-primary/20 px-4 py-16 text-center shadow-md">
	<div class="flex flex-col items-center gap-3 md:flex-row">
		<h1 class="text-6xl font-extrabold">Open Source</h1>
		<span class="text-rotate w-fit text-6xl text-primary">
			<span class="justify-items-center">
				<span>SURVEYS</span>
				<span>INSIGHTS</span>
				<span>STATISTICS</span>
				<span>FEEDBACK</span>
			</span>
		</span>
	</div>

	<span class="max-w-120 text-center text-base-content/80">
		Community-driven insights for everything you need. Engage your audience with accessible
		polling and feedback tools.
	</span>

	<div class="flex w-full flex-col gap-2 px-4 md:w-fit md:flex-row">
		<a class="btn w-full btn-lg btn-primary md:w-fit" href="/register">Get Started</a>
		<a class="btn w-full btn-outline btn-lg btn-secondary md:w-fit" href="/help">Help</a>
	</div>
</div>

<div
	class="mx-auto mt-4 flex max-w-[calc(100%-2rem)] flex-col gap-4 has-[>*:only-child]:max-w-120 md:flex-row">
	{#if authStore.isLoggedIn}
		<div class="card mx-auto w-full rounded-lg bg-success/20 shadow-md card-md">
			<div class="card-body">
				<h2 class="card-title"><CircleCheck /> You are already logged in</h2>

				<span>Welcome back!</span>

				<div class="card-actions">
					<a class="btn btn-block btn-success" href="/app">To Dashboard</a>
				</div>
			</div>
		</div>
	{/if}

	<div class="card mx-auto w-full rounded-lg bg-accent/20 shadow-md card-md">
		<div class="card-body">
			<h2 class="card-title"><Users /> Join a Room!</h2>

			<span>Have a room code to join? Continue here</span>

			<div class="card-actions">
				<a class="btn btn-block btn-accent" href="/app">Join Now</a>
			</div>
		</div>
	</div>
</div>

<div class="mx-auto mt-16 w-fit px-4 text-center">
	<h2 class="text-2xl font-bold">Core Functionalities</h2>
	<div class="mt-2 max-w-120 text-base-content/80">
		Whether you need a quick consensus or detailed qualitative data, our tools are built to
		facilitate open communication
	</div>
</div>

<div class="mt-8 flex flex-col items-center justify-center gap-4 md:flex-row md:items-stretch">
	{#each features as feature}
		{@const Icon = feature.icon}
		<div
			class="card flex w-full max-w-90 min-w-70 flex-col overflow-hidden bg-base-100 shadow-sm card-md">
			<div
				class={[
					'min-h-2 w-full rounded-b-sm bg-info',
					feature.color === 'success' && 'bg-success',
					feature.color === 'warning' && 'bg-warning',
					feature.color === 'info' && 'bg-info',
				]}>
			</div>

			<div class="card-body flex h-full flex-col justify-between">
				<div>
					<Icon
						class={[
							'rounded-box p-2 text-xl',
							feature.color === 'success' && 'bg-success text-success-content',
							feature.color === 'warning' && 'bg-warning text-warning-content',
							feature.color === 'info' && 'bg-info text-info-content',
						]}
						size={40} />
					<h3 class="mt-4 card-title">{feature.title}</h3>
				</div>

				<span class="mt-2 grow text-sm text-base-content/80">
					{feature.body}
				</span>
			</div>
		</div>
	{/each}
</div>

<div class="mx-auto mt-16 w-fit px-4 text-center">
	<h2 class="text-2xl font-bold">Question Types</h2>
	<div class="mt-2 max-w-120 text-base-content/80">
		Different types to ask your audience what they think.
	</div>
</div>

<div class="flex w-full items-center">
	<div
		class="mx-auto mt-8 carousel w-full carousel-center space-x-4 rounded-box bg-base-300 p-4 shadow-md md:max-w-200">
		{#each questionTypes as questionType}
			<div
				class="bg-brounded-box carousel-item flex max-w-60 flex-col gap-8 rounded-box bg-base-100 p-4 shadow-md md:min-w-120">
				<h3 class="text-center text-xl font-bold">{questionType.title}</h3>

				<span class="text-center text-base-content/80">
					{questionType.description}
				</span>

				<img src={questionType.src} class="mt-auto" alt={questionType.alt} />
			</div>
		{/each}
	</div>
</div>

<div class="mt-16 flex w-full flex-col items-center justify-center gap-8 bg-primary/20 px-4 py-16">
	<div class="text-center">
		<h2 class="text-2xl font-bold">Process</h2>
		<div class="mt-2 max-w-120 text-base-content/80">Three steps to clarity</div>
	</div>

	<ul class="timeline timeline-vertical max-w-120 timeline-snap-icon max-md:timeline-compact">
		<li>
			<div class="timeline-middle">
				<CircleCheck class="text-primary" size={20} />
			</div>
			<div class="timeline-start mb-10 md:text-end">
				<div class="flex flex-row items-center justify-start gap-2 md:flex-row-reverse">
					<span class="font-bold">1</span>
					<span class="text-lg font-bold">Prepare</span>
					<NotepadText size={20} />
				</div>

				<div>Choose a community template or build your own custom session in minutes.</div>
			</div>
			<hr class="bg-primary" />
		</li>
		<li>
			<hr class="bg-primary" />
			<div class="timeline-middle">
				<CircleCheck class="text-primary" size={20} />
			</div>
			<div class="timeline-end md:mb-10">
				<div class="flex items-center justify-start gap-2">
					<span class="font-bold">2</span>
					<span class="text-lg font-bold">Start Session</span>
					<NotepadText size={20} />
				</div>

				<div>
					Launch a live session. Participants join anonymously via a simple link or
					code—no account required.
				</div>
			</div>
			<hr class="bg-primary" />
		</li>
		<li>
			<hr class="bg-primary" />
			<div class="timeline-middle">
				<CircleCheck class="text-primary" size={20} />
			</div>
			<div class="timeline-start mb-10 md:text-end">
				<div class="flex flex-row items-center justify-start gap-2 md:flex-row-reverse">
					<span class="font-bold">3</span>
					<span class="text-lg font-bold">Present Results</span>
					<Presentation size={20} />
				</div>

				<div>
					Watch results roll in live on the presenter's screen. Share insights instantly
					with your community.
				</div>
			</div>
		</li>
	</ul>
</div>

<div
	class="flex w-full flex-col items-center justify-center gap-8 bg-primary px-4 py-16 text-primary-content">
	<div class="text-center">
		<h2 class="text-4xl font-bold">Ready to engage your Community?</h2>
		<div class="mt-2 max-w-120 text-primary-content/80">
			Create an account to gain access to all features. No paywall.
		</div>
	</div>

	<div class="flex w-full flex-col gap-2 px-4 md:w-fit md:flex-row">
		<a class="btn-out btn w-full btn-lg md:w-fit" href="/register">Get Started</a>
		<a
			class="btn w-full border-primary-content btn-outline text-primary-content btn-lg md:w-fit"
			href="/help"
			aria-label="Open Help">
			<CircleQuestionMark />
		</a>
	</div>
</div>
