import * as vscode from 'vscode';
import * as os from 'os';
import * as path from 'path';
import * as fs from 'fs';
import * as json5 from 'json5';
import { OmnisharpSettings } from './omnisharpSettings';
import { Context } from './context';

export function activate(context: vscode.ExtensionContext) {
	const shouldAutoUpdate = vscode.workspace.getConfiguration()
		.get('roslynator.autoUpdateOmnisharpJson');

	if (shouldAutoUpdate) {
		ensureConfigurationUpdated({
			homeDirectoryPath: os.homedir(),
			extensionDirectoryPath: context.extensionPath
		});
	}
}

export function deactivate() { }

export function ensureConfigurationUpdated(context: Context) {
	const omnisharpDirectoryPath = path.join(context.homeDirectoryPath, '.omnisharp');

	if (!fs.existsSync(omnisharpDirectoryPath)) {
		fs.mkdirSync(omnisharpDirectoryPath);
	}

	const omnisharpJsonPath = path.join(omnisharpDirectoryPath, 'omnisharp.json');

	let omnisharpSettings: OmnisharpSettings;
	let settingsUpdated = false;

	if (fs.existsSync(omnisharpJsonPath)) {
		const omnisharpJson = fs.readFileSync(omnisharpJsonPath, { encoding: 'utf8' });
		omnisharpSettings = json5.parse(omnisharpJson);
	} else {
		omnisharpSettings = {};
	}

	if (!omnisharpSettings.RoslynExtensionsOptions || typeof omnisharpSettings.RoslynExtensionsOptions !== 'object') {
		omnisharpSettings.RoslynExtensionsOptions = {};
	}

	if (omnisharpSettings.RoslynExtensionsOptions.EnableAnalyzersSupport !== true) {
		omnisharpSettings.RoslynExtensionsOptions.EnableAnalyzersSupport = true;
		settingsUpdated = true;
	}

	if (!Array.isArray(omnisharpSettings.RoslynExtensionsOptions.LocationPaths)) {
		omnisharpSettings.RoslynExtensionsOptions.LocationPaths = [];
	}

	const roslynPath = path.join(context.extensionDirectoryPath, 'roslyn');
	const locationPaths = [
		path.join(roslynPath, 'common'),
		path.join(roslynPath, 'analyzers'),
		path.join(roslynPath, 'refactorings'),
		path.join(roslynPath, 'fixes')
	].map(p => p.replace(/\\/g, '/'));

	const containsPaths = locationPaths.every(
		p => omnisharpSettings.RoslynExtensionsOptions!.LocationPaths!.includes(p));

	if (!containsPaths) {
		const unrelatedPaths = omnisharpSettings.RoslynExtensionsOptions.LocationPaths
			.filter(p => !p.includes('josefpihrt-vscode.roslynator') && !locationPaths.includes(p));

		omnisharpSettings.RoslynExtensionsOptions.LocationPaths = [
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