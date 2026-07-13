<script lang="ts">
	import { ChevronLeft, ShieldAlert } from '@lucide/svelte';

	const legalName = import.meta.env.VITE_LEGAL_NAME ?? 'Your organization name';
	const legalStreet = import.meta.env.VITE_LEGAL_STREET ?? 'Street and house number';
	const legalCity = import.meta.env.VITE_LEGAL_CITY ?? 'Postal code and city';
	const legalCountry = import.meta.env.VITE_LEGAL_COUNTRY ?? 'Country';
	const legalEmail = import.meta.env.VITE_LEGAL_EMAIL ?? 'contact@example.com';
	const legalResponsiblePerson = import.meta.env.VITE_LEGAL_RESPONSIBLE_PERSON ?? legalName;
	const legalResponsibleAddress =
		import.meta.env.VITE_LEGAL_RESPONSIBLE_ADDRESS ?? `${legalStreet}, ${legalCity}`;
	const legalInstitution =
		import.meta.env.VITE_LEGAL_SUPERVISING_INSTITUTION ?? 'Optional supervising institution';
	const legalRepresentative =
		import.meta.env.VITE_LEGAL_SUPERVISING_REPRESENTATIVE ?? 'Optional representative';

	const content = {
		title: 'Legal Notice',
		subtitle: 'Information according to Section 5 DDG / Section 18 MStV',
		sections: [
			{
				title: 'Operator',
				body: `<strong>${legalName}</strong><br>${legalStreet}<br>${legalCity}<br>${legalCountry}`,
			},
			{
				title: 'Contact',
				body: `Email: ${legalEmail}`,
			},
			{
				title: 'Responsible for content',
				body: `According to Section 18 (2) MStV:<br><strong>${legalResponsiblePerson}</strong>, ${legalResponsibleAddress}`,
			},
			{
				title: 'School project context',
				body: `This website is a non-commercial school project created for educational and testing purposes. It is not operated for profit, and no goods or services are sold or advertised.<br><br><strong>Supervising institution (optional):</strong><br>${legalInstitution}<br>Represented by: ${legalRepresentative}`,
			},
			{
				title: 'Disclaimer',
				body: 'The content of this website has been created with care, but we cannot guarantee accuracy, completeness, or timeliness. As a school project, this site may not meet the standards of a professional website. Links to external websites are not controlled by us and we assume no liability for their content.',
			},
			{
				title: 'Copyright',
				body: 'Unless otherwise stated, content created by the project author is subject to copyright. Reproduction, distribution, or use outside the scope of this school project requires prior consent.',
			},
		],
	};
</script>

<div class="mx-auto flex w-full items-center justify-between md:w-200">
	<button
		class="btn mt-4 mb-4 ml-4 btn-lg"
		aria-label="Navigate Back"
		onclick={() => history.back()}>
		<ChevronLeft />
	</button>
</div>

<div class="mx-auto mb-8 w-fit text-center">
	<h1 class="text-3xl font-bold">{content.title}</h1>
	<p class="mt-1 text-sm text-base-content/60">{content.subtitle}</p>
</div>

<div class="mx-auto flex w-full flex-col gap-6 p-4 md:w-200">
	<div class="card border border-base-300 bg-base-100 shadow-sm">
		<div class="card-body gap-0 p-0">
			{#each content.sections as section, i}
				<div class="p-6">
					<h2 class="mb-2 badge badge-outline badge-sm font-semibold">{section.title}</h2>
					<p class="text-sm leading-relaxed text-base-content/80">{@html section.body}</p>
				</div>
				{#if i < content.sections.length - 1}
					<div class="divider m-0"></div>
				{/if}
			{/each}
		</div>
	</div>

	<div class="alert alert-warning">
		<ShieldAlert size={20} />
		<span class="text-sm">Double-check these legal details before public deployment.</span>
	</div>
</div>
