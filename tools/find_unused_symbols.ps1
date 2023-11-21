#dotnet tool install -g roslynator.dotnet.cli

roslynator find-symbols "c:/code/jp/roslynator/src/roslynator.sln" `
 --symbol-kind type `
 --unused --exclude Tests/*.Tests/** Tests/Refactorings.Tests/** CommandLine/Orang/**