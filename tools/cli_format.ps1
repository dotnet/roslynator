#dotnet tool install -g roslynator.dotnet.cli

dotnet build "$PSScriptRoot/../src/CommandLine.slnx" /p:Configuration=Debug /v:m /m

roslynator format "$PSScriptRoot/../src/Roslynator.sln" `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag `
    --end-of-line crlf
