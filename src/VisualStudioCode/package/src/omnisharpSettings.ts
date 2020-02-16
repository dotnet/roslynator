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