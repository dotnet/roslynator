// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Helpers;

namespace Roslynator.CSharp
{
    internal static class CSharpUtility
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        public static string GetCountOrLengthPropertyName(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol == null)
                return null;

            SymbolKind symbolKind = typeSymbol.Kind;

            if (symbolKind == SymbolKind.ErrorType)
                return null;

            if (symbolKind == SymbolKind.ArrayType)
                return "Length";

            string propertyName = GetCountOrLengthPropertyName(typeSymbol.SpecialType);

            if (propertyName != null)
                return (propertyName.Length > 0) ? propertyName : null;

            INamedTypeSymbol constructedFrom = null;

            if (symbolKind == SymbolKind.NamedType)
            {
                constructedFrom = ((INamedTypeSymbol)typeSymbol).ConstructedFrom;

                propertyName = GetCountOrLengthPropertyName(constructedFrom.SpecialType);

                if (propertyName != null)
                    return (propertyName.Length > 0) ? propertyName : null;
            }

            if (typeSymbol.ImplementsAny(
                SpecialType.System_Collections_Generic_ICollection_T,
                SpecialType.System_Collections_Generic_IReadOnlyCollection_T,
                allInterfaces: true))
            {
                if (typeSymbol.TypeKind == TypeKind.Interface)
                    return "Count";

                int position = expression.SpanStart;

                if (HasAccessibleProperty(typeSymbol, "Count", semanticModel, position))
                    return "Count";

                if (HasAccessibleProperty(typeSymbol, "Length", semanticModel, position))
                    return "Length";
            }

            return null;
        }

        private static bool HasAccessibleProperty(
            ITypeSymbol typeSymbol,
            string propertyName,
            SemanticModel semanticModel,
            int position)
        {
            foreach (ISymbol symbol in typeSymbol.GetMembers(propertyName))
            {
                if (symbol.Kind == SymbolKind.Property
                    && semanticModel.IsAccessible(position, symbol))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetCountOrLengthPropertyName(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.None:
                    return null;
                case SpecialType.System_String:
                case SpecialType.System_Array:
                    return "Length";
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return "Count";
            }

            return "";
        }

        public static bool IsNamespaceInScope(
            SyntaxNode node,
            INamespaceSymbol namespaceSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (namespaceSymbol == null)
                throw new ArgumentNullException(nameof(namespaceSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                switch (ancestor.Kind())
                {
                    case SyntaxKind.NamespaceDeclaration:
                        {
                            var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                            if (IsNamespace(namespaceSymbol, namespaceDeclaration.Name, semanticModel, cancellationToken)
                                || IsNamespace(namespaceSymbol, namespaceDeclaration.Usings, semanticModel, cancellationToken))
                            {
                                return true;
                            }

                            break;
                        }
                    case SyntaxKind.CompilationUnit:
                        {
                            var compilationUnit = (CompilationUnitSyntax)ancestor;

                            if (IsNamespace(namespaceSymbol, compilationUnit.Usings, semanticModel, cancellationToken))
                                return true;

                            break;
                        }
                }
            }

            return false;
        }

        private static bool IsNamespace(
            INamespaceSymbol namespaceSymbol,
            SyntaxList<UsingDirectiveSyntax> usings,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    && usingDirective.Alias == null
                    && IsNamespace(namespaceSymbol, usingDirective.Name, semanticModel, cancellationToken))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNamespace(
            INamespaceSymbol namespaceSymbol,
            NameSyntax name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (name != null)
            {
                ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                if (symbol?.IsNamespace() == true)
                {
                    string namespaceText = namespaceSymbol.ToString();

                    if (string.Equals(namespaceText, symbol.ToString(), StringComparison.Ordinal))
                    {
                        return true;
                    }
                    else if (name.IsParentKind(SyntaxKind.NamespaceDeclaration))
                    {
                        foreach (INamespaceSymbol containingNamespace in symbol.ContainingNamespaces())
                        {
                            if (string.Equals(namespaceText, containingNamespace.ToString(), StringComparison.Ordinal))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool IsStaticClassInScope(
            SyntaxNode node,
            INamedTypeSymbol staticClassSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (staticClassSymbol == null)
                throw new ArgumentNullException(nameof(staticClassSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                foreach (UsingDirectiveSyntax usingDirective in GetUsings(ancestor))
                {
                    if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    {
                        NameSyntax name = usingDirective.Name;

                        if (name != null
                            && staticClassSymbol.Equals(semanticModel.GetSymbol(name, cancellationToken)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static SyntaxList<UsingDirectiveSyntax> GetUsings(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)node).Usings;
                case SyntaxKind.CompilationUnit:
                    return ((CompilationUnitSyntax)node).Usings;
                default:
                    return default(SyntaxList<UsingDirectiveSyntax>);
            }
        }

        public static bool IsEmptyString(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.StringLiteralExpression)
            {
                return ((LiteralExpressionSyntax)expression).Token.ValueText.Length == 0;
            }
            else if (kind == SyntaxKind.InterpolatedStringExpression)
            {
                return !((InterpolatedStringExpressionSyntax)expression).Contents.Any();
            }
            else if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                if (memberAccess.Name?.Identifier.ValueText == "Empty")
                {
                    ISymbol symbol = semanticModel.GetSymbol(memberAccess, cancellationToken);

                    if (symbol?.IsField() == true)
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        if (string.Equals(fieldSymbol.Name, "Empty", StringComparison.Ordinal)
                            && fieldSymbol.ContainingType?.IsString() == true
                            && fieldSymbol.IsPublic()
                            && fieldSymbol.IsStatic
                            && fieldSymbol.IsReadOnly
                            && fieldSymbol.Type.IsString())
                        {
                            return true;
                        }
                    }
                }
            }

            Optional<object> optional = semanticModel.GetConstantValue(expression, cancellationToken);

            if (optional.HasValue)
            {
                var value = optional.Value as string;

                return value?.Length == 0;
            }

            return false;
        }

        public static NameSyntax EnsureFullyQualifiedName(
            NameSyntax name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

            if (symbol != null)
            {
                if (semanticModel.GetAliasInfo(name, cancellationToken) != null
                    || !symbol.ContainingNamespace.IsGlobalNamespace)
                {
                    if (symbol.IsNamespace())
                    {
                        return SyntaxFactory.ParseName(symbol.ToString()).WithTriviaFrom(name);
                    }
                    else if (symbol.IsNamedType())
                    {
                        return (NameSyntax)((INamedTypeSymbol)symbol).ToTypeSyntax(_symbolDisplayFormat).WithTriviaFrom(name);
                    }
                }
            }

            return name;
        }

        public static bool IsNameOfExpression(
            SyntaxNode node,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return node.IsKind(SyntaxKind.InvocationExpression)
                && IsNameOfExpression((InvocationExpressionSyntax)node, semanticModel, cancellationToken);
        }

        public static bool IsNameOfExpression(
            InvocationExpressionSyntax invocationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = invocationExpression.Expression;

            if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
            {
                var identifierName = (IdentifierNameSyntax)expression;

                if (string.Equals(identifierName.Identifier.ValueText, "nameof", StringComparison.Ordinal)
                    && semanticModel.GetSymbol(invocationExpression, cancellationToken) == null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsImplicitNumericConversion(ITypeSymbol from, ITypeSymbol to)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            return IsImplicitNumericConversion(from.SpecialType, to.SpecialType);
        }

        // http://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/implicit-numeric-conversions-table
        public static bool IsImplicitNumericConversion(SpecialType from, SpecialType to)
        {
            switch (from)
            {
                case SpecialType.System_Char:
                    {
                        switch (to)
                        {
                            case SpecialType.System_UInt16:
                            case SpecialType.System_Int32:
                            case SpecialType.System_UInt32:
                            case SpecialType.System_Int64:
                            case SpecialType.System_UInt64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_SByte:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Int16:
                            case SpecialType.System_Int32:
                            case SpecialType.System_Int64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_Byte:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Int16:
                            case SpecialType.System_UInt16:
                            case SpecialType.System_Int32:
                            case SpecialType.System_UInt32:
                            case SpecialType.System_Int64:
                            case SpecialType.System_UInt64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_Int16:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Int32:
                            case SpecialType.System_Int64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_UInt16:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Int32:
                            case SpecialType.System_UInt32:
                            case SpecialType.System_Int64:
                            case SpecialType.System_UInt64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_Int32:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Int64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_UInt32:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Int64:
                            case SpecialType.System_UInt64:
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Single:
                            case SpecialType.System_Double:
                            case SpecialType.System_Decimal:
                                return true;
                        }

                        break;
                    }
                case SpecialType.System_Single:
                    {
                        switch (to)
                        {
                            case SpecialType.System_Double:
                                return true;
                        }

                        break;
                    }
            }

            return false;
        }

        public static bool IsAllowedAccessibility(SyntaxNode node, Accessibility accessibility, bool allowOverride = false)
        {
            return AllowedAccessibilityHelper.IsAllowedAccessibility(node, accessibility, allowOverride: allowOverride);
        }
    }
}
