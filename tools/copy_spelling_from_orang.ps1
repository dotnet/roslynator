$spellingPath="../src/Workspaces.Core/Spelling/Core"

orang delete $spellingPath -e cs -n "Spellchecker.Identifier.cs" ne

orang copy "../../Orang/src/Spelling/Spelling" --target $spellingPath -e cs

orang replace $spellingPath -e cs -c "public(?= ((((static|sealed|abstract) )?class)|((readonly )?struct)|enum))" w -r "internal"

orang replace $spellingPath -e cs -c "namespace Orang.Spelling" w l -r "namespace Roslynator.Spelling"

orang replace $spellingPath -e cs -c "[NotNullWhen(true)] " l -t m

orang replace $spellingPath -e cs -c "\[(?<x>[\w\.]+)\.\.\]" -r ".Substring(`${x})"

orang replace $spellingPath -e cs -c "\[(?<x>[\w\.]+)\.\.(?<y>[\w\.]+)\]" -r ".Substring(`${x}, `${y} - `${x})"

orang replace $spellingPath -e cs -c " : ICapture" l -t m

orang replace $spellingPath -n WordList.cs e -c "StringComparer.FromComparison" l -r "StringComparerUtility.FromComparison"

orang replace $spellingPath -n WordChar.cs e -c "HashCode." l -r "Hash."

orang replace $spellingPath -e cs `
 -c "// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information." l `
 -r "// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information."

Write-Host DONE