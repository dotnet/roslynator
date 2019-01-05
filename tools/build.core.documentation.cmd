@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Preview\MSBuild\Current\Bin\MSBuild" "..\src\Core.sln" ^
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
