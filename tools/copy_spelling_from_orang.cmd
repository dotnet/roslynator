@echo off

set _spellingPath="..\src\Workspaces.Core\Spelling\Core"

orang delete %_spellingPath% -e cs

orang copy "..\..\Orang\src\Spelling\Spelling" --target %_spellingPath% -e cs

orang replace %_spellingPath% -e cs -c "public(?= ((((static|sealed|abstract) )?class)|((readonly )?struct)|enum))" w -r "internal"

orang replace %_spellingPath% -e cs -c "namespace Orang.Spelling" w l -r "namespace Roslynator.Spelling"

orang replace %_spellingPath% -e cs -c "[NotNullWhen(true)] " l -t m

orang replace %_spellingPath% -e cs -c "\[(?<x>[\w\.]+)\.\.\]" -r ".Substring(${x})"

orang replace %_spellingPath% -e cs -c "\[(?<x>[\w\.]+)\.\.(?<y>[\w\.]+)\]" -r ".Substring(${x}, ${y} - ${x})"

orang replace %_spellingPath% -e cs -c " : ICapture" l -t m

orang replace %_spellingPath% -n WordList.cs e -c "StringComparer.FromComparison" l -r "StringComparerUtility.FromComparison"

orang replace %_spellingPath% -n WordChar.cs e -c "HashCode." l -r "Hash."

orang replace %_spellingPath% -e cs ^
 -c "// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information." l ^
 -r "// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information."

pause