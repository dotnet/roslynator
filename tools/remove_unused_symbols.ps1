#dotnet tool install -g roslynator.dotnet.cli

roslynator find-symbols "$PSScriptRoot/../src/Roslynator.sln" `
 --symbol-kind type --unused --remove --exclude CommandLine/Orang/** --without-attribute System.ObsoleteAttribute