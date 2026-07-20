<script lang="ts">
	import { apiClient } from '$lib/apiClient';
	import { Eye, Key, LoaderCircle, Mail, User } from '@lucide/svelte';
	import type { ClassValue } from 'svelte/elements';

	type Props = {
		onloginfinished?: () => void;
		class?: ClassValue;
		style?: string;
	};

	let { onloginfinished, class: classes, style }: Props = $props();

	let username = $state('');
	let password = $state('');

	let passwordVisible = $state(false);

	let isLoggingIn = $state(false);

	let wrongLoginCreds = $state(false);

	async function login() {
		isLoggingIn = true;

		await apiClient.api
			.v1UserLoginCreate({
				username,
				password,
			})
			.then((result) => {
				if (result.status === 200) {
					onloginfinished?.();
				}
			})
			.catch(() => {
				wrongLoginCreds = true;
			});

		isLoggingIn = false;
	}
</script>

<div class={['card w-96 bg-base-100 shadow-sm card-lg', classes]} {style}>
	<div class="card-body">
		<h2 class="card-title">Login</h2>

		<form
			onsubmit={(e) => {
				e.preventDefault();
				login();
			}}>
			<label class="validator input mb-2">
				<User class="opacity-50" />
				<input
					type="text"
					placeholder="Username"
					name="username"
					autocomplete="username"
					bind:value={username}
					required />
			</label>

			<div class="join w-full">
				<div class="w-full">
					<label class="validator input join-item mb-1">
						<Key class="opacity-50" />
						<input
							type={passwordVisible ? 'text' : 'password'}
							placeholder="Password"
							bind:value={password}
							name="password"
							autocomplete="current-password"
							required />
					</label>
				</div>

				<button
					type="button"
					class="btn join-item btn-neutral"
					aria-label="Show Password"
					onclick={() => (passwordVisible = !passwordVisible)}>
					<Eye />
				</button>
			</div>

			{#if wrongLoginCreds}
				<div class="inline-grid *:[grid-area:1/1]">
					<div class="status status-error"></div>
				</div>
				Wrong E-Mail or Password
			{/if}

			<div class="mt-4 card-actions flex justify-end">
				<button class="btn btn-primary" type="submit">
					{#if isLoggingIn}
						<LoaderCircle class="animate-spin" />
					{/if}
					Login
				</button>
			</div>
		</form>

		<p>
			Don't have an Account?
			<a class="link link-primary" href="/register">Register</a>
		</p>
	</div>
</div>
