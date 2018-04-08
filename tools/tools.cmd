@echo off
dotnet "..\src\Tools\AddCodeFileHeader\bin\Release\netcoreapp2.0\AddCodeFileHeader.dll" "..\src"
echo AddCodeFileHeader DONE
dotnet "..\src\Tools\MetadataGenerator\bin\Release\netcoreapp2.0\MetadataGenerator.dll" "..\src"
echo MetadataGenerator DONE
dotnet "..\src\Tools\CodeGenerator\bin\Release\netcoreapp2.0\CodeGenerator.dll" "..\src"
echo CodeGenerator DONE
pause