export interface OmnisharpSettings {
	RoslynExtensionsOptions?: {
		EnableAnalyzersSupport?: boolean;
		LocationPaths?: string[];
	};
}

export enum OmnisharpSettingsKey {
	RoslynExtensionsOptions = 'RoslynExtensionsOptions',
	EnableAnalyzersSupport = 'EnableAnalyzersSupport',
	LocationPaths = 'LocationPaths'
}

export function findKey(settings: any, key: OmnisharpSettingsKey) {
	return Object.keys(settings).find(k => k.toLowerCase() === key.toLowerCase()) ?? key;
}