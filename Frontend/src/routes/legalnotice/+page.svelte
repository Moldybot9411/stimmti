<script lang="ts">
	import { ChevronLeft, ShieldAlert } from '@lucide/svelte';
	import { env } from '$env/dynamic/public';
	import BackButton from '$lib/components/BackButton.svelte';

	const legalName = env.PUBLIC_LEGAL_NAME ?? 'Your organization name';
	const legalStreet = env.PUBLIC_LEGAL_STREET ?? 'Street and house number';
	const legalCity = env.PUBLIC_LEGAL_CITY ?? 'Postal code and city';
	const legalCountry = env.PUBLIC_LEGAL_COUNTRY ?? 'Country';
	const legalEmail = env.PUBLIC_LEGAL_EMAIL ?? 'contact@example.com';
	const legalResponsiblePerson = env.PUBLIC_LEGAL_RESPONSIBLE_PERSON ?? legalName;
	const legalResponsibleAddress =
		env.PUBLIC_LEGAL_RESPONSIBLE_ADDRESS ?? `${legalStreet}, ${legalCity}`;
	const legalInstitution =
		env.PUBLIC_LEGAL_SUPERVISING_INSTITUTION ?? 'Optional supervising institution';
	const legalRepresentative =
		env.PUBLIC_LEGAL_SUPERVISING_REPRESENTATIVE ?? 'Optional representative';

	const hasMissingLegalEnv = [
		env.PUBLIC_LEGAL_NAME,
		env.PUBLIC_LEGAL_STREET,
		env.PUBLIC_LEGAL_CITY,
		env.PUBLIC_LEGAL_COUNTRY,
		env.PUBLIC_LEGAL_EMAIL,
		env.PUBLIC_LEGAL_RESPONSIBLE_PERSON,
		env.PUBLIC_LEGAL_RESPONSIBLE_ADDRESS,
	].some((value) => !value?.trim());

	type Section = {
		title: string;
		paragraphs: string[];
		lines?: { text: string; strong?: boolean }[];
	};

	const content = {
		title: 'Legal Notice',
		subtitle: 'Information according to Section 5 DDG / Section 18 MStV',
		sections: [
			{
				title: 'Operator',
				paragraphs: [],
				lines: [
					{ text: legalName, strong: true },
					{ text: legalStreet },
					{ text: legalCity },
					{ text: legalCountry },
				],
			},
			{
				title: 'Contact',
				paragraphs: [`Email: ${legalEmail}`],
			},
			{
				title: 'Responsible for content',
				paragraphs: ['According to Section 18 (2) MStV:'],
				lines: [
					{ text: `${legalResponsiblePerson}, ${legalResponsibleAddress}`, strong: true },
				],
			},
			{
				title: 'School project context',
				paragraphs: [
					'This website is a non-commercial school project created for educational and testing purposes. It is not operated for profit, and no goods or services are sold or advertised.',
				],
				lines: [
					{ text: 'Supervising institution (optional):', strong: true },
					{ text: legalInstitution },
					{ text: `Represented by: ${legalRepresentative}` },
				],
			},
			{
				title: 'Disclaimer',
				paragraphs: [
					'The content of this website has been created with care, but we cannot guarantee accuracy, completeness, or timeliness. As a school project, this site may not meet the standards of a professional website. Links to external websites are not controlled by us and we assume no liability for their content.',
				],
			},
			{
				title: 'Copyright',
				paragraphs: [
					'Unless otherwise stated, content created by the project author is subject to copyright. Reproduction, distribution, or use outside the scope of this school project requires prior consent.',
				],
			},
		] satisfies Section[],
	};
</script>

<div class="mx-auto w-full md:w-200">
	<BackButton class="mt-4 mb-4 ml-4" />
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
					<div class="text-sm leading-relaxed text-base-content/80">
						{#each section.paragraphs as paragraph}
							<p class="mb-3 last:mb-0">{paragraph}</p>
						{/each}
						{#if section.lines}
							{#each section.lines as line}
								<p class="mb-1 last:mb-0">
									{#if line.strong}
										<strong>{line.text}</strong>
									{:else}
										{line.text}
									{/if}
								</p>
							{/each}
						{/if}
					</div>
				</div>
				{#if i < content.sections.length - 1}
					<div class="divider m-0"></div>
				{/if}
			{/each}
		</div>
	</div>

	{#if hasMissingLegalEnv}
		<div class="alert alert-warning">
			<ShieldAlert size={20} />
			<span class="text-sm">Double-check these legal details before public deployment.</span>
		</div>
	{/if}
</div>
