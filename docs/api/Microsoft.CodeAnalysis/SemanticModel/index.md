---
sidebar_label: SemanticModel
---

# [SemanticModel](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.semanticmodel) Class Extensions

| Extension Method | Summary |
| ---------------- | ------- |
| [DetermineParameter(SemanticModel, ArgumentSyntax, Boolean, Boolean, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/DetermineParameter/index.md#547493537) | Determines a parameter symbol that matches to the specified argument\. Returns null if no matching parameter is found\. |
| [DetermineParameter(SemanticModel, AttributeArgumentSyntax, Boolean, Boolean, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/DetermineParameter/index.md#3103958802) | Determines a parameter symbol that matches to the specified attribute argument\. Returns null if not matching parameter is found\. |
| [GetEnclosingNamedType(SemanticModel, Int32, CancellationToken)](../../Roslynator/SemanticModelExtensions/GetEnclosingNamedType/index.md) | Returns the innermost named type symbol that the specified position is considered inside of\. |
| [GetEnclosingSymbol&lt;TSymbol&gt;(SemanticModel, Int32, CancellationToken)](../../Roslynator/SemanticModelExtensions/GetEnclosingSymbol/index.md) | Returns the innermost symbol of type **TSymbol** that the specified position is considered inside of\. |
| [GetExtensionMethodInfo(SemanticModel, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetExtensionMethodInfo/index.md) | Returns what extension method symbol, if any, the specified expression syntax bound to\. |
| [GetMethodSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetMethodSymbol/index.md) | Returns method symbol, if any, the specified expression syntax bound to\. |
| [GetReducedExtensionMethodInfo(SemanticModel, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetReducedExtensionMethodInfo/index.md) | Returns what extension method symbol, if any, the specified expression syntax bound to\. |
| [GetSymbol(SemanticModel, AttributeSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetSymbol/index.md#3570389687) | Returns what symbol, if any, the specified attribute syntax bound to\. |
| [GetSymbol(SemanticModel, ConstructorInitializerSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetSymbol/index.md#3142024581) | Returns what symbol, if any, the specified constructor initializer syntax bound to\. |
| [GetSymbol(SemanticModel, CrefSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetSymbol/index.md#423864560) | Returns what symbol, if any, the specified cref syntax bound to\. |
| [GetSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetSymbol/index.md#2073342452) | Returns what symbol, if any, the specified expression syntax bound to\. |
| [GetSymbol(SemanticModel, OrderingSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetSymbol/index.md#1387654106) | Returns what symbol, if any, the specified ordering syntax bound to\. |
| [GetSymbol(SemanticModel, SelectOrGroupClauseSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetSymbol/index.md#2866826046) | Returns what symbol, if any, the specified select or group clause bound to\. |
| [GetSymbol(SemanticModel, SyntaxNode, CancellationToken)](../../Roslynator/SemanticModelExtensions/GetSymbol/index.md) | Returns what symbol, if any, the specified node bound to\. |
| [GetTypeByMetadataName(SemanticModel, String)](../../Roslynator/SemanticModelExtensions/GetTypeByMetadataName/index.md) | Returns the type within the compilation's assembly using its canonical CLR metadata name\. |
| [GetTypeSymbol(SemanticModel, AttributeSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetTypeSymbol/index.md#4220455895) | Returns type information about an attribute syntax\. |
| [GetTypeSymbol(SemanticModel, ConstructorInitializerSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetTypeSymbol/index.md#2306729789) | Returns type information about a constructor initializer syntax\. |
| [GetTypeSymbol(SemanticModel, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetTypeSymbol/index.md#1357550300) | Returns type information about an expression syntax\. |
| [GetTypeSymbol(SemanticModel, SelectOrGroupClauseSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/GetTypeSymbol/index.md#1028976081) | Returns type information about a select or group clause\. |
| [GetTypeSymbol(SemanticModel, SyntaxNode, CancellationToken)](../../Roslynator/SemanticModelExtensions/GetTypeSymbol/index.md) | Returns type information about the specified node\. |
| [HasConstantValue(SemanticModel, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/HasConstantValue/index.md) | Returns true if the specified node has a constant value\. |
| [IsDefaultValue(SemanticModel, ITypeSymbol, ExpressionSyntax, CancellationToken)](../../Roslynator/CSharp/CSharpExtensions/IsDefaultValue/index.md) | Returns true if the specified expression represents default value of the specified type\. |

