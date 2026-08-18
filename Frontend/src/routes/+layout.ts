export const ssr = false;

import { apiClient } from '$lib/apiClient';
import { redirect } from '@sveltejs/kit';
import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async ({ url }) => {
	try {
		const result = await apiClient.api.v1UserMeList();

		if (result.status === 200) {
			return {
				user: result.data,
			};
		}
	} catch (error) {
		console.error('Error:', error);
	}

	if (url.pathname.startsWith('/app')) {
		throw redirect(307, '/login');
	}

	return {
		user: null,
	};
};
