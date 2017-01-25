// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Analysis
{
    public static class CSharpAnalysis
    {
        public static TypeAnalysisResult AnalyzeType(
            VariableDeclarationSyntax variableDeclaration,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (variableDeclaration == null)
                throw new ArgumentNullException(nameof(variableDeclaration));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = variableDeclaration.Type;

            if (type != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                if (variables.Count > 0
                    && !variableDeclaration.IsParentKind(SyntaxKind.FieldDeclaration))
                {
                    ExpressionSyntax expression = variables[0].Initializer?.Value;

                    if (expression != null)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                        if (typeSymbol?.SupportsExplicitDeclaration() == true)
                        {
                            if (variables.Count > 1
                                || IsLocalConstDeclaration(variableDeclaration.Parent))
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.None;
                            }
                            else if (IsImplicitTypeAllowed(typeSymbol, expression, semanticModel, cancellationToken))
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.Implicit
                                    : TypeAnalysisResult.ExplicitButShouldBeImplicit;
                            }
                            else
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.Explicit;
                            }
                        }
                    }
                }
            }

            return TypeAnalysisResult.None;
        }

        public static TypeAnalysisResult AnalyzeType(
            DeclarationExpressionSyntax declarationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (declarationExpression == null)
                throw new ArgumentNullException(nameof(declarationExpression));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = declarationExpression.Type;

            if (type != null)
            {
                VariableDesignationSyntax designation = declarationExpression.Designation;

                if (designation != null)
                {
                    SyntaxKind kind = designation.Kind();

                    if (kind == SyntaxKind.ParenthesizedVariableDesignation)
                    {
                        return TypeAnalysisResult.None;
                    }
                    else if (kind == SyntaxKind.SingleVariableDesignation)
                    {
                        var symbol = semanticModel.GetDeclaredSymbol((SingleVariableDesignationSyntax)designation, cancellationToken) as ILocalSymbol;

                        if (symbol != null)
                        {
                            ITypeSymbol typeSymbol = symbol.Type;

                            if (typeSymbol?.SupportsExplicitDeclaration() == true)
                            {
                                return (type.IsVar)
                                    ? TypeAnalysisResult.ImplicitButShouldBeExplicit
                                    : TypeAnalysisResult.Explicit;
                            }
                        }
                    }
                }
            }

            return TypeAnalysisResult.None;
        }

        private static bool IsLocalConstDeclaration(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.LocalDeclarationStatement)
                && ((LocalDeclarationStatementSyntax)node).IsConst;
        }

        private static bool IsImplicitTypeAllowed(
            ITypeSymbol typeSymbol,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.CastExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.DefaultExpression:
                    {
                        return typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken));
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        return typeSymbol.Equals(semanticModel.GetTypeSymbol(expression, cancellationToken))
                            && semanticModel.GetSymbol(expression, cancellationToken)?.IsEnumField() == true;
                    }
            }

            return false;
        }

        public static TypeAnalysisResult AnalyzeType(
            ForEachStatementSyntax forEachStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (forEachStatement == null)
                throw new ArgumentNullException(nameof(forEachStatement));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            TypeSyntax type = forEachStatement.Type;

            if (type == null)
                return TypeAnalysisResult.None;

            if (!type.IsVar)
                return TypeAnalysisResult.Explicit;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol?.SupportsExplicitDeclaration() == true)
                return TypeAnalysisResult.ImplicitButShouldBeExplicit;

            return TypeAnalysisResult.Implicit;
        }

        public static BracesAnalysisResult AnalyzeBraces(SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

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

        public static BracesAnalysisResult AnalyzeBraces(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            bool anyHasEmbedded = false;
            bool anyHasBlock = false;
            bool allSupportsEmbedded = true;

            int cnt = 0;

            foreach (SyntaxNode node in IfElseChain.GetChain(ifStatement))
            {
                cnt++;

                StatementSyntax statement = GetStatement(node);

                if (!anyHasEmbedded && !statement.IsKind(SyntaxKind.Block))
                    anyHasEmbedded = true;

                if (!anyHasBlock && statement.IsKind(SyntaxKind.Block))
                    anyHasBlock = true;

                if (allSupportsEmbedded && !SupportsEmbedded(statement))
                    allSupportsEmbedded = false;

                if (cnt > 1 && anyHasEmbedded && !allSupportsEmbedded)
                {
                    return BracesAnalysisResult.AddBraces;
                }
            }

            if (cnt > 1
                && allSupportsEmbedded
                && anyHasBlock)
            {
                if (anyHasEmbedded)
                {
                    return BracesAnalysisResult.AddBraces | BracesAnalysisResult.RemoveBraces;
                }
                else
                {
                    return BracesAnalysisResult.RemoveBraces;
                }
            }

            return BracesAnalysisResult.None;
        }

        private static bool SupportsEmbedded(StatementSyntax statement)
        {
            if (statement.Parent.IsKind(SyntaxKind.IfStatement)
                && ((IfStatementSyntax)statement.Parent).Condition?.IsMultiLine() == true)
            {
                return false;
            }

            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                if (block.Statements.Count != 1)
                    return false;

                statement = block.Statements[0];
            }

            return !statement.IsKind(SyntaxKind.LocalDeclarationStatement)
                && !statement.IsKind(SyntaxKind.LabeledStatement)
                && statement.IsSingleLine();
        }

        private static StatementSyntax GetStatement(SyntaxNode node)
        {
            if (node.IsKind(SyntaxKind.IfStatement))
            {
                return ((IfStatementSyntax)node).Statement;
            }
            else
            {
                return ((ElseClauseSyntax)node).Statement;
            }
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

            foreach (NameSyntax name in NamespacesInScope(node))
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

        private static IEnumerable<UsingDirectiveSyntax> RegularUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (!usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)
                    && usingDirective.Alias == null)
                {
                    yield return usingDirective;
                }
            }
        }

        private static IEnumerable<NameSyntax> Namespaces(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings.RegularUsings())
            {
                NameSyntax name = usingDirective.Name;

                if (name != null)
                    yield return name;
            }
        }

        private static IEnumerable<UsingDirectiveSyntax> StaticUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
                    yield return usingDirective;
            }
        }

        private static IEnumerable<NameSyntax> StaticClasses(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings.StaticUsings())
            {
                NameSyntax name = usingDirective.Name;

                if (name != null)
                    yield return name;
            }
        }

        private static IEnumerable<UsingDirectiveSyntax> AliasUsings(this SyntaxList<UsingDirectiveSyntax> usings)
        {
            foreach (UsingDirectiveSyntax usingDirective in usings)
            {
                if (usingDirective.Alias != null)
                    yield return usingDirective;
            }
        }
    }
}
