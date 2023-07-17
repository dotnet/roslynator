---
sidebar_label: RemoveRegionAsync
---

# WorkspaceExtensions\.RemoveRegionAsync\(Document, RegionInfo, CancellationToken\) Method

**Containing Type**: [WorkspaceExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.Workspaces\.dll

  
Creates a new document with the specified region removed\.

```csharp
public static System.Threading.Tasks.Task<Microsoft.CodeAnalysis.Document> RemoveRegionAsync(this Microsoft.CodeAnalysis.Document document, Roslynator.CSharp.Syntax.RegionInfo region, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**document** &ensp; [Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)

**region** &ensp; [RegionInfo](../../Syntax/RegionInfo/index.md)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)&lt;[Document](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.document)&gt;

