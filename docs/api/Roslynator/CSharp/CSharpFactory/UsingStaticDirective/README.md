# CSharpFactory\.UsingStaticDirective Method

[Home](../../../../README.md)

**Containing Type**: [CSharpFactory](../README.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [UsingStaticDirective(NameSyntax)](#Roslynator_CSharp_CSharpFactory_UsingStaticDirective_Microsoft_CodeAnalysis_CSharp_Syntax_NameSyntax_) | |
| [UsingStaticDirective(SyntaxToken, SyntaxToken, NameSyntax, SyntaxToken)](#Roslynator_CSharp_CSharpFactory_UsingStaticDirective_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_NameSyntax_Microsoft_CodeAnalysis_SyntaxToken_) | |

## UsingStaticDirective\(NameSyntax\) <a id="Roslynator_CSharp_CSharpFactory_UsingStaticDirective_Microsoft_CodeAnalysis_CSharp_Syntax_NameSyntax_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingStaticDirective(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
```

### Parameters

**name** &ensp; [NameSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namesyntax)

### Returns

[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

## UsingStaticDirective\(SyntaxToken, SyntaxToken, NameSyntax, SyntaxToken\) <a id="Roslynator_CSharp_CSharpFactory_UsingStaticDirective_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_SyntaxToken_Microsoft_CodeAnalysis_CSharp_Syntax_NameSyntax_Microsoft_CodeAnalysis_SyntaxToken_"></a>

```csharp
public static Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax UsingStaticDirective(Microsoft.CodeAnalysis.SyntaxToken usingKeyword, Microsoft.CodeAnalysis.SyntaxToken staticKeyword, Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name, Microsoft.CodeAnalysis.SyntaxToken semicolonToken)
```

### Parameters

**usingKeyword** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**staticKeyword** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**name** &ensp; [NameSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.namesyntax)

**semicolonToken** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

### Returns

[UsingDirectiveSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.usingdirectivesyntax)

