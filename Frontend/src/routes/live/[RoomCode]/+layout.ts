import { SessionConnection } from '$lib/signalr.svelte';
import { redirect } from '@sveltejs/kit';
import type { LayoutLoad } from './$types';

export const load: LayoutLoad = async ({ params }) => {
	let roomCode = params.RoomCode;

	let hub = new SessionConnection();
	await hub.init();

	let joinResult = await hub.joinSession(roomCode);

	if (!joinResult) {
		redirect(301, '/live');
	}

	return {
		hub,
	};
};
