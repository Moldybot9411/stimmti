<script lang="ts">
	import { goto } from '$app/navigation';
	import { scrollIntoViewOnMount } from '$lib/actions/scrollaction.js';
	import NewSurveyDialog from '$lib/components/NewSurveyDialog.svelte';
	import Pagination from '$lib/components/Pagination.svelte';
	import {
		Archive,
		BadgePlus,
		ChartNoAxesCombined,
		Check,
		ChevronRight,
		Cog,
		Folder,
		Form,
		Play,
		Plus,
		Scroll,
		X,
	} from '@lucide/svelte';

	let { data } = $props();

	let menuTabs = [
		{ label: 'Surveys', icon: Form },
		{ label: 'Sessions', icon: ChartNoAxesCombined },
		{ label: 'Templates', icon: Plus },
		{ label: 'Archive', icon: Archive },
	];

	type Survey = { type: 'survey'; title: string };
	let mockSurveys: (
		| Survey
		| {
				type: 'folder';
				title: string;
				color:
					| 'warning'
					| 'error'
					| 'success'
					| 'primary'
					| 'secondary'
					| 'info'
					| 'accent';
				surveys: Survey[];
		  }
	)[] = [
		{
			type: 'folder',
			title: 'Seminars',
			color: 'error',
			surveys: [
				{ type: 'survey', title: 'NodeJS Seminar Feedback' },
				{ type: 'survey', title: 'C# & ASP.NET Architecture' },
			],
		},
		{
			type: 'folder',
			title: 'Planning',
			color: 'warning',
			surveys: [
				{ type: 'survey', title: 'Milestone 1 (M1) Check-in' },
				{ type: 'survey', title: 'MySQL Performance Tuning' },
				{ type: 'survey', title: 'Mentimeter Alternatives' },
				{ type: 'survey', title: 'Sprint Retrospective' },
			],
		},
		{ type: 'survey', title: 'Docker Deployment Quiz' },
		{ type: 'survey', title: 'Svelte 5 vs. React' },
	];

	let activeViewId = $derived(data.currentView);

	let editingItem = $state<string | null>(null);
	let newSurveyDialogRef: HTMLDialogElement | undefined = $state();
</script>

<div class="flex w-full flex-col gap-4 md:flex-row">
	<ul class="menu h-fit w-full bg-base-100 shadow-sm md:sticky md:top-4 md:flex-1">
		<li>
			<h2 class="menu-title">Library</h2>
			<ul>
				{#each menuTabs as tab}
					<li class={activeViewId === tab.label.toLowerCase() ? 'menu-active' : ''}>
						<a href="?view={tab.label.toLowerCase()}">
							<tab.icon size={16} />
							{tab.label}
						</a>
					</li>
				{/each}
			</ul>
			<button class="btn mt-2 btn-primary" onclick={() => newSurveyDialogRef?.showModal()}>
				<BadgePlus />
				New Survey
			</button>
		</li>
	</ul>

	<div class="card flex-5 bg-base-100 shadow-sm card-md">
		<div class="card-body">
			<h2 class="card-title">
				{activeViewId.charAt(0).toUpperCase() + activeViewId.slice(1)}
			</h2>

			{#if activeViewId === 'surveys'}
				<ul class="menu w-full rounded-box">
					{#each mockSurveys as item}
						{#if item.type === 'folder'}
							<li>
								<details>
									<summary
										class={[
											item.color === 'primary' && 'text-primary',
											item.color === 'secondary' && 'text-secondary',
											item.color === 'accent' && 'text-accent',
											item.color === 'info' && 'text-info',
											item.color === 'success' && 'text-success',
											item.color === 'warning' && 'text-warning',
											item.color === 'error' && 'text-error',
										]}>
										<Folder size={16} />
										{item.title}
									</summary>
									<ul>
										{#each item.surveys as survey}
											<li>
												<button
													onclick={() => (editingItem = survey.title)}
													class={[
														editingItem === survey.title &&
															'bg-base-300',
													]}>
													<div class="flex items-center gap-2">
														<Scroll size={16} />
														{survey.title}
														<ChevronRight size={16} />
													</div>
												</button>
											</li>
										{/each}
									</ul>
								</details>
							</li>
						{:else if item.type === 'survey'}
							<li>
								<button
									onclick={() => (editingItem = item.title)}
									class={[editingItem === item.title && 'bg-base-300']}>
									<div class="flex items-center gap-2">
										<Scroll size={16} />
										{item.title}
										<ChevronRight size={16} />
									</div>
								</button>
							</li>
						{/if}
					{/each}
				</ul>

				<div class="divider"></div>

				<Pagination numPages={5} currentPage={1} />
			{:else if activeViewId === 'sessions'}
				<p>Here will be the Sessions</p>
			{:else if activeViewId === 'templates'}
				<p>Here will be the Templates</p>
			{:else if activeViewId === 'archive'}
				<p>Here will be the Archive</p>
			{/if}
		</div>
	</div>

	{#if editingItem}
		<div
			class="card h-fit flex-3 bg-base-100 shadow-sm card-md"
			use:scrollIntoViewOnMount={editingItem}>
			<div class="card-body">
				<div class="flex justify-between">
					<h2 class="card-title justify-between">
						<Scroll />
						{editingItem}
					</h2>
					<button
						class="btn btn-ghost btn-neutral btn-sm"
						onclick={() => (editingItem = null)}
						aria-label="Close">
						<X size={16} />
					</button>
				</div>

				<div class="p-4">
					Here will be quick settings that will be saved immediatly

					<fieldset
						class="fieldset w-full rounded-box border border-base-300 bg-base-100 p-4">
						<legend class="fieldset-legend">Some Quick Options</legend>
						<label class="label">
							<input type="checkbox" checked class="toggle" />
							Some
						</label>

						<label class="label">
							<input type="checkbox" class="toggle" />
							Other
						</label>
					</fieldset>
				</div>

				<div class="card-actions flex-col">
					<button
						class="btn btn-block btn-outline btn-secondary btn-sm"
						onclick={() => goto(`/app/surveys/${crypto.randomUUID()}`)}
						><Cog size={20} /> Full Settings</button>
					<button class="btn btn-block btn-primary"><Play /> Start Session </button>
				</div>
			</div>
		</div>
	{/if}
</div>

<NewSurveyDialog bind:ref={newSurveyDialogRef} />
