import { apiClient } from '$lib/apiClient';

export async function createFeedbackFormTemplate() {
	try {
		let surveyResult = await apiClient.api.v1SurveyCreate({
			title: 'Feedback Form',
			description: 'Collect detailed Feedback after a meeting or an event.',
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
