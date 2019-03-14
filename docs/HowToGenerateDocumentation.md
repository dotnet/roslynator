
# How to Generate API Documentation

1) Install package [Roslynator.CommandLine](http://www.nuget.org/packages/Roslynator.CommandLine/)&ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.CommandLine.svg)](https://nuget.org/packages/Roslynator.CommandLine)

2) Add MSBuild Target to your csproj (vbproj) file

```xml
<Target Name="GenerateDocumentation" AfterTargets="RoslynatorInitialize" Condition=" '$(Configuration)' == 'Release'">

    <!-- Execute 'generate-doc' command. This command will generate documentation files -->
  <Exec Command="$(RoslynatorExe) generate-doc &quot;$(SolutionPath)&quot; ^
    --msbuild-path &quot;$(MSBuildBinPath)&quot; ^
    -o &quot;$(SolutionDir)docs&quot; ^
    -h &quot;API Reference&quot;"
        LogStandardErrorAsError="true" ConsoleToMSBuild="true">
    <Output TaskParameter="ConsoleOutput" PropertyName="OutputOfExec" />
  </Exec>

    <!-- Execute 'list-symbols' command. This command will generate list of symbol definitions -->
  <Exec Command="$(RoslynatorExe) list-symbols &quot;$(SolutionPath)&quot; ^
    --msbuild-path &quot;$(MSBuildBinPath)&quot; ^
    --output &quot;$(SolutionDir)docs\api.cs&quot; ^
	--visibility public"
        LogStandardErrorAsError="true" ConsoleToMSBuild="true">
    <Output TaskParameter="ConsoleOutput" PropertyName="OutputOfExec" />
  </Exec>

</Target>
```

#### Commands

* [`generate-doc`](cli/generate-doc-command.md)
* [`generate-doc-root`](cli/generate-doc-root-command.md)
* [`list-symbols`](cli/list-symbols-command.md)

3) Build solution in **Release** configuration

4) Publish documentation to GitHub

## See Also

* [MSBuild reserved and well-known properties](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2017)
* [Common MSBuild project properties](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2017)
