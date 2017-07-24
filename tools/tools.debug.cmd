@echo off
"..\source\Tools\AddCodeFileHeader\bin\Debug\AddCodeFileHeader" "..\source"
echo AddCodeFileHeader DONE
"..\source\Tools\MetadataGenerator\bin\Debug\MetadataGenerator" "..\source"
echo MetadataGenerator DONE
"..\source\Tools\CodeGenerator\bin\Debug\CodeGenerator" "..\source"
echo CodeGenerator DONE
pause