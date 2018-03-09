@echo off
"..\source\Tools\MetadataGenerator\bin\Debug\MetadataGenerator" "..\source"
echo MetadataGenerator DONE
"..\source\Tools\CodeGenerator\bin\Debug\CodeGenerator" "..\source"
echo CodeGenerator DONE
pause