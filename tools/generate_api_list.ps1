$roslynatorExe="../src/CommandLine/bin/Debug/net7.0/roslynator"

dotnet restore "../src/CommandLine.sln" -v minimal /m
dotnet build "../src/CommandLine.sln" --no-restore -c Debug -v minimal /m

& $roslynatorExe list-symbols generate_ref_docs.sln `
 --properties Configuration=Release `
 --visibility public `
 --depth member `
 --ignored-parts containing-namespace assembly-attributes `
 --output "api.txt"
