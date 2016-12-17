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

            foreach (NameSyntax name in NamespacesInScope(node))
            {
                if (name.IsParentKind(SyntaxKind.NamespaceDeclaration))
                {
                    if (IsNamespaceOrContainedInNamespace(name, namespaceSymbol, semanticModel, cancellationToken))
                        return true;
                }
                else if (IsNamespace(name, namespaceSymbol, semanticModel, cancellationToken))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsNamespace(
            NameSyntax name,
            INamespaceSymbol namespaceSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

            return symbol?.IsNamespace() == true
                && string.Equals(namespaceSymbol.ToString(), symbol.ToString(), StringComparison.Ordinal);
        }

        private static bool IsNamespaceOrContainedInNamespace(
            NameSyntax name,
            INamespaceSymbol namespaceSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

            if (symbol?.IsNamespace() == true)
            {
                string namespaceText = namespaceSymbol.ToString();

                if (string.Equals(namespaceText, symbol.ToString(), StringComparison.Ordinal))
                    return true;

                foreach (INamespaceSymbol containingNamespace in symbol.ContainingNamespaces())
                {
                    if (string.Equals(namespaceText, containingNamespace.ToString(), StringComparison.Ordinal))
                        return true;
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

            foreach (NameSyntax name in StaticClassesInScope(node))
            {
                ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                if (symbol?.Equals(staticClassSymbol) == true)
                    return true;
            }

            return false;
        }

        private static IEnumerable<UsingDirectiveSyntax> UsingDirectivesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    foreach (UsingDirectiveSyntax usingDirective in ((NamespaceDeclarationSyntax)ancestor).Usings)
                        yield return usingDirective;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    foreach (UsingDirectiveSyntax usingDirective in ((CompilationUnitSyntax)ancestor).Usings)
                        yield return usingDirective;
                }
            }
        }

        private static IEnumerable<NameSyntax> NamespacesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                    NameSyntax namespaceName = namespaceDeclaration.Name;

                    if (namespaceName != null)
                        yield return namespaceName;

                    foreach (NameSyntax name in namespaceDeclaration.Usings.Namespaces())
                        yield return name;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    var compilationUnit = (CompilationUnitSyntax)ancestor;

                    foreach (NameSyntax name in compilationUnit.Usings.Namespaces())
                        yield return name;
                }
            }
        }

        private static IEnumerable<NameSyntax> StaticClassesInScope(SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.Ancestors())
            {
                SyntaxKind kind = ancestor.Kind();

                if (kind == SyntaxKind.NamespaceDeclaration)
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)ancestor;

                    foreach (NameSyntax name in namespaceDeclaration.Usings.StaticClasses())
                        yield return name;
                }
                else if (kind == SyntaxKind.CompilationUnit)
                {
                    var compilationUnit = (CompilationUnitSyntax)ancestor;

                    foreach (NameSyntax name in compilationUnit.Usings.StaticClasses())
                        yield return name;
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
                case SyntaxKind.CollectionInitializerExpression:
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

        public static BracesAnalysisResult AnalyzeSwitchSection(SwitchSectionSyntax section)
        {
            if (section == null)
                throw new ArgumentNullException(nameof(section));

            SyntaxList<StatementSyntax> statements = section.Statements;

            if (statements.Count > 1)
            {
                return BracesAnalysisResult.AddBraces;
            }
            else if (statements.Count == 1)
            {
                if (statements[0].IsKind(SyntaxKind.Block))
                {
                    return BracesAnalysisResult.RemoveBraces;
                }
                else
                {
                    return BracesAnalysisResult.AddBraces;
                }
            }

            return BracesAnalysisResult.None;
        }
    }
}
