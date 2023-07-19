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
 --ignored-root-parts all `
 --max-derived-types 10

& $roslynatorExe generate-doc-root generate_ref_docs.sln `
 --properties Configuration=Release `
 -o "build/ref.md" `
 --host docusaurus `
 --heading "Roslynator .NET API Reference" `
 --ignored-parts content `
 --root-directory-url "ref"

  # & $roslynatorExe list-symbols generate_ref_docs.sln `
#  --properties Configuration=Release `
#  --visibility public `
#  --depth member `
#  --ignored-parts containing-namespace assembly-attributes `
#  --output "../docs/api.txt"

# & $roslynatorExe generate-doc-root generate_ref_docs.sln `
#  --properties Configuration=Release `
#  --projects Core `
#  -o "../src/Core/README.md" `
#  --host docusaurus `
#  --heading "Roslynator.Core" `
#  --root-directory-url $rootDirectoryUrl

# & $roslynatorExe generate-doc-root generate_ref_docs.sln `
#  --properties Configuration=Release `
#  --projects CSharp `
#  -o "../src/CSharp/README.md" `
#  --host docusaurus `
#  --heading "Roslynator.CSharp" `
#  --root-directory-url $rootDirectoryUrl

# & $roslynatorExe generate-doc-root generate_ref_docs.sln `
#  --properties Configuration=Release `
#  --projects Workspaces.Core `
#  -o "../src/Workspaces.Core/README.md" `
#  --host docusaurus `
#  --heading "Roslynator.CSharp.Workspaces" `
#  --root-directory-url $rootDirectoryUrl

# & $roslynatorExe generate-doc-root generate_ref_docs.sln `
#  --properties Configuration=Release `
#  --projects CSharp.Workspaces `
#  -o "../src/CSharp.Workspaces/README.md" `
#  --host docusaurus `
#  --heading "Roslynator.CSharp.Workspaces" `
#  --root-directory-url $rootDirectoryUrl

# & $roslynatorExe generate-doc-root generate_ref_docs.sln `
#  --properties Configuration=Release `
#  --projects Testing.Common Testing.CSharp Testing.CSharp.Xunit Testing.CSharp.MSTest `
#  -o "../src/Tests/README.md" `
#  --host docusaurus `
#  --heading "Roslynator Testing Framework" `
#  --root-directory-url $rootDirectoryUrl
