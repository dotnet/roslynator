#dotnet tool install -g roslynator.dotnet.cli

dotnet build "$PSScriptRoot/../src/CommandLine.sln" /p:Configuration=Debug /v:m /m

roslynator lloc "$PSScriptRoot/../src/Roslynator.sln" `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag
