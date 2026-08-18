import { apiClient } from '$lib/apiClient';
import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ params }) => {
	try {
		const res = await apiClient.api.v1SurveyQuestionsList(params.id);

		return {
			surveyId: params.id,
			questions: res.data,
		};
	} catch (e) {
		console.error(e);
		throw error(404, 'Survey not found or error while loading questions');
	}
};
