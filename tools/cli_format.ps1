#dotnet tool install -g roslynator.dotnet.cli

dotnet build "..\src\CommandLine.sln" /p:Configuration=Debug /v:m /m

roslynator format "..\src\Roslynator.sln" `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag `
    --end-of-line crlf
