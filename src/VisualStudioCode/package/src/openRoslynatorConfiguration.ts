import * as vscode from 'vscode';
import * as path from 'path';
import * as fs from 'fs';

export async function openRoslynatorConfiguration(fileName: string, content: string, localAppDataPath: string | undefined) {
	if (!localAppDataPath) {
		return;
	}

	const configurationDirectoryPath = path.join(localAppDataPath, 'JosefPihrt', 'Roslynator');

	if (!fs.existsSync(configurationDirectoryPath)) {
		fs.mkdirSync(configurationDirectoryPath, { recursive: true });
	}

	const configurationFilePath = path.join(configurationDirectoryPath, fileName);

	if (!fs.existsSync(configurationFilePath)) {
		fs.writeFileSync(configurationFilePath, content);
	}

	const document = await vscode.workspace.openTextDocument(configurationFilePath);
	await vscode.window.showTextDocument(document);
}