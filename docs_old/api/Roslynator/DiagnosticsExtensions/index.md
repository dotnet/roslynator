---
sidebar_label: DiagnosticsExtensions
---

# DiagnosticsExtensions Class

**Namespace**: [Roslynator](../index.md)

**Assembly**: Roslynator\.Core\.dll

  
A set of extension methods for [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext), [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext) and [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)\.

```csharp
public static class DiagnosticsExtensions
```

## Methods

| Method | Summary |
| ------ | ------- |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable&lt;Location&gt;, ImmutableDictionary&lt;String, String&gt;, Object\[\])](ReportDiagnostic/index.md#68790103) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable&lt;Location&gt;, Object\[\])](ReportDiagnostic/index.md#1383342935) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary&lt;String, String&gt;, Object\[\])](ReportDiagnostic/index.md#2280657848) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](ReportDiagnostic/index.md#2255916064) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](ReportDiagnostic/index.md#28701035) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](ReportDiagnostic/index.md#3010188031) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](ReportDiagnostic/index.md#1158862014) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable&lt;Location&gt;, ImmutableDictionary&lt;String, String&gt;, Object\[\])](ReportDiagnostic/index.md#894259342) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable&lt;Location&gt;, Object\[\])](ReportDiagnostic/index.md#33512774) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary&lt;String, String&gt;, Object\[\])](ReportDiagnostic/index.md#1696572768) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](ReportDiagnostic/index.md#2130173888) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](ReportDiagnostic/index.md#3236841423) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](ReportDiagnostic/index.md#1513434071) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](ReportDiagnostic/index.md#160845785) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable&lt;Location&gt;, ImmutableDictionary&lt;String, String&gt;, Object\[\])](ReportDiagnostic/index.md#2514698881) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable&lt;Location&gt;, Object\[\])](ReportDiagnostic/index.md#3276257697) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary&lt;String, String&gt;, Object\[\])](ReportDiagnostic/index.md#367812552) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](ReportDiagnostic/index.md#2256519343) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](ReportDiagnostic/index.md#351654068) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](ReportDiagnostic/index.md#3667764902) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](ReportDiagnostic/index.md#3998532232) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |

