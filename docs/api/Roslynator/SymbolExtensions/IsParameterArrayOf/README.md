# SymbolExtensions\.IsParameterArrayOf Method

[Home](../../../README.md)

**Containing Type**: [SymbolExtensions](../README.md)

**Assembly**: Roslynator\.Core\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [IsParameterArrayOf(IParameterSymbol, SpecialType)](#Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the parameter was declared as a parameter array that has a specified element type\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType)](#Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |
| [IsParameterArrayOf(IParameterSymbol, SpecialType, SpecialType, SpecialType)](#Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_) | Returns true if the parameter was declared as a parameter array that has one of specified element types\. |

## IsParameterArrayOf\(IParameterSymbol, SpecialType\) <a id="Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_"></a>

\
Returns true if the parameter was declared as a parameter array that has a specified element type\.

```csharp
public static bool IsParameterArrayOf(this Microsoft.CodeAnalysis.IParameterSymbol parameterSymbol, Microsoft.CodeAnalysis.SpecialType elementType)
```

### Parameters

**parameterSymbol** &ensp; [IParameterSymbol](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.iparametersymbol)

**elementType** &ensp; [SpecialType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.specialtype)

### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)

## IsParameterArrayOf\(IParameterSymbol, SpecialType, SpecialType\) <a id="Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_"></a>

\
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

## IsParameterArrayOf\(IParameterSymbol, SpecialType, SpecialType, SpecialType\) <a id="Roslynator_SymbolExtensions_IsParameterArrayOf_Microsoft_CodeAnalysis_IParameterSymbol_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_Microsoft_CodeAnalysis_SpecialType_"></a>

\
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

