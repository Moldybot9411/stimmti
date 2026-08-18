<script lang="ts">
	import type { ValidationProblemDetails } from '$lib/api';
	import { apiClient } from '$lib/apiClient';
	import { Eye, Key, LoaderCircle, Mail, User } from '@lucide/svelte';
	import axios from 'axios';
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		onregisterfinish?: () => void;
		class?: ClassValue;
		style?: string;
	};

	let { onregisterfinish, class: classes, style }: Props = $props();

	const steps = ['Username', 'Password'];
	const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

	let stage = $state(0);
	let isAtFirstStep = $derived(stage === 0);
	let isAtLastStep = $derived(stage === steps.length - 1);

	let formRef = $state<HTMLFormElement>();

	let email = $state('');
	let username = $state('');
	let password = $state('');
	let passwordRep = $state('');

	let usernameError = $state<string | null>(null);
	let usernameSuccess = $state<string | null>(null);
	let isCheckingUsername = $state(false);

	let usernameTimeout: ReturnType<typeof setTimeout> | undefined;

	let passwordVisible = $state(false);

	let isRegistering = $state(false);

	function handleUsernameInput() {
		clearTimeout(usernameTimeout);

		usernameError = null;
		usernameSuccess = null;

		if (!username || !username.trim()) {
			usernameError = 'Please enter a Username';
			isCheckingUsername = false;
			return;
		}

		isCheckingUsername = true;

		usernameTimeout = setTimeout(async () => {
			try {
				const result = await apiClient.api.v1UserCheckUsernameList({ Username: username });

				if (!result.data.isAvailable) {
					usernameError = result.data.message ?? 'Username already taken';
				} else {
					usernameSuccess = result.data.message ?? 'Username is available';
				}
			} catch (e: unknown) {
				if (axios.isAxiosError(e)) {
					const serverError = e.response?.data as ValidationProblemDetails;

					const validationError = serverError?.errors?.['Username']?.[0];

					if (validationError) {
						usernameError = validationError;
					} else {
						usernameError = 'An error occured while checking the username';
					}
				} else {
					usernameError = 'An error occurred while checking the username';
				}
			} finally {
				isCheckingUsername = false;
			}
		}, 500);
	}

	let isCurrentStepValid = $derived.by(() => {
		if (stage === 0)
			return (
				username.length > 0 && checkForm() && usernameError === null && !isCheckingUsername
			);
		if (stage === 1)
			return password.length > 0 && passwordRegex.test(password) && password === passwordRep;

		return false;
	});

	function checkForm(): boolean {
		if (!formRef) return false;

		return formRef.checkValidity();
	}

	function next() {
		if (stage < steps.length - 1 && isCurrentStepValid) {
			stage++;
		}
	}

	function prev() {
		if (stage > 0) {
			stage--;
		}
	}

	async function submitForm() {
		isRegistering = true;

		await apiClient.api
			.v1UserRegisterCreate({
				email,
				username,
				password,
			})
			.then((result) => {
				if (result.status === 200) {
					onregisterfinish?.();
				}
			})
			.catch(() => {});

		isRegistering = false;
	}
</script>

<div class={['card w-96 bg-base-100 shadow-sm card-lg', classes]} {style}>
	<div class="card-body">
		<h2 class="card-title">Register</h2>

		<ul class="steps mb-4">
			{#each steps as step, index}
				<li class={['step', stage >= index && 'step-primary']}>
					{index === stage ? step : ''}
				</li>
			{/each}
		</ul>

		<form
			onsubmit={(e) => {
				e.preventDefault();
				if (isAtLastStep) {
					submitForm();
				} else {
					next();
				}
			}}
			bind:this={formRef}>
			{#if stage === 0}
				<label class={['validator input mb-2', usernameError && 'input-error']}>
					<User class="opacity-50" />
					<input
						type="text"
						placeholder="Username"
						bind:value={username}
						oninput={handleUsernameInput}
						maxlength={20}
						required />
				</label>

				{#if isCheckingUsername}
					<LoaderCircle class="animate-spin" />
				{:else if usernameError}
					<div class="inline-grid *:[grid-area:1/1]">
						<div class="status status-error"></div>
					</div>
					{usernameError}
				{:else if usernameSuccess}
					<div class="inline-grid *:[grid-area:1/1]">
						<div class="status status-success"></div>
					</div>
					{usernameSuccess}
				{/if}
			{:else if stage === 1}
				<div class="join w-full">
					<div class="w-full">
						<label class="validator input join-item mb-1">
							<Key class="opacity-50" />
							<input
								type={passwordVisible ? 'text' : 'password'}
								placeholder="Password"
								minlength="8"
								pattern={'^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$'}
								title="Must be more than 8 characters, including number, lowercase letter, uppercase letter"
								maxlength={64}
								bind:value={password}
								required />
						</label>
						<p class="validator-hint mb-2 hidden">
							Must be more than 8 characters, including
							<br />At least one number <br />At least one lowercase letter <br />At
							least one uppercase letter
						</p>
					</div>

					<button
						type="button"
						class="btn join-item btn-neutral"
						aria-label="Show Password"
						onclick={() => (passwordVisible = !passwordVisible)}>
						<Eye />
					</button>
				</div>

				<label
					class={[
						'validator input',
						passwordRep && password !== passwordRep && 'input-error',
					]}>
					<Key class="opacity-50" />
					<input
						type="password"
						placeholder="Repeat Password"
						bind:value={passwordRep}
						required />
				</label>
			{/if}

			<div class="mt-4 flex justify-end gap-2">
				{#if !isAtFirstStep}
					<button type="button" class="btn" onclick={prev}>Previous</button>
				{/if}

				{#if !isAtLastStep}
					<button
						class="btn btn-primary"
						type="submit"
						onclick={next}
						disabled={!isCurrentStepValid}>
						Next
					</button>
				{/if}

				{#if isAtLastStep}
					<button
						type="submit"
						class="btn btn-primary"
						onclick={submitForm}
						disabled={!isCurrentStepValid || isRegistering}>
						{#if isRegistering}
							<LoaderCircle class="animate-spin" />
						{/if}
						Register
					</button>
				{/if}
			</div>
		</form>

		<div class="mt-2">
			<p>
				Already have an Account?
				<a class="link link-primary" href="/login">Login</a>
			</p>
		</div>
	</div>
</div>
