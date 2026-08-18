import { apiClient } from '$lib/apiClient';
import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ params }) => {
	try {
		let res = await apiClient.api.v1SessionSessionList({ sessionId: params.id });

		return {
			questionResults: res.data,
		};
	} catch (e) {
		console.error(e);

		throw error(404, 'Session not found or error while loading');
	}
};
