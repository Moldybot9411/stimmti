import { env } from '$env/dynamic/public';
import { Api } from './api';

export const apiClient = new Api({
	baseURL: env.PUBLIC_DOMAIN || 'http://localhost:5202',
	withCredentials: true,
});
