// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class CSharpUtility
    {
        private static readonly SymbolDisplayFormat _symbolDisplayFormat = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

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

        public static IEnumerable<InvocationExpressionSyntax> ConvertInterpolatedStringToStringBuilderAppend(
            InterpolatedStringExpressionSyntax interpolatedString,
            ExpressionSyntax stringBuilderExpression)
        {
            bool isVerbatim = interpolatedString.IsVerbatim();

            foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
            {
                switch (content.Kind())
                {
                    case SyntaxKind.Interpolation:
                        {
                            var interpolation = (InterpolationSyntax)content;

                            InterpolationAlignmentClauseSyntax alignmentClause = interpolation.AlignmentClause;
                            InterpolationFormatClauseSyntax formatClause = interpolation.FormatClause;

                            if (alignmentClause != null
                                || formatClause != null)
                            {
                                var sb = new StringBuilder();
                                sb.Append("\"{0");

                                if (alignmentClause != null)
                                {
                                    sb.Append(',');
                                    sb.Append(alignmentClause.Value.ToString());
                                }

                                if (formatClause != null)
                                {
                                    sb.Append(':');
                                    sb.Append(formatClause.FormatStringToken.Text);
                                }

                                sb.Append("}\"");

                                yield return SimpleMemberInvocationExpression(
                                    stringBuilderExpression,
                                    IdentifierName("AppendFormat"),
                                    ArgumentList(Argument(ParseExpression(sb.ToString())), Argument(interpolation.Expression)));
                            }
                            else
                            {
                                yield return SimpleMemberInvocationExpression(
                                    stringBuilderExpression,
                                    IdentifierName("Append"),
                                    Argument(interpolation.Expression));
                            }

                            break;
                        }
                    case SyntaxKind.InterpolatedStringText:
                        {
                            var interpolatedStringText = (InterpolatedStringTextSyntax)content;

                            string text = interpolatedStringText.TextToken.Text;

                            text = (isVerbatim)
                                ? "@\"" + text + "\""
                                : "\"" + text + "\"";

                            ExpressionSyntax stringLiteral = ParseExpression(text);

                            yield return SimpleMemberInvocationExpression(
                                stringBuilderExpression,
                                IdentifierName("Append"),
                                Argument(stringLiteral));

                            break;
                        }
                }
            }
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
    }
}
