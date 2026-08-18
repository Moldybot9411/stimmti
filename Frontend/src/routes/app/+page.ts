import type { PaginatedSessionListDto } from '$lib/api';
import { apiClient } from '$lib/apiClient';
import type { PageLoad } from './$types';

export const load: PageLoad = () => {
	return {
		recentSessions: getRecentSessions(),
	};
};

async function getRecentSessions(): Promise<PaginatedSessionListDto> {
	try {
		const response = await apiClient.api.v1SessionGetSessionListList({
			pageSize: 8,
			currentPage: 1,
		});
		return response.data;
	} catch (error) {
		console.error('Error fetching sessions:', error);
		return { sessionCount: 0, sessionListInfo: [] };
	}
}
