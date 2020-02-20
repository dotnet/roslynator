import * as assert from 'assert';

import * as vscode from 'vscode';
import * as fs from 'fs-extra';
import * as path from 'path';

import { openRoslynatorConfiguration } from '../../openRoslynatorConfiguration';

suite('Open Roslynator configuration file', () => {
	test('Open configuration file for the first time (non-existing)', async () => {
		const appDataPath = path.join(__dirname, 'tempOpenForTheFirstTimeAppData');

		if (fs.existsSync(appDataPath)) {
			fs.removeSync(appDataPath);
		}

		await openRoslynatorConfiguration('roslynator.ruleset', 'test content', appDataPath);

		const openFilePath = vscode.window.activeTextEditor?.document.uri.path;
		const openFileContent = vscode.window.activeTextEditor?.document.getText();

		assert.ok(fs.existsSync(path.join(appDataPath, 'JosefPihrt', 'Roslynator', 'VisualStudioCode', 'roslynator.ruleset')));
		assert.ok(openFilePath?.endsWith('tempOpenForTheFirstTimeAppData/JosefPihrt/Roslynator/VisualStudioCode/roslynator.ruleset'));
		assert.equal(openFileContent, 'test content');
	});

	test('Open existing configuration file', async () => {
		const appDataPath = path.join(__dirname, 'tempOpenExistingAppData');

		if (fs.existsSync(appDataPath)) {
			fs.removeSync(appDataPath);
		}

		const roslynatorConfigurationDirectoryPath = path.join(appDataPath, 'JosefPihrt', 'Roslynator', 'VisualStudioCode');

		fs.mkdirSync(roslynatorConfigurationDirectoryPath, { recursive: true });
		fs.writeFileSync(path.join(roslynatorConfigurationDirectoryPath, 'roslynator.config'), 'existing content');

		await openRoslynatorConfiguration('roslynator.config', 'new content', appDataPath);

		const openFileContent = vscode.window.activeTextEditor?.document.getText();

		assert.equal(openFileContent, 'existing content');
	});
});