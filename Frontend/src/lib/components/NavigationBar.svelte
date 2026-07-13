<script lang="ts">
	import {
		ChartPie,
		CircleQuestionMark,
		Cog,
		FileBadge,
		House,
		LibraryBig,
		LogOut,
		User,
	} from '@lucide/svelte';
	import ThemeToggle from './ThemeToggle.svelte';
	import { authStore, logoutUser } from '$lib/authStore.svelte';
	import { apiClient } from '$lib/apiClient';
	import { goto } from '$app/navigation';
	import { page } from '$app/state';
	import UserAvatar from './UserAvatar.svelte';
	import { themeManager } from '$lib/Theme.svelte';

	const logoSrc = $derived(
		themeManager.theme === 'light' ? '/stimmti-logo.svg' : '/stimmti-logo-light.svg'
	);

	let menuTabs = [
		{ label: 'Home', icon: House, href: '/app' },
		{ label: 'Library', icon: LibraryBig, href: '/app/library' },
		{ label: 'Overview', icon: ChartPie, href: '/app/overview' },
	];

	async function logout() {
		await apiClient.api.v1UserLogoutCreate().then((result) => {
			if (result.status === 200) {
				logoutUser();
				goto('/');
			}
		});
	}
</script>

{#snippet profileDropdown()}
	<div class="dropdown dropdown-end">
		<button class="btn bg-base-300 btn-ghost py-6">
			<UserAvatar />

			<span class="hidden max-w-40 truncate md:inline">
				{authStore.user?.username}
			</span>
		</button>

		<ul
			tabindex="-1"
			class="dropdown-content menu z-10 mt-3 w-52 menu-md rounded-box bg-base-100 p-2 shadow">
			<li>
				<a href="/app/profile">
					<User size={16} />
					Profile
				</a>
			</li>
			<li>
				<a href="/app/settings">
					<Cog size={16} />
					Settings</a>
			</li>
			<li>
				<details>
					<summary><FileBadge size={16} /> Legal</summary>
					<ul>
						<li><a href="/privacypolicy">Privacy Policy</a></li>
						<li><a href="/legalnotice">Legal Notice</a></li>
					</ul>
				</details>
			</li>
			<li>
				<button onclick={logout}>
					<LogOut size={16} />
					Logout
				</button>
			</li>
		</ul>
	</div>
{/snippet}

{#snippet utils(small?: boolean)}
	<a href="/help" class="btn btn-circle btn-ghost" aria-label="Help" title="Open Help Page">
		<CircleQuestionMark />
	</a>
	<ThemeToggle />

	{#if !small}
		<div class="divider mx-0 divider-horizontal"></div>
	{/if}
{/snippet}

<div class="navbar bg-base-100 shadow-sm">
	<div class="mx-auto navbar-start flex-col md:flex-row">
		<div class="mr-0 flex md:mr-2">
			<a href="/app" class="btn btn-ghost px-0" aria-label="Go to homepage">
				<img src={logoSrc} alt="Stimmti Logo" class="h-15 w-auto object-contain" />
			</a>

			<div class="flex items-center gap-2 md:hidden">
				{@render utils(true)}
			</div>
		</div>

		<div class="hidden gap-2 md:flex">
			<div role="tablist" class="tabs-box tabs flex-nowrap">
				{#each menuTabs as tab}
					{@const Icon = tab.icon}
					{@const isActive = page.url.pathname.endsWith(tab.href)}

					<a
						role="tab"
						href={tab.href}
						class={[
							'tab flex flex-nowrap items-center gap-1 transition-all',
							isActive && 'tab-active',
						]}>
						<Icon size={16} />
						{tab.label}
					</a>
				{/each}
			</div>
		</div>
	</div>

	<div class="navbar-end hidden md:flex">
		<div class="flex items-center gap-4">
			{@render utils()}

			{@render profileDropdown()}
		</div>
	</div>
</div>
