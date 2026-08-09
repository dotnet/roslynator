#dotnet tool install -g roslynator.dotnet.cli

dotnet build "$PSScriptRoot/../src/CommandLine.slnx" /p:Configuration=Debug /v:m /m

roslynator loc "$PSScriptRoot/../src/Roslynator.slnx" `
    --ignore-block-boundary `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag
 