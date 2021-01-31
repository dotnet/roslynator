# MemberDeclarationListInfo\.IndexOf Method

[Home](../../../../../README.md)

**Containing Type**: [MemberDeclarationListInfo](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IndexOf(Func\<MemberDeclarationSyntax, Boolean>)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__) | Searches for a member that matches the predicate and returns zero\-based index of the first occurrence in the list\. |
| [IndexOf(MemberDeclarationSyntax)](#Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_) | The index of the member in the list\. |

## IndexOf\(Func\<MemberDeclarationSyntax, Boolean>\) <a id="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_System_Func_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_System_Boolean__"></a>

\
Searches for a member that matches the predicate and returns zero\-based index of the first occurrence in the list\.

```csharp
public int IndexOf(Func<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax, bool> predicate)
```

### Parameters

**predicate** &ensp; [Func](https://docs.microsoft.com/en-us/dotnet/api/system.func-2)\<[MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax), [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)>

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

## IndexOf\(MemberDeclarationSyntax\) <a id="Roslynator_CSharp_Syntax_MemberDeclarationListInfo_IndexOf_Microsoft_CodeAnalysis_CSharp_Syntax_MemberDeclarationSyntax_"></a>

\
The index of the member in the list\.

```csharp
public int IndexOf(Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax member)
```

### Parameters

**member** &ensp; [MemberDeclarationSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax)

### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)

