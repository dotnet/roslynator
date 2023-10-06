dotnet restore "../src/CommandLine.sln" -v minimal /m
dotnet build "../src/CommandLine.sln" --no-restore -c Release -v minimal /m

$roslynatorExe="../src/CommandLine/bin/Release/net7.0/Roslynator"
$rootDirectoryUrl="build/ref"

& $roslynatorExe generate-doc generate_ref_docs.sln `
 --properties Configuration=Release `
 -o $rootDirectoryUrl `
 --host docusaurus `
 --heading "Roslynator .NET API Reference" `
 --group-by-common-namespace `
 --ignored-common-parts content `
 --max-derived-types 10 `
 --root-file-path "build/ref.md"
