@echo off

"C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild" "..\src\Core.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Release,ShouldGenerateDocumentation=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

echo OK
pause
