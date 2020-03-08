@echo off

set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild"
set _properties=Configuration=Release,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591,DefineConstants=VSCODE

dotnet restore --force "..\src\VisualStudioCode.sln"

%_msbuildPath% "..\src\VisualStudioCode.sln" ^
 /t:Clean,Build ^
 /p:%_properties% ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

cd ..\src\VisualStudioCode\package

del /S /Q roslyn\*.dll

cd roslyn

md common
md analyzers
md fixes
md refactorings

copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.Core.dll common
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.Common.dll common
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.CSharp.dll common
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.Workspaces.Core.dll common
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.Workspaces.Common.dll common
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.CSharp.Workspaces.dll common
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.CSharp.Analyzers.dll analyzers
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.CSharp.Analyzers.CodeFixes.dll analyzers
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.Formatting.Analyzers.dll analyzers
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.Formatting.Analyzers.CodeFixes.dll analyzers
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.CSharp.Refactorings.dll refactorings
copy /Y ..\..\bin\Release\netstandard2.0\Roslynator.CSharp.CodeFixes.dll fixes

cd ..

npm install

echo Package is being created
vsce package
echo Package successfully created

echo OK

pause
