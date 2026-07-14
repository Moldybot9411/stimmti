<script>
	import { authStore } from '$lib/authStore.svelte';
	import {
		BookDashed,
		Check,
		CircleCheck,
		CircleQuestionMark,
		Flashlight,
		MessageSquareQuote,
		NotepadText,
		Presentation,
		Rocket,
	} from '@lucide/svelte';

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
</script>

<div class="navbar bg-base-100 shadow-sm">
	<div class="flex-1">
		<span class="px-2 text-xl font-extrabold">Stimmti</span>
	</div>
	<div class="flex-none">
		<ul class="menu menu-horizontal gap-2">
			<li>
				<a class="btn btn-outline btn-neutral" href="/register">Register</a>
			</li>

			<li>
				{#if authStore.isLoggedIn}
					<a class="btn btn-primary" href="/app">To Dashboard</a>
				{:else}
					<a class="btn btn-primary" href="/login">Login</a>
				{/if}
			</li>
		</ul>
	</div>
</div>

<div class="flex w-full flex-col items-center justify-center gap-8 bg-primary/20 py-16">
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
		Community-driven insights for non-profit projects. Empower your organization with accessible
		polling and feedback tools.
	</span>

	<div class="flex w-full flex-col gap-2 px-4 md:w-fit md:flex-row">
		<a class="btn w-full btn-lg btn-primary md:w-fit" href="/register">Get Started</a>
		<a class="btn w-full btn-outline btn-lg btn-secondary md:w-fit" href="/help">Help</a>
	</div>
</div>

<div class="mx-auto mt-16 w-fit text-center">
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
				class="min-h-2 w-full rounded-b-sm bg-info"
				class:bg-success={feature.color === 'success'}
				class:bg-warning={feature.color === 'warning'}
				class:bg-info={feature.color === 'info'}>
			</div>

			<div class="card-body flex h-full flex-col justify-between">
				<div>
					<Icon class="rounded-box bg-info p-2 text-xl text-info-content" size={40} />
					<h3 class="mt-4 card-title">{feature.title}</h3>
				</div>

				<span class="mt-2 grow text-sm text-base-content/80">
					{feature.body}
				</span>
			</div>
		</div>
	{/each}
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
