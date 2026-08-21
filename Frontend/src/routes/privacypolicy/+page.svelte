<script lang="ts">
	import { ChevronLeft, ShieldAlert } from '@lucide/svelte';
	import { env } from '$env/dynamic/public';
	import BackButton from '$lib/components/BackButton.svelte';

	const legalName = env.PUBLIC_LEGAL_NAME ?? 'Your organization name';
	const legalStreet = env.PUBLIC_LEGAL_STREET ?? 'Street and house number';
	const legalCity = env.PUBLIC_LEGAL_CITY ?? 'Postal code and city';
	const legalEmail = env.PUBLIC_LEGAL_EMAIL ?? 'contact@example.com';

	const hasMissingLegalEnv = [
		env.PUBLIC_LEGAL_NAME,
		env.PUBLIC_LEGAL_STREET,
		env.PUBLIC_LEGAL_CITY,
		env.PUBLIC_LEGAL_EMAIL,
	].some((value) => !value?.trim());

	type Section = {
		title: string;
		paragraphs: string[];
		listItems?: string[];
		lines?: { text: string; strong?: boolean }[];
	};

	const content = {
		title: 'Privacy Policy',
		sections: [
			{
				title: '1. Responsible Entity',
				paragraphs: [
					'This project is developed as part of a school assignment and is not intended for commercial use. The responsible entity for data processing is:',
				],
				lines: [
					{ text: legalName, strong: true },
					{ text: `${legalStreet}, ${legalCity}` },
					{ text: legalEmail },
				],
			},
			{
				title: '2. Data Collection and Usage',
				paragraphs: [
					'We do not collect personal data unless you explicitly provide it (for example through survey participation). Any data you provide will only be used for the purposes stated at the time of collection. For example:',
					'No data will be shared with third parties unless required by law.',
				],
				listItems: [
					'To display and evaluate survey results',
					'To respond to inquiries',
					"To improve this project's functionality",
				],
			},
			{
				title: '3. Cookies and Tracking',
				paragraphs: [
					'This website does not use analytics or ad tracking tools. Only technically necessary data (for example session or participant state) may be stored to make live surveys work reliably.',
				],
			},
			{
				title: '4. Data Security',
				paragraphs: [
					'We apply appropriate technical and organizational measures to protect your data from unauthorized access, loss, or misuse. As this is a school project, please avoid submitting sensitive personal information.',
				],
			},
			{
				title: '5. Your Rights',
				paragraphs: [
					'Under GDPR, you generally have rights to access, correction, deletion, restriction of processing, data portability, and objection. To exercise these rights, contact us using the email listed below.',
				],
			},
			{
				title: '6. Contact',
				paragraphs: [
					'If you have any questions about this Privacy Policy or data handling, contact us at:',
				],
				lines: [{ text: legalEmail, strong: true }],
			},
			{
				title: '7. Disclaimer',
				paragraphs: [
					'This project is for educational purposes and is not intended for commercial use. While we aim to comply with applicable privacy standards, this website is part of a learning exercise.',
				],
			},
		] satisfies Section[],
	};
</script>

<div class="mx-auto w-full md:w-200">
	<BackButton class="mt-4 mb-4 ml-4" />
</div>

<h1 class="mx-auto mb-6 w-fit text-3xl font-bold">{content.title}</h1>

<div class="mx-auto flex w-full flex-col gap-4 p-4 md:w-200">
	<div class="join h-fit w-full join-vertical rounded-box bg-base-100">
		{#each content.sections as section}
			<div class="collapse-arrow collapse join-item border border-base-300">
				<input type="checkbox" />
				<div class="collapse-title font-semibold">{section.title}</div>
				<div class="collapse-content text-sm">
					{#each section.paragraphs as paragraph}
						<p class="mb-3 last:mb-0">{paragraph}</p>
					{/each}
					{#if section.listItems}
						<ul class="list mb-3">
							{#each section.listItems as item}
								<li class="list-row">{item}</li>
							{/each}
						</ul>
					{/if}
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
		{/each}
	</div>
</div>

{#if hasMissingLegalEnv}
	<div class="alert alert-warning">
		<ShieldAlert size={20} />
		<span class="text-sm">Double-check these legal details before public deployment.</span>
	</div>
{/if}
