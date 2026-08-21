<script lang="ts">
	import BackButton from '$lib/components/BackButton.svelte';
	import {
		Bug,
		ChevronLeft,
		HatGlasses,
		Key,
		UserPen,
		Vote,
		type LucideIcon,
	} from '@lucide/svelte';

	type Tab = 'general' | 'participant' | 'creator' | 'privacy';
	type FaqItem = {
		icon: LucideIcon;
		title: string;
		description: string;
		linkText?: string;
		linkHref?: string;
	};

	let tabs: Tab = $state('general');

	const bugReportUrl =
		'https://github.com/Moldybot9411/stimmti/issues/new?template=bug_report.yml';
	const feedbackUrl = 'https://github.com/Moldybot9411/stimmti/issues/new?template=feedback.yml';

	const faq: Record<Tab, FaqItem[]> = {
		general: [
			{
				icon: Vote,
				title: 'What is this survey app?',
				description:
					'A web platform where users can create surveys, run them, and review results directly in the browser without installing software.',
			},
			{
				icon: HatGlasses,
				title: 'Do I need to install anything?',
				description:
					'No. The app runs fully in your browser. You only need a modern browser (Chrome, Firefox, or Edge) and an internet connection.',
			},
			{
				icon: Vote,
				title: 'Which devices are supported?',
				description:
					'Any device with internet access and a modern browser: Desktop PC, Laptop, Tablet, or Smartphone.',
			},
			{
				icon: Key,
				title: 'Is the app free to use?',
				description:
					'Yes. The app is completely free to use and contains no advertisements.',
			},
			{
				icon: UserPen,
				title: 'Where can I get help if my question is not listed here?',
				description:
					'Participants should contact the person who shared the survey link or QR code. Survey creators should use the designated support channel of their organization/project.',
			},
		],
		participant: [
			{
				icon: HatGlasses,
				title: 'Do I need an account to participate in a survey?',
				description: 'No. Participation is anonymous and does not require registration.',
			},
			{
				icon: Vote,
				title: 'How do I join a survey?',
				description:
					'Use the link you received or scan the QR code. This takes you directly to the survey.',
			},
			{
				icon: Key,
				title: 'Are my answers really anonymous?',
				description:
					'Yes. Participation is fully anonymous. Personal data such as your name or email address is not collected, and survey creators cannot require it.',
			},
			{
				icon: UserPen,
				title: 'Can I pause a survey and continue later?',
				description:
					'This depends on the specific survey setup. In most cases, surveys should be completed in one session.',
			},
			{
				icon: Vote,
				title: 'Can I change my answers after submitting?',
				description: 'No. After submission, answers can no longer be edited.',
			},
			{
				icon: Bug,
				title: 'What if the link or QR code does not work?',
				description:
					'Check whether the full link was copied correctly or the QR code was scanned properly. If it still fails, contact the person who invited you.',
			},
			{
				icon: Key,
				title: 'Do I have to answer every question?',
				description:
					'No. Answering is not mandatory. Just wait until the presenter continues to the next question.',
			},
		],
		creator: [
			{
				icon: UserPen,
				title: 'How do I create an account?',
				description:
					'Use the "Register" button on the landing page and choose a unique username and a password. An email address is not required.',
			},
			{
				icon: Vote,
				title: 'How do I create a new survey?',
				description:
					'Use "Create New Survey" in the dashboard\'s "Home" or "Library" tab and configure title, questions, and answer options.',
			},
			{
				icon: Key,
				title: 'Which question types are available?',
				description:
					'Common types include single choice, multiple choice, number scale, word cloud, and free text. We welcome feedback and suggestions for additional question types.',
			},
			{
				icon: Vote,
				title: 'How do I start a survey session?',
				description:
					'Go to your dashboard\'s "Library" tab, choose a survey, and click "Start Session". While the session is active, the link and QR code remain active and participants can submit their answers.',
			},
			{
				icon: UserPen,
				title: 'Do I need to train participants first?',
				description:
					'Usually no. Participants typically only need the link or QR code. Additional guidance is optional and up to you.',
			},
			{
				icon: Key,
				title: 'Can I edit a survey after a session?',
				description:
					'Yes. You can edit questions after a session. No data from previous sessions of that survey is lost. However, once questions are changed, results from different sessions are no longer directly comparable.',
			},
			{
				icon: Vote,
				title: 'How can I see session results?',
				description:
					'Under your dashboard\'s "Library" tab, click on "Sessions". Select the session you want to review and a dedicated page will display all data collected for that session.',
			},
			{
				icon: UserPen,
				title: 'Can I export results?',
				description: 'Not yet. Export functionality is planned for a future update.',
			},
			{
				icon: Key,
				title: 'Can I close a survey early?',
				description:
					'Yes. Return to your dashboard, where a notification will appear for any open sessions. You can close a running session at any time using the "Fix" button, though this is generally not recommended.',
			},
			{
				icon: Vote,
				title: 'Where can I find all surveys I created?',
				description: 'In the dashboard under the "Library" tab.',
			},
		],
		privacy: [
			{
				icon: Key,
				title: 'Is my data shared with third parties?',
				description: 'Please see our Privacy Policy for full details.',
				linkText: 'Open Privacy Policy',
				linkHref: '/privacypolicy',
			},
			{
				icon: Bug,
				title: 'How secure is my survey creator account?',
				description:
					'Account access is protected by your personal password. Use a strong, unique password and do not share your credentials.',
			},
		],
	};
</script>

{#snippet tile(
	icon: LucideIcon,
	title: string,
	description: string,
	linkText?: string,
	linkHref?: string
)}
	{@const Icon = icon}

	<div class="collapse-arrow collapse join-item border border-base-300">
		<input type="radio" name="my-accordion-4" />
		<div class="collapse-title flex flex-row gap-2 font-semibold"><Icon /> {title}</div>
		<div class="collapse-content text-sm">
			<p>{description}</p>
			{#if linkText && linkHref}
				<a class="mt-2 inline-block link link-primary" href={linkHref}>{linkText}</a>
			{/if}
		</div>
	</div>
{/snippet}

<div class="mx-auto w-full md:max-w-200">
	<BackButton class="mt-4 mb-4 ml-4" />
</div>

<h1 class="mx-auto mb-6 w-fit px-4 text-center text-3xl font-bold text-balance">
	Frequently asked questions
</h1>

<div class="mx-auto flex w-full flex-col gap-4 p-4 md:w-200">
	<div role="tablist" class="tabs-box mx-auto tabs w-full flex-col md:w-fit md:flex-row">
		<button
			role="tab"
			onclick={() => (tabs = 'general')}
			class={['tab', tabs === 'general' && 'tab-active']}>General</button>
		<button
			role="tab"
			onclick={() => (tabs = 'participant')}
			class={['tab', tabs === 'participant' && 'tab-active']}>Participants</button>
		<button
			onclick={() => (tabs = 'creator')}
			role="tab"
			class={['tab', tabs === 'creator' && 'tab-active']}>Survey Creators</button>
		<button
			onclick={() => (tabs = 'privacy')}
			role="tab"
			class={['tab', tabs === 'privacy' && 'tab-active']}>Privacy & Security</button>
	</div>

	<div class="join h-fit w-full join-vertical rounded-box bg-base-100">
		{#each faq[tabs] as item (item.title)}
			{@render tile(item.icon, item.title, item.description, item.linkText, item.linkHref)}
		{/each}
	</div>

	<div class="card border border-base-300 bg-base-100 shadow-sm">
		<div class="card-body">
			<h2 class="card-title">Feedback & Bug Reports</h2>
			<p class="text-sm text-base-content/80">
				Found a problem or have an idea? You can report bugs and share feature wishes
				directly via GitHub Issues.
			</p>
			<div class="mt-2 flex flex-wrap gap-2">
				<a
					class="btn btn-outline btn-error btn-sm"
					href={bugReportUrl}
					target="_blank"
					rel="noopener noreferrer">Report a Bug</a>
				<a
					class="btn btn-outline btn-primary btn-sm"
					href={feedbackUrl}
					target="_blank"
					rel="noopener noreferrer">Share Feedback</a>
			</div>
			<p class="mt-2 text-xs text-base-content/60">
				Note: Submitting issues requires a GitHub account.
			</p>
		</div>
	</div>
</div>
