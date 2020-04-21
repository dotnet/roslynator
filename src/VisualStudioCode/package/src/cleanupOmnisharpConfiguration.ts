import * as json5 from 'json5';
import * as os from 'os';
import * as path from 'path';
import * as fs from 'fs';
import { findKey, OmnisharpSettingsKey } from './omnisharpSettings';

export function removeLocationPaths(homeDirectoryPath: string) {
	const omnisharpJsonPath = path.join(homeDirectoryPath, '.omnisharp', 'omnisharp.json');

	if (!fs.existsSync(omnisharpJsonPath)) {
		return;
	}

	const omnisharpJson = fs.readFileSync(omnisharpJsonPath, { encoding: 'utf8' });
	const omnisharpSettings = json5.parse(omnisharpJson);

	const roslynExtensionsOptionsKey = findKey(omnisharpSettings, OmnisharpSettingsKey.RoslynExtensionsOptions);

	if (!omnisharpSettings[roslynExtensionsOptionsKey] || typeof omnisharpSettings[roslynExtensionsOptionsKey] !== 'object') {
		return;
	}

	const locationPathsKey = findKey(omnisharpSettings[roslynExtensionsOptionsKey], OmnisharpSettingsKey.LocationPaths);

	if (!Array.isArray(omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey])) {
		return;
	}

	omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey] = (omnisharpSettings[roslynExtensionsOptionsKey][locationPathsKey] as any[])
		.filter(p => typeof p === 'string' && !p.includes('josefpihrt-vscode.roslynator'));

	const updatedOmnisharpJson = JSON.stringify(omnisharpSettings, null, 4);
	fs.writeFileSync(omnisharpJsonPath, updatedOmnisharpJson);
}

removeLocationPaths(os.homedir());

