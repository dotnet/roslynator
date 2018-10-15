
# How to Generate API Documentation

1) Install package [Roslynator.CommandLine](http://www.nuget.org/packages/Roslynator.CommandLine/)&ensp;[![NuGet](https://img.shields.io/nuget/v/Roslynator.CommandLine.svg)](https://nuget.org/packages/Roslynator.CommandLine)

2) Add MSBuild Target to your csproj (vbproj) file

```xml
<Target Name="GenerateDocumentation" AfterTargets="RoslynatorInitialize" Condition=" '$(Configuration)' == 'Release'">

  <PropertyGroup>

    <!-- One or more assembly paths you want generator documentation for, for example: A.dll B.dll -->
    <RoslynatorAssemblies>&quot;$(TargetPath)&quot;</RoslynatorAssemblies>

    <!-- A file that will contain all assembly references -->
    <RoslynatorAssemblyReferencesFile>$(TargetDir)Roslynator.assemblyreferences</RoslynatorAssemblyReferencesFile>

    <RoslynatorAssemblyReferences>&quot;$(RoslynatorAssemblyReferencesFile)&quot; &quot;$(TargetPath)&quot;</RoslynatorAssemblyReferences>

  </PropertyGroup>

    <!-- Save assembly references to a file -->
    <WriteLinesToFile File="$(RoslynatorAssemblyReferencesFile)" Lines="@(_ResolveAssemblyReferenceResolvedFiles)" Overwrite="true" Encoding="Unicode" />

    <!-- Execute 'doc' command. This command will generate documentation files from specified assemblies -->
  <Exec Command="$(RoslynatorExe) generate-doc ^
    -a $(RoslynatorAssemblies) ^
    -r $(RoslynatorAssemblyReferences) ^
    -o &quot;$(SolutionDir)docs&quot; ^
    -h &quot;API Reference&quot;"
        LogStandardErrorAsError="true" ConsoleToMSBuild="true">
    <Output TaskParameter="ConsoleOutput" PropertyName="OutputOfExec" />
  </Exec>

    <!-- Execute 'declarations' command. This command will generate a single file that contains all declarations from specified assemblies -->
  <Exec Command="$(RoslynatorExe) generate-declarations ^
    -a $(RoslynatorAssemblies) ^
    -r $(RoslynatorAssemblyReferences) ^
    -o &quot;$(SolutionDir)docs\api.cs&quot;"
        LogStandardErrorAsError="true" ConsoleToMSBuild="true">
    <Output TaskParameter="ConsoleOutput" PropertyName="OutputOfExec" />
  </Exec>

</Target>
```

#### Commands

* [`generate-doc`](cli/generate-doc-command.md)
* [`generate-doc-root`](cli/generate-doc-root-command.md)
* [`generate-declarations`](cli/generate-declarations-command.md)

3) Build solution in **Release** configuration

4) Publish documentation to GitHub

## See Also

* [MSBuild reserved and well-known properties](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2017)
* [Common MSBuild project properties](https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2017)
