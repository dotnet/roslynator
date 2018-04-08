// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineUsingStaticRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            UsingDirectiveSyntax usingDirective,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var classSymbol = (INamedTypeSymbol)semanticModel.GetSymbol(usingDirective.Name, cancellationToken);

            SyntaxNode parent = usingDirective.Parent;

            Debug.Assert(parent.IsKind(SyntaxKind.CompilationUnit, SyntaxKind.NamespaceDeclaration));

            int index = SyntaxInfo.UsingDirectiveListInfo(parent).IndexOf(usingDirective);

            List<SimpleNameSyntax> names = CollectNames(parent, classSymbol, semanticModel, cancellationToken);

            SyntaxNode newNode = parent.ReplaceNodes(names, (node, _) =>
            {
                TypeSyntax type = classSymbol.ToMinimalTypeSyntax(semanticModel, node.SpanStart);

                return SimpleMemberAccessExpression(type, node).WithTriviaFrom(node);
            });

            newNode = RemoveUsingDirective(newNode, index);

            return await document.ReplaceNodeAsync(parent, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static List<SimpleNameSyntax> CollectNames(
            SyntaxNode node,
            INamedTypeSymbol classSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var names = new List<SimpleNameSyntax>();

            foreach (SyntaxNode descendant in node.DescendantNodes())
            {
                if (!descendant.IsParentKind(SyntaxKind.SimpleMemberAccessExpression)
                    && (descendant is SimpleNameSyntax name))
                {
                    ISymbol symbol = semanticModel.GetSymbol(name, cancellationToken);

                    if (symbol.IsKind(SymbolKind.Event, SymbolKind.Field, SymbolKind.Method, SymbolKind.Property)
                        && symbol.ContainingType?.Equals(classSymbol) == true)
                    {
                        names.Add(name);
                    }
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