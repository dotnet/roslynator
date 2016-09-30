@echo off
AddCodeFileHeader "..\source"
echo AddCodeFileHeader DONE
CodeGenerator "..\source"
echo CodeGenerator DONE
MetadataGenerator "..\source"
echo MetadataGenerator DONE
pause