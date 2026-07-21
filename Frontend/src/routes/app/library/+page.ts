import { redirect } from '@sveltejs/kit';
import type { PageLoad } from './$types';
import type { GetFolderResponseDto, GetSurveyResponseDto, PaginatedSessionListDto } from '$lib/api';
import { apiClient } from '$lib/apiClient';

const validViews = ['surveys', 'sessions'] as const;
export type View = (typeof validViews)[number];

export const load: PageLoad = async ({ url }) => {
	const view = url.searchParams.get('view')?.toLowerCase();

	if (!view || !(validViews as readonly string[]).includes(view)) {
		throw redirect(302, `${url.pathname}?view=${validViews[0]}`);
	}

	return {
		folder: getfolders(),
		surveys: getsurveys(),
		sessions: getSessions(),
		currentView: view as View,
	};
};

async function getfolders(): Promise<GetFolderResponseDto[]> {
	try {
		const response = await apiClient.api.v1SurveyFoldersList();
		return response.data ?? [];
	} catch (error) {
		console.error('Error fetching folders:', error);
		return [];
	}
}

async function getsurveys(): Promise<GetSurveyResponseDto[]> {
	try {
		const response = await apiClient.api.v1SurveyList();
		return response.data ?? [];
	} catch (error) {
		console.error('Error fetching surveys:', error);
		return [];
	}
}

async function getSessions(): Promise<PaginatedSessionListDto> {
	try {
		const response = await apiClient.api.v1SessionGetSessionListList({
			pageSize: 15,
			currentPage: 1,
		});
		return response.data;
	} catch (error) {
		console.error('Error fetching sessions:', error);
		return { sessionCount: 0, sessionListInfo: [] };
	}
}
