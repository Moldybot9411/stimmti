<script lang="ts">
	import { ChevronLeft, ShieldAlert } from '@lucide/svelte';

	const legalName = import.meta.env.VITE_LEGAL_NAME ?? 'Your organization name';
	const legalStreet = import.meta.env.VITE_LEGAL_STREET ?? 'Street and house number';
	const legalCity = import.meta.env.VITE_LEGAL_CITY ?? 'Postal code and city';
	const legalEmail = import.meta.env.VITE_LEGAL_EMAIL ?? 'contact@example.com';

	const content = {
		title: 'Privacy Policy',
		sections: [
			{
				title: '1. Responsible Entity',
				body: `<p>This project is developed as part of a school assignment and is not intended for commercial use. The responsible entity for data processing is:<br><br><strong>${legalName}</strong><br>${legalStreet}, ${legalCity}<br>${legalEmail}</p>`,
			},
			{
				title: '2. Data Collection and Usage',
				body: `<p>We do not collect personal data unless you explicitly provide it (for example through survey participation). Any data you provide will only be used for the purposes stated at the time of collection. For example:</p>
                    <ul class="list"><li class="list-row">To display and evaluate survey results</li><li class="list-row">To respond to inquiries</li><li class="list-row">To improve this project's functionality</li></ul>
                    <p>No data will be shared with third parties unless required by law.</p>`,
			},
			{
				title: '3. Cookies and Tracking',
				body: `<p>This website does not use analytics or ad tracking tools. Only technically necessary data (for example session or participant state) may be stored to make live surveys work reliably.</p>`,
			},
			{
				title: '4. Data Security',
				body: `<p>We apply appropriate technical and organizational measures to protect your data from unauthorized access, loss, or misuse. As this is a school project, please avoid submitting sensitive personal information.</p>`,
			},
			{
				title: '5. Your Rights',
				body: `<p>Under GDPR, you generally have rights to access, correction, deletion, restriction of processing, data portability, and objection. To exercise these rights, contact us using the email listed below.</p>`,
			},
			{
				title: '6. Contact',
				body: `<p>If you have any questions about this Privacy Policy or data handling, contact us at:<br><br><strong>${legalEmail}</strong></p>`,
			},
			{
				title: '7. Disclaimer',
				body: `<p>This project is for educational purposes and is not intended for commercial use. While we aim to comply with applicable privacy standards, this website is part of a learning exercise.</p>`,
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

<h1 class="mx-auto mb-6 w-fit text-3xl font-bold">{content.title}</h1>

<div class="mx-auto flex w-full flex-col gap-4 p-4 md:w-200">
	<div class="join h-fit w-full join-vertical rounded-box bg-base-100">
		{#each content.sections as section}
			<div class="collapse-arrow collapse join-item border border-base-300">
				<input type="checkbox" />
				<div class="collapse-title font-semibold">{section.title}</div>
				<div class="collapse-content text-sm">
					{@html section.body}
				</div>
			</div>
		{/each}
	</div>
</div>

<div class="alert alert-warning">
	<ShieldAlert size={20} />
	<span class="text-sm">Double-check these legal details before public deployment.</span>
</div>
