import type { UserAuthDto } from './api';

type AuthData = {
	isLoggedIn: boolean;
	user: UserAuthDto | null;
};

export const authStore = $state<AuthData>({
	isLoggedIn: false,
	user: null,
});

export function logoutUser() {
	authStore.isLoggedIn = false;
	authStore.user = null;
}

export function loginUser(userData: UserAuthDto) {
	authStore.isLoggedIn = true;
	authStore.user = userData;
}
