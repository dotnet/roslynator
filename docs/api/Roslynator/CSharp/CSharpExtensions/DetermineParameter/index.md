---
sidebar_label: DetermineParameter
---

# CSharpExtensions\.DetermineParameter Method

**Containing Type**: [CSharpExtensions](../index.md)

**Assembly**: Roslynator\.CSharp\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [DetermineParameter(SemanticModel, ArgumentSyntax, Boolean, Boolean, CancellationToken)](#547493537) | Determines a parameter symbol that matches to the specified argument\. Returns null if no matching parameter is found\. |
| [DetermineParameter(SemanticModel, AttributeArgumentSyntax, Boolean, Boolean, CancellationToken)](#3103958802) | Determines a parameter symbol that matches to the specified attribute argument\. Returns null if not matching parameter is found\. |

<a id="547493537"></a>

## DetermineParameter\(SemanticModel, ArgumentSyntax, Boolean, Boolean, CancellationToken\) 

  
Determines a parameter symbol that matches to the specified argument\.
Returns null if no matching parameter is found\.

```csharp
public static Microsoft.CodeAnalysis.IParameterSymbol DetermineParameter(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax argument, bool allowParams = false, bool allowCandidate = false, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**argument** &ensp; [ArgumentSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.argumentsyntax)

**allowParams** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowCandidate** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

<a id="3103958802"></a>

## DetermineParameter\(SemanticModel, AttributeArgumentSyntax, Boolean, Boolean, CancellationToken\) 

  
Determines a parameter symbol that matches to the specified attribute argument\.
Returns null if not matching parameter is found\.

```csharp
public static Microsoft.CodeAnalysis.IParameterSymbol DetermineParameter(this Microsoft.CodeAnalysis.SemanticModel semanticModel, Microsoft.CodeAnalysis.CSharp.Syntax.AttributeArgumentSyntax attributeArgument, bool allowParams = false, bool allowCandidate = false, System.Threading.CancellationToken cancellationToken = default)
```

### Parameters

**semanticModel** &ensp; [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel)

**attributeArgument** &ensp; [AttributeArgumentSyntax](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.attributeargumentsyntax)

**allowParams** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**allowCandidate** &ensp; [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

**cancellationToken** &ensp; [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)

### Returns

[IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

