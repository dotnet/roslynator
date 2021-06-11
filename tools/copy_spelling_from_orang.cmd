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

pause