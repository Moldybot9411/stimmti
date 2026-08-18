export function scrollIntoViewOnMount(node: HTMLElement, triggervalue: any) {
	const executeScroll = () =>
		setTimeout(() => {
			node.scrollIntoView({
				behavior: 'smooth',
				block: 'start',
			});
		}, 50);

	executeScroll();

	return {
		update(newTriggerValue: any) {
			executeScroll();
		},
		destroy() {},
	};
}
