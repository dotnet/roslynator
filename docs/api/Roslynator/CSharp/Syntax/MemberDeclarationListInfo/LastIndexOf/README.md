# MemberDeclarationListInfo\.LastIndexOf Method

[Home](../../../../../README.md)

**Containing Type**: [MemberDeclarationListInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [LastIndexOf(Func\<MemberDeclarationSyntax, Boolean>)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__) | Searches for a member that matches the predicate and returns zero\-based index of the last occurrence in the list\. |
| [LastIndexOf(MemberDeclarationSyntax)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | Searches for a member and returns zero\-based index of the last occurrence in the list\. |

## LastIndexOf\(Func\<MemberDeclarationSyntax, Boolean>\) <a id="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__"></a>

\
Searches for a member that matches the predicate and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## LastIndexOf\(MemberDeclarationSyntax\) <a id="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_LastIndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
Searches for a member and returns zero\-based index of the last occurrence in the list\.

```csharp
public int LastIndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

