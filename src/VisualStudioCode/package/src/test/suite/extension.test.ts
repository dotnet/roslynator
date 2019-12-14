import * as assert from 'assert';

import * as fs from 'fs-extra';
import * as path from 'path';

import * as roslynator from '../../extension';
import { OmnisharpSettings } from '../../omnisharpSettings';

suite('Auto update omnisharp.json', () => {
	const tempPath = path.join(__dirname, 'temp');

	const homePath = path.join(tempPath, 'home');
	const extensionsPath = path.join(homePath, '.vscode', 'extensions');

	const omnisharpPath = path.join(homePath, '.omnisharp');
	const omnisharpJsonPath = path.join(omnisharpPath, 'omnisharp.json');

	setup(() => {
		fs.mkdirSync(homePath, { recursive: true });
	});

	teardown(() => {
		fs.removeSync(tempPath);
	});

	test('Create omnisharp.json', () => {
		roslynator.ensureConfigurationUpdated({
			extensionDirectoryPath: path.join(extensionsPath, 'josefpihrt-vscode.roslynator-1.0.1'),
			homeDirectoryPath: homePath
		});

		assert.ok(fs.existsSync(omnisharpJsonPath));

		const omnisharpSettings = fs.readJSONSync(omnisharpJsonPath) as OmnisharpSettings;

		assert.ok(omnisharpSettings.RoslynExtensionsOptions?.EnableAnalyzersSupport);
		assert.ok(omnisharpSettings.RoslynExtensionsOptions?.LocationPaths?.some(
			p => p.endsWith('/temp/home/.vscode/extensions/josefpihrt-vscode.roslynator-1.0.1/roslyn/common')));
	});

	test('Update omnisharp.json', () => {
		const oldOmnisharpSettings: OmnisharpSettings = {
			RoslynExtensionsOptions: {
				EnableAnalyzersSupport: true,
				LocationPaths: [
					"/temp/home/.vscode/extensions/josefpihrt-vscode.roslynator-1.0.1/roslyn/common",
					"/temp/home/.vscode/extensions/josefpihrt-vscode.roslynator-1.0.1/roslyn/analyzers",
					"/temp/home/.vscode/extensions/josefpihrt-vscode.roslynator-1.0.1/roslyn/refactorings",
					"/temp/home/.vscode/extensions/josefpihrt-vscode.roslynator-1.0.1/roslyn/fixes"
				]
			}
		};

		fs.mkdirSync(omnisharpPath);
		fs.writeJSONSync(omnisharpJsonPath, oldOmnisharpSettings);

		roslynator.ensureConfigurationUpdated({
			extensionDirectoryPath: path.join(extensionsPath, 'josefpihrt-vscode.roslynator-1.0.2'),
			homeDirectoryPath: homePath
		});

		const omnisharpSettings = fs.readJSONSync(omnisharpJsonPath) as OmnisharpSettings;

		assert.ok(omnisharpSettings.RoslynExtensionsOptions?.LocationPaths?.every(
			p => p.includes('josefpihrt-vscode.roslynator-1.0.2')));
		assert.ok(omnisharpSettings.RoslynExtensionsOptions?.LocationPaths?.every(
			p => !p.includes('josefpihrt-vscode.roslynator-1.0.1')));
	});
});