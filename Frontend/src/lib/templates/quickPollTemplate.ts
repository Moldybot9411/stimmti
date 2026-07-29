import { apiClient } from '$lib/apiClient';

export async function createQuickPollTemplate() {
	try {
		let surveyResult = await apiClient.api.v1SurveyCreate({
			title: 'Quick Poll',
			description: 'A simple single-question poll to gauge immediate reactions.',
		});

		let surveyId = surveyResult.data.surveyId;

		console.debug(
			'Waiting for the question endpoint. Question would be created for Survey',
			surveyId
		);

		return surveyId;
	} catch (error) {
		console.error(error);

		throw error;
	}
}
