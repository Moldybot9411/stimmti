import { redirect } from '@sveltejs/kit';
import type { PageLoad } from './$types';
import type { GetFolderResponseDto, GetSurveyResponseDto } from '$lib/api';
import { apiClient } from '$lib/apiClient';

const validViews = ['surveys', 'sessions', 'templates', 'archive'] as const;
type View = (typeof validViews)[number];

export const load: PageLoad = async ({ url }) => {
	const view = url.searchParams.get('view')?.toLowerCase();

	if (!view || !(validViews as readonly string[]).includes(view)) {
		throw redirect(302, `${url.pathname}?view=${validViews[0]}`);
	}

	return {
		folder: await getfolders(),
		surveys: await getsurveys(),
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
