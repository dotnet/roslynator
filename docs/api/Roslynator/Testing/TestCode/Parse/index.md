---
sidebar_label: Parse
---

# TestCode\.Parse Method

**Containing Type**: [TestCode](../index.md)

**Assembly**: Roslynator\.Testing\.Common\.dll

## Overloads

| Method | Summary |
| ------ | ------- |
| [Parse(String, String, String)](#3209459912) | Finds and replace span that is marked with `[\|\|]` token\. |
| [Parse(String)](#2022869111) | Finds and removes spans that are marked with `[\|` and `\|]` tokens\. |

<a id="3209459912"></a>

## Parse\(String, String, String\) 

  
Finds and replace span that is marked with `[||]` token\.

```csharp
public static Roslynator.Testing.TestCode Parse(string value, string replacement1, string replacement2 = null)
```

### Parameters

**value** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**replacement1** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

**replacement2** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### Returns

[TestCode](../index.md)

<a id="2022869111"></a>

## Parse\(String\) 

  
Finds and removes spans that are marked with `[|` and `|]` tokens\.

```csharp
public static Roslynator.Testing.TestCode Parse(string value)
```

### Parameters

**value** &ensp; [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### Returns

[TestCode](../index.md)

