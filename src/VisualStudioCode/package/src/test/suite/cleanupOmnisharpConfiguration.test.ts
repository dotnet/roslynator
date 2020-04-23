import * as assert from 'assert';

import * as fs from 'fs-extra';
import * as path from 'path';

import { removeLocationPaths } from '../../cleanupOmnisharpConfiguration';

suite('Auto update omnisharp.json', () => {
	const tempPath = path.join(__dirname, 'temp');

	const homePath = path.join(tempPath, 'home');

	const omnisharpPath = path.join(homePath, '.omnisharp');
	const omnisharpJsonPath = path.join(omnisharpPath, 'omnisharp.json');

	setup(() => {
		fs.mkdirSync(omnisharpPath, { recursive: true });
	});

	teardown(() => {
		fs.removeSync(tempPath);
	});

	test('Remove Roslynator entries from LocationPaths', () => {
		const omnisharpSettingsBeforeCleanup = {
			RoslynExtensionsOptions: {
				enableAnalyzersSupport: true,
				locationPaths: [
					'/path/to/custom/analyzers/',
					'c:/Users/Adrian/.vscode/extensions/josefpihrt-vscode.roslynator-2.9.0/roslyn/common',
					'c:/Users/Adrian/.vscode/extensions/josefpihrt-vscode.roslynator-2.9.0/roslyn/analyzers',
					'c:/Users/Adrian/.vscode/extensions/josefpihrt-vscode.roslynator-2.9.0/roslyn/refactorings',
					'c:/Users/Adrian/.vscode/extensions/josefpihrt-vscode.roslynator-2.9.0/roslyn/fixes'
				]
			}
		};

		fs.writeJSONSync(omnisharpJsonPath, omnisharpSettingsBeforeCleanup);

		removeLocationPaths(homePath);

		const omnisharpSettings = fs.readJSONSync(omnisharpJsonPath);

		assert.ok(!(omnisharpSettings.RoslynExtensionsOptions.locationPaths as string[])
			.some(p => p.includes('josefpihrt-vscode.roslynator')));
		assert.ok((omnisharpSettings.RoslynExtensionsOptions.locationPaths as string[])
			.some(p => p === '/path/to/custom/analyzers/'));
	});
});