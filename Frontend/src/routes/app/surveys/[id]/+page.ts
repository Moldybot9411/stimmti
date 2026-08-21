import { apiClient } from '$lib/apiClient';
import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ params }) => {
	try {
		const surveyData = await apiClient.api.v1SurveyDetail(params.id);
		const questionList = apiClient.api.v1SurveyQuestionsList(params.id);

		return {
			surveyId: params.id,
			survey: surveyData.data,
			streamed: {
				questions: questionList,
			},
		};
	} catch (e) {
		console.error(e);
		throw error(404, 'Survey not found or error while loading questions');
	}
};
