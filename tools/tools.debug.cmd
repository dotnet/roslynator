@echo off
dotnet "..\src\Tools\MetadataGenerator\bin\Debug\netcoreapp2.0\MetadataGenerator.dll" "..\src"
echo MetadataGenerator DONE
dotnet "..\src\Tools\CodeGenerator\bin\Debug\netcoreapp2.0\CodeGenerator.dll" "..\src"
echo CodeGenerator DONE
pause