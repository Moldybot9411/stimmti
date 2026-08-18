export class ThemeManager {
	theme = $state<'light' | 'dark'>('dark');

	init() {
		const savedTheme = localStorage.getItem('theme');

		if (savedTheme === 'light') {
			this.theme = 'light';
		} else {
			this.theme = 'dark';
		}

		document.documentElement.setAttribute('data-theme', this.theme);
	}

	toggle() {
		this.theme = this.theme === 'light' ? 'dark' : 'light';
		document.documentElement.setAttribute('data-theme', this.theme);

		localStorage.setItem('theme', this.theme);
	}

	setTheme(theme: 'dark' | 'light') {
		this.theme = theme;
		document.documentElement.setAttribute('data-theme', this.theme);

		localStorage.setItem('theme', this.theme);
	}
}

export const themeManager = new ThemeManager();
