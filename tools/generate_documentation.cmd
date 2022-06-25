@echo off

set _programFiles=%ProgramFiles%

set _roslynatorExe="..\src\CommandLine\bin\Debug\net48\roslynator"
set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin"
set _msbuildProperties="Configuration=Release"
set _rootDirectoryUrl="../../docs/api/"

dotnet restore /p:RoslynatorCommandLine=true,Configuration=Debug "..\src\CommandLine.sln"

%_msbuildPath%\msbuild "..\src\CommandLine.sln" /t:Clean,Build /p:RoslynatorCommandLine=true,Configuration=Debug /v:m /m

%_roslynatorExe% generate-doc "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 -o "..\docs\api" ^
 --heading "Roslynator Reference" ^
 --target docusaurus ^
 --group-by-common-namespace ^
 --ignored-common-parts content ^
 --max-derived-types 10

%_roslynatorExe% list-symbols "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --visibility public ^
 --depth member ^
 --ignored-parts containing-namespace assembly-attributes ^
 --output "..\docs\api.txt"

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects Core ^
 -o "..\src\Core\README.md" ^
 --heading "Roslynator.Core" ^
 --target docusaurus ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects CSharp ^
 -o "..\src\CSharp\README.md" ^
 --heading "Roslynator.CSharp" ^
 --target docusaurus ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects Workspaces.Core ^
 -o "..\src\Workspaces.Core\README.md" ^
 --heading "Roslynator.CSharp.Workspaces" ^
 --target docusaurus ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects CSharp.Workspaces ^
 -o "..\src\CSharp.Workspaces\README.md" ^
 --heading "Roslynator.CSharp.Workspaces" ^
 --target docusaurus ^
 --root-directory-url %_rootDirectoryUrl%

%_roslynatorExe% generate-doc-root "..\src\Core.sln" ^
 -m %_msbuildPath% ^
 --properties %_msbuildProperties% ^
 --projects Testing.Common Testing.CSharp Testing.CSharp.Xunit ^
 -o "..\src\Tests\README.md" ^
 --heading "Roslynator Testing Framework" ^
 --target docusaurus ^
 --root-directory-url %_rootDirectoryUrl%

pause
