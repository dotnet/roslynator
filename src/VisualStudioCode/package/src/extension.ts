import * as vscode from 'vscode';
import * as os from 'os';
import * as path from 'path';
import { configurationFileContent } from './configurationFiles.generated';
import { ensureOmnisharpConfigurationUpdated } from './updateOmnisharpConfiguration';
import { openRoslynatorConfiguration } from './openRoslynatorConfiguration';

export function activate(context: vscode.ExtensionContext) {
	const shouldAutoUpdate = vscode.workspace.getConfiguration()
		.get('roslynator.autoUpdateOmnisharpJson');

	if (shouldAutoUpdate) {
		ensureOmnisharpConfigurationUpdated({
			homeDirectoryPath: os.homedir(),
			extensionDirectoryPath: context.extensionPath
		});
	}

	const localAppDataPath = getLocalAppDataPath();

	context.subscriptions.push(
		vscode.commands.registerCommand(
			'roslynator.openDefaultConfigurationFile',
			() => openRoslynatorConfiguration('.roslynatorconfig', configurationFileContent.roslynatorconfig, localAppDataPath))
	);
}

export function deactivate() { }

function getLocalAppDataPath() {
	const platform = os.platform();

	if (platform === 'win32') {
		return path.join(os.homedir(), 'AppData', 'Local');
	} else if (platform === 'linux' || platform === 'darwin') {
		return path.join(os.homedir(), '.local', 'share');
	}
}