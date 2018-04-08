// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

                if (symbol?.Kind == SymbolKind.Namespace)
                {
                    string namespaceText = namespaceSymbol.ToString();

                    if (string.Equals(namespaceText, symbol.ToString(), StringComparison.Ordinal))
                    {
                        return true;
                    }
                    else if (name.IsParentKind(SyntaxKind.NamespaceDeclaration))
                    {
                        INamespaceSymbol containingNamespace = symbol.ContainingNamespace;

                        while (containingNamespace != null)
                        {
                            if (string.Equals(namespaceText, containingNamespace.ToString(), StringComparison.Ordinal))
                                return true;

                            containingNamespace = containingNamespace.ContainingNamespace;
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
                foreach (UsingDirectiveSyntax usingDirective in SyntaxInfo.UsingDirectiveListInfo(ancestor).Usings)
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

        public static bool IsEmptyStringExpression(
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

                    if (symbol?.Kind == SymbolKind.Field)
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        if (SymbolUtility.IsPublicStaticReadOnly(fieldSymbol, "Empty")
                            && fieldSymbol.ContainingType?.SpecialType == SpecialType.System_String
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
                    SymbolKind kind = symbol.Kind;

                    if (kind == SymbolKind.Namespace)
                    {
                        return SyntaxFactory.ParseName(symbol.ToString()).WithTriviaFrom(name);
                    }
                    else if (kind == SymbolKind.NamedType)
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

            if (expression?.Kind() == SyntaxKind.IdentifierName)
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

        public static bool IsPropertyOfNullableOfT(
            IdentifierNameSyntax identifierName,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (identifierName == null)
                return false;

            if (!string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal))
                return false;

            ISymbol symbol = semanticModel.GetSymbol(identifierName, cancellationToken);

            return SymbolUtility.IsPropertyOfNullableOfT(symbol, name);
        }

        public static bool IsStringConcatenation(BinaryExpressionSyntax addExpression, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return addExpression.Kind() == SyntaxKind.AddExpression
                && SymbolUtility.IsStringAdditionOperator(semanticModel.GetMethodSymbol(addExpression, cancellationToken));
        }

        public static bool IsStringLiteralConcatenation(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression?.Kind() != SyntaxKind.AddExpression)
                return false;

            while (true)
            {
                if (binaryExpression.Right?.WalkDownParentheses().Kind() != SyntaxKind.StringLiteralExpression)
                    return false;

                ExpressionSyntax left = binaryExpression.Left?.WalkDownParentheses();

                switch (left?.Kind())
                {
                    case SyntaxKind.StringLiteralExpression:
                        {
                            return true;
                        }
                    case SyntaxKind.AddExpression:
                        {
                            binaryExpression = (BinaryExpressionSyntax)left;
                            break;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
        }

        public static bool IsStringExpression(ExpressionSyntax expression, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression == null)
                return false;

            if (expression
                .WalkDownParentheses()
                .Kind()
                .Is(SyntaxKind.StringLiteralExpression, SyntaxKind.InterpolatedStringExpression))
            {
                return true;
            }

            return semanticModel.GetTypeInfo(expression, cancellationToken)
                .ConvertedType?
                .SpecialType == SpecialType.System_String;
        }

        public static bool IsBooleanExpression(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                return false;

            return CSharpFacts.IsBooleanExpression(expression.WalkDownParentheses().Kind())
                || semanticModel
                    .GetTypeInfo(expression, cancellationToken)
                    .ConvertedType?
                    .SpecialType == SpecialType.System_Boolean;
        }

        public static SyntaxToken GetIdentifier(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).Identifier;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).Identifier;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).Identifier;
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).Identifier;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).Identifier;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).Identifier;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).Identifier;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).Identifier;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).ThisKeyword;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).Identifier;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).Declaration?.Variables.FirstOrDefault()?.Identifier ?? default(SyntaxToken);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).Declaration?.Variables.FirstOrDefault()?.Identifier ?? default(SyntaxToken);
                case SyntaxKind.VariableDeclarator:
                    return ((VariableDeclaratorSyntax)node).Identifier;
            }

            return default(SyntaxToken);
        }
    }
}
