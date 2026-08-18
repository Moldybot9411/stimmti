<script lang="ts">
	import { goto } from '$app/navigation';
	import { apiClient } from '$lib/apiClient';
	import { authStore, logoutUser } from '$lib/authStore.svelte';
	import { addToast } from '$lib/components/Toast/Toast.svelte';
	import { themeManager } from '$lib/Theme.svelte';
	import { Cog, HeartCrack, Trash, User } from '@lucide/svelte';

	let accountDeleteModal: HTMLDialogElement | null = $state(null);

	let firstAccountDeleteCheck = $state(false);
	let usernameAccountDeletevalue = $state('');

	function deleteAccount() {
		apiClient.api
			.v1UserDeleteUserDelete()
			.then(() => {
				addToast({
					label: 'Account deleted successfully',
					icon: HeartCrack,
					type: 'success',
				});
			})
			.then(() => {
				logoutUser();
				goto('/');
			})
			.catch((e) => {
				console.error(e);
				addToast({
					label: 'Error deleting account',
					icon: HeartCrack,
					type: 'error',
				});
			});
	}
</script>

<div class="card w-full bg-base-100 shadow-sm card-md md:mx-auto md:w-fit md:min-w-200">
	<div class="card-body">
		<h2 class="card-title text-2xl">Settings</h2>

		<div class="divider my-0"></div>

		<div class="tabs-lift tabs">
			<label class="tab flex gap-2">
				<input type="radio" name="settings_tabs" checked />
				<Cog />
				General
			</label>

			<div class="tab-content border-base-300 bg-base-100 p-6">
				<h2 class="mb-2 text-lg font-bold">Theme</h2>

				<div class="flex gap-4">
					<button
						class={[
							'card w-full cursor-pointer p-2',
							themeManager.theme === 'light' && 'border-base-content  card-border',
						]}
						onclick={() => themeManager.setTheme('light')}>
						<div
							class="mockup-window w-full border border-base-300 bg-base-100"
							data-theme="light">
							<div class="grid h-20 grid-cols-2 grid-rows-2 gap-2 p-4">
								<div class="row-span-2 w-full rounded-box bg-primary"></div>
								<div class="w-full rounded-box bg-accent"></div>
								<div class="w-full rounded-box bg-accent"></div>
							</div>
						</div>

						<div class="mt-2 flex flex-row gap-2">
							<input
								type="radio"
								name="theme"
								id="radio_light"
								checked={themeManager.theme === 'light'}
								class="radio radio-sm"
								onchange={() => {
									themeManager.setTheme('light');
								}} />
							<label class="cursor-pointer" for="radio_light">Light</label>
						</div>
					</button>

					<button
						class={[
							'card w-full cursor-pointer p-2',
							themeManager.theme === 'dark' && 'border-base-content card-border',
						]}
						onclick={() => themeManager.setTheme('dark')}>
						<div
							class="mockup-window w-full border border-base-300 bg-base-100"
							data-theme="dark">
							<div class="grid h-20 grid-cols-2 grid-rows-2 gap-2 p-4">
								<div class="row-span-2 w-full rounded-box bg-primary"></div>
								<div class="w-full rounded-box bg-accent"></div>
								<div class="w-full rounded-box bg-accent"></div>
							</div>
						</div>

						<div class="mt-2 flex flex-row gap-2">
							<input
								type="radio"
								name="theme"
								id="radio_dark"
								checked={themeManager.theme === 'dark'}
								class="radio radio-sm"
								onchange={() => {
									themeManager.setTheme('dark');
								}} />
							<label class="cursor-pointer" for="radio_dark">Dark</label>
						</div>
					</button>
				</div>
			</div>

			<label class="tab flex gap-2">
				<input type="radio" name="settings_tabs" />
				<User />
				Account
			</label>
			<div class="tab-content border-base-300 bg-base-100 p-6">
				<button class="btn btn-error" onclick={() => accountDeleteModal?.showModal()}>
					<Trash />
					Delete Account
				</button>
			</div>
		</div>
	</div>
</div>

<dialog class="modal" bind:this={accountDeleteModal}>
	<div class="modal-box">
		<h3 class="text-lg font-bold">Delete Account</h3>
		<p class="pt-4">Are you absolutely sure?</p>
		<p class="font-bold">
			All your data including Surveys and their Sessions will be PERMANENTLY deleted.
		</p>

		{#if firstAccountDeleteCheck}
			<p class="mt-4 mb-2">
				To confirm, please type your username "{authStore.user?.username}" below
			</p>

			<input
				type="text"
				class={[
					'input',
					usernameAccountDeletevalue === authStore.user?.username
						? 'input-success'
						: 'input-error',
				]}
				onkeydown={(e) => {
					if (e.key === 'Enter') {
						e.preventDefault();

						if (usernameAccountDeletevalue === authStore.user?.username) {
							deleteAccount();
						}
					}
				}}
				bind:value={usernameAccountDeletevalue} />
		{/if}

		<div class="modal-action">
			<form method="dialog">
				<button
					class="btn"
					onclick={() => {
						firstAccountDeleteCheck = false;
						usernameAccountDeletevalue = '';
						accountDeleteModal?.close();
					}}>
					Cancel
				</button>
			</form>
			{#if !firstAccountDeleteCheck}
				<button class="btn btn-error" onclick={() => (firstAccountDeleteCheck = true)}>
					<Trash />
					Delete My Account
				</button>
			{:else}
				<button
					class="btn btn-error"
					onclick={deleteAccount}
					disabled={usernameAccountDeletevalue !== authStore.user?.username}>
					<Trash />
					Delete My Account
				</button>
			{/if}
		</div>
	</div>
</dialog>
