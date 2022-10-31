# DiagnosticsExtensions\.ReportDiagnostic Method

[Home](../../../README.md)

**Containing Type**: [DiagnosticsExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, ImmutableDictionary\<String, String\>, Object\[\])](#68790103) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, Object\[\])](#1383342935) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String\>, Object\[\])](#2280657848) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#2255916064) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#28701035) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#3010188031) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#1158862014) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, ImmutableDictionary\<String, String\>, Object\[\])](#894259342) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, Object\[\])](#33512774) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String\>, Object\[\])](#1696572768) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#2130173888) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#3236841423) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#1513434071) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#160845785) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, ImmutableDictionary\<String, String\>, Object\[\])](#2514698881) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, Object\[\])](#3276257697) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String\>, Object\[\])](#367812552) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\])](#2256519343) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\])](#351654068) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\])](#3667764902) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |
| [ReportDiagnostic(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\])](#3998532232) | Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\. |

<a id="68790103"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, ImmutableDictionary\<String, String\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)\>

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="1383342935"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="2280657848"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="2255916064"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="28701035"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="3010188031"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="1158862014"></a>

## ReportDiagnostic\(SymbolAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [ISymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isymbol)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SymbolAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SymbolAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.symbolanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="894259342"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, ImmutableDictionary\<String, String\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)\>

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="33512774"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="1696572768"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="2130173888"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="3236841423"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="1513434071"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="160845785"></a>

## ReportDiagnostic\(SyntaxNodeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxNodeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxNodeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxnodeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="2514698881"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, ImmutableDictionary\<String, String\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)\>

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="3276257697"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, IEnumerable\<Location\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.Location> additionalLocations, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**additionalLocations** &ensp; [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)\<[Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="367812552"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, ImmutableDictionary\<String, String\>, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, System.Collections.Immutable.ImmutableDictionary<string, string> properties, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**properties** &ensp; [ImmutableDictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.immutable.immutabledictionary-2)\<[String](https://docs.microsoft.com/en-us/dotnet/api/system.string), [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)\>

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="2256519343"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, Location, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.Location location, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**location** &ensp; [Location](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.location)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="351654068"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxNode, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxNode node, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**node** &ensp; [SyntaxNode](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxnode)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="3667764902"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxToken, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxToken token, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**token** &ensp; [SyntaxToken](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtoken)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]<a id="3998532232"></a>

## ReportDiagnostic\(SyntaxTreeAnalysisContext, DiagnosticDescriptor, SyntaxTrivia, Object\[\]\) 

  
Report a [Diagnostic](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostic) about a [SyntaxTree](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtree)\.

```csharp
public static void ReportDiagnostic(this Microsoft.CodeAnalysis.Diagnostics.SyntaxTreeAnalysisContext context, Microsoft.CodeAnalysis.DiagnosticDescriptor descriptor, Microsoft.CodeAnalysis.SyntaxTrivia trivia, params object[] messageArgs)
```

### Parameters

**context** &ensp; [SyntaxTreeAnalysisContext](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.syntaxtreeanalysiscontext)

**descriptor** &ensp; [DiagnosticDescriptor](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnosticdescriptor)

**trivia** &ensp; [SyntaxTrivia](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.syntaxtrivia)

**messageArgs** &ensp; [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)\[\]