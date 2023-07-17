---
sidebar_label: IsParameterArrayOf
---

# SymbolExtensions\.IsParameterArrayOf Method

**Containing Type**: [SymbolExtensions](../index.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType, SpecialType)](#527099619) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType)](#2792278798) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType)](#3634009028) | Returns true if the parameter was declared as a parameter array that has a specified element type\. |

<a id="527099619"></a>

## IsParameterArrayOf\(IParameterSymbol, SpecialType, SpecialType, SpecialType\) 

  
Returns true if the parameter was declared as a parameter array that has one of specified element types\.

```csharp
public static bool IsParameterArrayOf(this Microsoft.CodeAnalysis.IParameterSymbol parameterSymbol, Microsoft.CodeAnalysis.SpecialType elementType1, Microsoft.CodeAnalysis.SpecialType elementType2, Microsoft.CodeAnalysis.SpecialType elementType3)
```

### Parameters

**parameterSymbol** &ensp; [IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

**elementType1** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**elementType2** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**elementType3** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="2792278798"></a>

## IsParameterArrayOf\(IParameterSymbol, SpecialType, SpecialType\) 

  
Returns true if the parameter was declared as a parameter array that has one of specified element types\.

```csharp
public static bool IsParameterArrayOf(this Microsoft.CodeAnalysis.IParameterSymbol parameterSymbol, Microsoft.CodeAnalysis.SpecialType elementType1, Microsoft.CodeAnalysis.SpecialType elementType2)
```

### Parameters

**parameterSymbol** &ensp; [IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

**elementType1** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

**elementType2** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

<a id="3634009028"></a>

## IsParameterArrayOf\(IParameterSymbol, SpecialType\) 

  
Returns true if the parameter was declared as a parameter array that has a specified element type\.

```csharp
public static bool IsParameterArrayOf(this Microsoft.CodeAnalysis.IParameterSymbol parameterSymbol, Microsoft.CodeAnalysis.SpecialType elementType)
```

### Parameters

**parameterSymbol** &ensp; [IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

**elementType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

