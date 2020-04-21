import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';
import * as json5 from 'json5';
import { OmnisharpSettingsKey, findKey } from './omnisharpSettings';
import { Context } from './context';

export function ensureOmnisharpConfigurationUpdated(context: Context) {
	const omnisharpDirectoryPath = path.join(context.homeDirectoryPath, '.omnisharp');

	if (!fs.existsSync(omnisharpDirectoryPath)) {
		fs.mkdirSync(omnisharpDirectoryPath);
	}

	const omnisharpJsonPath = path.join(omnisharpDirectoryPath, 'omnisharp.json');

	let omnisharpSettings: any;
	let settingsUpdated = false;

	if (fs.existsSync(omnisharpJsonPath)) {
		const omnisharpJson = fs.readFileSync(omnisharpJsonPath, { encoding: 'utf8' });
		omnisharpSettings = json5.parse(omnisharpJson);
	} else {
		omnisharpSettings = {};
	}

	const roslynExtensionsOptionsKey = findKey(omnisharpSettings, OmnisharpSettingsKey.RoslynExtensionsOptions);

	if (!omnisharpSettings[roslynExtensionsOptionsKey] || typeof omnisharpSettings[roslynExtensionsOptionsKey] !== 'object') {
		omnisharpSettings[roslynExtensionsOptionsKey] = {};
	}

	const enableAnalyzersSupportKey = findKey(omnisharpSettings[roslynExtensionsOptionsKey], OmnisharpSettingsKey.EnableAnalyzersSupport);

	if (omnisharpSettings[roslynExtensionsOptionsKey][enableAnalyzersSupportKey] !== true) {
		omnisharpSettings[roslynExtensionsOptionsKey][enableAnalyzersSupportKey] = true;
		settingsUpdated = true;
	}

	const locationPathsKey = findKey(omnisharpSettings[roslynExtensionsOptionsKey], OmnisharpSettingsKey.LocationPaths);

	if (!Array.isArray(omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey])) {
		omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey] = [];
	}

	const roslynPath = path.join(context.extensionDirectoryPath, 'roslyn');
	const locationPaths = [
		path.join(roslynPath, 'common'),
		path.join(roslynPath, 'analyzers'),
		path.join(roslynPath, 'refactorings'),
		path.join(roslynPath, 'fixes')
	].map(p => p.replace(/\\/g, '/'));

	const containsPaths = locationPaths.every(
		p => (omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey] as any[]).includes(p));

	if (!containsPaths) {
		const unrelatedPaths: string[] = (omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey] as any[])
			.filter(p => typeof p === 'string'
				&& !p.includes('josefpihrt-vscode.roslynator') && !locationPaths.includes(p));

		omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey] = [
			...unrelatedPaths,
			...locationPaths
		];
		settingsUpdated = true;
	}

	if (settingsUpdated) {
		const updatedOmnisharpJson = JSON.stringify(omnisharpSettings, null, 4);
		fs.writeFileSync(omnisharpJsonPath, updatedOmnisharpJson);

		vscode.window.showInformationMessage('omnisharp.json has been updated with Roslynator configuration.');
	}
}
