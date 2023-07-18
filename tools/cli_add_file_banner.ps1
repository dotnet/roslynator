#dotnet tool install -g roslynator.dotnet.cli

dotnet build "../src/CommandLine.sln" /p:Configuration=Debug /v:m /m

roslynator fix "../src/Roslynator.sln" `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag `
    --file-banner " Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information."
