# UsingDirectiveListInfo\.IndexOf Method

[Home](../../../../../README.md)

**Containing Type**: [UsingDirectiveListInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(Func\<UsingDirectiveSyntax, Boolean\>)](#2836230997) | Searches for an using directive that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(UsingDirectiveSyntax)](#1386691920) | The index of the using directive in the list\. |

<a id="2836230997"></a>

## IndexOf\(Func\<UsingDirectiveSyntax, Boolean\>\) 

  
Searches for an using directive that matches the predicate and returns zero\-based index of the first occurrence in the list\.

```csharp
public int IndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)\>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

<a id="1386691920"></a>

## IndexOf\(UsingDirectiveSyntax\) 

  
The index of the using directive in the list\.

```csharp
public int IndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax usingDirective)
```

### Parameters

**usingDirective** &ensp; [UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

