#dotnet tool install -g roslynator.dotnet.cli

dotnet build "..\src\CommandLine.sln" /p:Configuration=Debug /v:m /m

roslynator loc "..\src\Roslynator.sln" `
    --ignore-block-boundary `
    --verbosity d `
    --file-log "roslynator.log" `
    --file-log-verbosity diag
 