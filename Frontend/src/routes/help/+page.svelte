<script lang="ts">
	import {
		Bug,
		ChevronLeft,
		HatGlasses,
		Key,
		UserPen,
		Vote,
		type LucideIcon,
	} from '@lucide/svelte';

	let tabs: 'participant' | 'presenter' | 'technical' = $state('participant');
</script>

{#snippet tile(icon: LucideIcon, title: string, description: string)}
	{@const Icon = icon}

	<div class="collapse-arrow collapse join-item border border-base-300">
		<input type="radio" name="my-accordion-4" />
		<div class="collapse-title flex flex-row gap-2 font-semibold"><Icon /> {title}</div>
		<div class="collapse-content text-sm">
			{description}
		</div>
	</div>
{/snippet}

<button class="btn mt-4 mb-4 ml-4 btn-lg" aria-label="Navigate Back" onclick={() => history.back()}>
	<ChevronLeft />
</button>

<h1 class="mx-auto mb-6 w-fit text-3xl font-bold">Frequently asked questions</h1>

<div role="tablist" class="tabs-box mx-auto tabs w-fit">
	<button
		role="tab"
		onclick={() => (tabs = 'participant')}
		class={['tab', tabs === 'participant' && 'tab-active']}>Participant</button>
	<button
		role="tab"
		onclick={() => (tabs = 'presenter')}
		class={['tab', tabs === 'presenter' && 'tab-active']}>Presenter</button>
	<button
		onclick={() => (tabs = 'technical')}
		role="tab"
		class={['tab', tabs === 'technical' && 'tab-active']}>Technical</button>
</div>

<div class="mx-auto flex w-full flex-col gap-4 p-4 md:w-200">
	<div class="join join-vertical h-fit w-full rounded-box bg-base-100">
		{#if tabs === 'participant'}
			{@render tile(
				HatGlasses,
				'Does a Presenter know who I am?',
				'No. Even if you have an account you will participate with no data linked to your account.'
			)}
			{@render tile(
				Vote,
				'How do I join a session?',
				"Click the 'Join Session' button on the landing page. The session code will be provided by the presenter."
			)}
		{:else if tabs === 'presenter'}
			{@render tile(
				Key,
				'I forgot my password. What should I do?',
				"There's nothing we can do. We don't have an E-Mail service so there's no way to reset your password :("
			)}
			{@render tile(
				UserPen,
				'How do I update my profile information?',
				"When logged in, click your profile image in the top right. From there, click the \'Profile\' button. Follow further instructions there."
			)}
		{:else if tabs === 'technical'}
			{@render tile(
				Bug,
				'What should I do when an error occurs?',
				'Fix the error by doing stuff!'
			)}
		{/if}
	</div>
</div>
