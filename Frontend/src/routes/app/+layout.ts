import type { GetOpenSessionsDto } from '$lib/api';
import { apiClient } from '$lib/apiClient';
import type { LayoutLoad } from './$types';

export const load: LayoutLoad = () => {
	return {
		openSessions: getOpenSessions(),
	};
};

async function getOpenSessions(): Promise<GetOpenSessionsDto[]> {
	try {
		const response = await apiClient.api.v1SessionOpenSessionsList();
		return response.data ?? [];
	} catch (error) {
		console.error('Error fetching open surveys:', error);
		return [];
	}
}
