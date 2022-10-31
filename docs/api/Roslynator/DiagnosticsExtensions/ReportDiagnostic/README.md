# DiagnosticsExtensions\.ReportDiagnostic Method

[Home](../../../README.md)

**Containing Type**: [DiagnosticsExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)>

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SymbolAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)>

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxNodeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, ImmutableDictionary\<String, String>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)>

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Generic_IEnumerable_Microsoft_CodeAnalysis_Location__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String>, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Collections_Immutable_ImmutableDictionary_System_String_System_String__System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_Location_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxNode_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxToken_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) <a id="Roslynator_DiagnosticsExtensions_ReportDiagnostic_Microsoft_CodeAnalysis_Diagnostics_SyntaxTreeAnalysisContext_Microsoft_CodeAnalysis_DiagnosticDescriptor_Microsoft_CodeAnalysis_SyntaxTrivia_System_Object___"></a>

\
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]