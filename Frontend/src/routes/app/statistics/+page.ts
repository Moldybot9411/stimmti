import { apiClient } from '$lib/apiClient';
import { error } from '@sveltejs/kit';
import type { PageLoad } from './$types';

export const load: PageLoad = async () => {
	try {
		let res = await apiClient.api.v1StatisticsStatisticsList({ pageSize: 15 });

		return {
			statisticsData: res.data,
		};
	} catch (e) {
		console.error(e);

		throw error(501, 'Internal error loading your statistics');
	}
};
