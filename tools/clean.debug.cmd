@echo off

"C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild" "..\src\Roslynator.sln" ^
 /t:Clean ^
 /p:Configuration=Debug ^
 /v:minimal ^
 /m
