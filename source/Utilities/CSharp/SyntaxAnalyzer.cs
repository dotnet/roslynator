// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class SyntaxAnalyzer
    {
        public static bool IsUsingDirectiveInScope(
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

            return UsingsInScope(node)
                .Any(usings => ContainsUsing(usings, namespaceSymbol, semanticModel, cancellationToken));
        }

        private static bool ContainsUsing(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            INamespaceSymbol namespaceSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (UsingDirectiveSyntax usingDirective in usingDirectives)
            {
                if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    && usingDirective.Alias == null)
                {
                    ISymbol symbol = semanticModel.GetSymbol(usingDirective.Name, cancellationToken);

                    if (symbol?.IsNamespace() == true
                        && string.Equals(namespaceSymbol.ToString(), symbol.ToString(), StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsUsingStaticDirectiveInScope(
            SyntaxNode node,
            INamedTypeSymbol namedTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            return UsingsInScope(node)
                .Any(usings => ContainsUsingStatic(usings, namedTypeSymbol, semanticModel, cancellationToken));
        }

        private static bool ContainsUsingStatic(
            SyntaxList<UsingDirectiveSyntax> usingDirectives,
            INamedTypeSymbol namedTypeSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (UsingDirectiveSyntax usingDirective in usingDirectives)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                {
                    ISymbol symbol = semanticModel.GetSymbol(usingDirective.Name, cancellationToken);

                    if (symbol?.IsErrorType() == false
                        && namedTypeSymbol.Equals(symbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static IEnumerable<SyntaxList<UsingDirectiveSyntax>> UsingsInScope(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    yield return ((NamespaceDeclarationSyntax)ancestor).Usings;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    yield return ((CompilationUnitSyntax)ancestor).Usings;
                }
            }
        }

        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return AreParenthesesRedundantOrInvalidPrivate(node, node.Kind());
        }

        public static bool AreParenthesesRedundantOrInvalid(SyntaxNode node, SyntaxKind replacementKind)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return AreParenthesesRedundantOrInvalidPrivate(node, replacementKind);
        }

        private static bool AreParenthesesRedundantOrInvalidPrivate(SyntaxNode node, SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                    return true;
            }

            SyntaxNode parent = node.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.Argument:
                case SyntaxKind.AttributeArgument:
                case SyntaxKind.QualifiedName:
                //case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                case SyntaxKind.ExpressionStatement:
                case SyntaxKind.TypeArgumentList:
                case SyntaxKind.VariableDeclaration:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.Interpolation:
                    return true;
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)parent;

                        return node == forEachStatement.Expression
                            || node == forEachStatement.Type;
                    }
                case SyntaxKind.WhileStatement:
                    return node == ((WhileStatementSyntax)parent).Condition;
                case SyntaxKind.DoStatement:
                    return node == ((DoStatementSyntax)parent).Condition;
                case SyntaxKind.UsingStatement:
                    return node == ((UsingStatementSyntax)parent).Expression;
                case SyntaxKind.LockStatement:
                    return node == ((LockStatementSyntax)parent).Expression;
                case SyntaxKind.IfStatement:
                    return node == ((IfStatementSyntax)parent).Condition;
                case SyntaxKind.SwitchStatement:
                    return node == ((SwitchStatementSyntax)parent).Expression;
                case SyntaxKind.ConditionalExpression:
                    {
                        var conditionalExpression = (ConditionalExpressionSyntax)parent;

                        return node == conditionalExpression.WhenTrue
                            || node == conditionalExpression.WhenFalse;
                    }
            }

            if (parent is AssignmentExpressionSyntax)
                return true;

            if (parent?.IsKind(SyntaxKind.EqualsValueClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.VariableDeclarator) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
                        return true;
                }
                else if (parent?.IsKind(SyntaxKind.Parameter) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsAwait(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                throw new ArgumentNullException(nameof(methodDeclaration));

            return methodDeclaration
                .DescendantNodes(node => !node.IsKind(
                    SyntaxKind.SimpleLambdaExpression,
                    SyntaxKind.ParenthesizedLambdaExpression,
                    SyntaxKind.AnonymousMethodExpression))
                .Any(f => f.IsKind(SyntaxKind.AwaitExpression));
        }

        public static SwitchSectionAnalysisResult AnalyzeSwitchSection(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return SwitchSectionAnalysisResult.AddBraces;
            }
            else if (statements.Count == 1)
            {
                if (statements[0].IsKind(SyntaxKind.Block))
                {
                    return SwitchSectionAnalysisResult.RemoveBraces;
                }
                else
                {
                    return SwitchSectionAnalysisResult.AddBraces;
                }
            }

            return SwitchSectionAnalysisResult.None;
        }
    }
}
