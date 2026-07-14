<script lang="ts">
	import { apiClient } from '$lib/apiClient';
	import Pagination from '$lib/components/Pagination.svelte';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { ChartNoAxesCombined, CircleX, Scroll, User } from '@lucide/svelte';

	let { data } = $props();
	let statData = $derived(data.statisticsData);

	const pageSize = 15;

	let surveyPage = $state(1);
	let sessionPage = $state(1);

	let loading = $state(false);

	$effect(() => {
		surveyPage;
		sessionPage;

		loading = true;
		apiClient.api
			.v1StatisticsStatisticsList({
				pageSize,
				currentSurveyPage: surveyPage,
				currentSessionPage: sessionPage,
			})
			.then((res) => {
				if (res.status == 200) {
					statData = res.data;
				}
			})
			.catch(() => {
				addToast({ label: 'Error fetching statistics data', type: 'error', icon: CircleX });
			})
			.finally(() => {
				loading = false;
			});
	});
</script>

<div class="mb-4 flex w-full justify-center">
	<div class="stats w-full stats-vertical bg-base-100 shadow md:w-fit md:stats-horizontal">
		<div class="stat text-primary">
			<div class="stat-figure text-secondary">
				<Scroll size={28} />
			</div>
			<div class="stat-title">Surveys</div>
			<div class="stat-value">{statData.surveyCount}</div>
			<div class="stat-desc">↗︎{statData.surveyDelta} in the last 30 days</div>
		</div>

		<div class="stat text-accent">
			<div class="stat-figure text-secondary">
				<ChartNoAxesCombined size={28} />
			</div>
			<div class="stat-title">Sessions</div>
			<div class="stat-value">{statData.sessionCount}</div>
			<div class="stat-desc">↗︎{statData.sessionDelta} in the last 30 days</div>
		</div>

		<div class="stat text-warning">
			<div class="stat-figure text-secondary">
				<User size={28} />
			</div>
			<div class="stat-title">Participants</div>
			<div class="stat-value">{statData.participantCount}</div>
			<div class="stat-desc">↗︎{statData.participantDelta} in the last 30 days</div>
		</div>
	</div>
</div>

<div class="tabs-lift tabs">
	<input type="radio" name="my_tabs_3" class="tab" aria-label="Most used Surveys" checked />
	<div class="tab-content border-base-300 bg-base-100 p-6">
		<div class="mb-2 overflow-x-auto">
			<table class="table">
				<thead>
					<tr>
						<th></th>
						<th>Survey Name</th>
						<th># Sessions</th>
					</tr>
				</thead>
				<tbody>
					{#if loading}
						{#each new Array(pageSize)}
							<tr>
								<th><div class="h-5.25 w-full skeleton"></div></th>
								<td><div class="h-5.25 w-full skeleton"></div></td>
								<td><div class="h-5.25 w-full skeleton"></div></td>
							</tr>
						{/each}
					{:else}
						{#each statData.surveyStatistics as survey, index}
							<tr>
								<th>{index + 1 + (surveyPage - 1) * pageSize}</th>
								<td>{survey.name}</td>
								<td>{survey.numSessions}</td>
							</tr>
						{/each}
					{/if}
				</tbody>
			</table>
		</div>

		{#if statData.surveyCount > pageSize}
			<div class="divider"></div>

			<Pagination
				numPages={Math.ceil(statData.surveyCount / pageSize)}
				bind:currentPage={surveyPage} />
		{/if}
	</div>

	<input type="radio" name="my_tabs_3" class="tab" aria-label="Participants in Sessions" />
	<div class="tab-content border-base-300 bg-base-100 p-6">
		<div class="mb-2 overflow-x-auto">
			<table class="table">
				<thead>
					<tr>
						<th></th>
						<th>Session Name</th>
						<th># Participants</th>
						<th>Date</th>
					</tr>
				</thead>
				<tbody>
					{#if loading}
						{#each new Array(pageSize)}
							<tr>
								<th><div class="h-5.25 w-full skeleton"></div></th>
								<td><div class="h-5.25 w-full skeleton"></div></td>
								<td><div class="h-5.25 w-full skeleton"></div></td>
								<td><div class="h-5.25 w-full skeleton"></div></td>
							</tr>
						{/each}
					{:else}
						{#each statData.sessionStatistics as session, index}
							<tr>
								<th>{index + 1 + (sessionPage - 1) * pageSize}</th>
								<td>{session.name}</td>
								<td>{session.participantCount}</td>
								<td
									>{new Date(session.openedAt).toLocaleDateString('en-US', {
										year: 'numeric',
										month: 'long',
										day: '2-digit',
									})}</td>
							</tr>
						{/each}
					{/if}
				</tbody>
			</table>
		</div>

		{#if statData.sessionCount > pageSize}
			<div class="divider"></div>

			<Pagination
				numPages={Math.ceil(statData.sessionCount / pageSize)}
				bind:currentPage={sessionPage} />
		{/if}
	</div>
</div>
