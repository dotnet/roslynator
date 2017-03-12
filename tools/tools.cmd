@echo off
"..\source\Tools\AddCodeFileHeader\bin\Release\AddCodeFileHeader" "..\source"
echo AddCodeFileHeader DONE
"..\source\Tools\MetadataGenerator\bin\Release\MetadataGenerator" "..\source"
echo MetadataGenerator DONE
"..\source\Tools\CodeGenerator\bin\Release\CodeGenerator" "..\source"
echo CodeGenerator DONE
pause