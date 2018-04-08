// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineAliasExpression
{
    internal static class InlineAliasExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            UsingDirectiveSyntax usingDirective,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IAliasSymbol aliasSymbol = semanticModel.GetDeclaredSymbol(usingDirective, cancellationToken);

            SyntaxNode parent = usingDirective.Parent;

            Debug.Assert(parent.IsKind(SyntaxKind.CompilationUnit, SyntaxKind.NamespaceDeclaration), "");

            int index = SyntaxInfo.UsingDirectiveListInfo(parent).IndexOf(usingDirective);

            List<IdentifierNameSyntax> names = CollectNames(parent, aliasSymbol, semanticModel, cancellationToken);

            NameSyntax name = usingDirective.Name;

            ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

            SyntaxNode newNode = parent.ReplaceNodes(names, (f, _) =>
            {
                if (symbol != null
                    && semanticModel
                        .GetSpeculativeSymbolInfo(f.SpanStart, name, SpeculativeBindingOption.BindAsTypeOrNamespace)
                        .Symbol?
                        .Equals(symbol) == true)
                {
                    return name.WithTriviaFrom(f);
                }
                else
                {
                    return aliasSymbol.Target.ToMinimalTypeSyntax(semanticModel, f.SpanStart).WithTriviaFrom(f);
                }
            });

            newNode = RemoveUsingDirective(newNode, index);

            return await document.ReplaceNodeAsync(parent, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static List<IdentifierNameSyntax> CollectNames(
            SyntaxNode node,
            IAliasSymbol aliasSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var names = new List<IdentifierNameSyntax>();

            foreach (SyntaxNode descendant in node.DescendantNodes())
            {
                if (descendant.Kind() == SyntaxKind.IdentifierName)
                {
                    IAliasSymbol symbol = semanticModel.GetAliasInfo(descendant, cancellationToken);

                    if (symbol?.Equals(aliasSymbol) == true)
                        names.Add((IdentifierNameSyntax)descendant);
                }
            }

            return names;
        }

        private static SyntaxNode RemoveUsingDirective(SyntaxNode node, int index)
        {
            switch (node.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var compilationUnit = (CompilationUnitSyntax)node;

                        UsingDirectiveSyntax usingDirective = compilationUnit.Usings[index];
                        return compilationUnit.RemoveNode(usingDirective);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var namespaceDeclaration = (NamespaceDeclarationSyntax)node;

                        UsingDirectiveSyntax usingDirective = namespaceDeclaration.Usings[index];
                        return namespaceDeclaration.RemoveNode(usingDirective);
                    }
            }

            return node;
        }
    }
}
