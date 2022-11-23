@echo off

dotnet build "..\src\Tests\Tests.sln" -c Release --no-incremental -v n

if errorlevel 1 (
 pause
 exit
)

dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.Common\Testing.Common.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.CSharp\Testing.CSharp.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.CSharp.Xunit\Testing.CSharp.Xunit.csproj"
dotnet pack -c Release --no-build -v normal "..\src\Tests\Testing.CSharp.MSTest\Testing.CSharp.MSTest.csproj"

echo OK
pause
