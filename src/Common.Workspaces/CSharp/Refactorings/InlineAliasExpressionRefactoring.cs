// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            var rewriter = new Rewriter(aliasSymbol, aliasSymbol.Target.ToTypeSyntax(), semanticModel, cancellationToken);

            SyntaxNode newNode = rewriter.Visit(parent);

            newNode = RemoveUsingDirective(newNode, index);

            return await document.ReplaceNodeAsync(parent, newNode, cancellationToken).ConfigureAwait(false);
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

        private class Rewriter : CSharpSyntaxRewriter
        {
            public Rewriter(IAliasSymbol aliasSymbol, TypeSyntax replacement, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                AliasSymbol = aliasSymbol;
                Replacement = replacement;
                SemanticModel = semanticModel;
                CancellationToken = cancellationToken;
            }

            public IAliasSymbol AliasSymbol { get; }

            public TypeSyntax Replacement { get; }

            public SemanticModel SemanticModel { get; }

            public CancellationToken CancellationToken { get; }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                IAliasSymbol aliasSymbol = SemanticModel.GetAliasInfo(node, CancellationToken);

                if (aliasSymbol?.Equals(AliasSymbol) == true)
                {
                    return Replacement
                        .WithTriviaFrom(node)
                        .WithSimplifierAnnotation();
                }

                return base.VisitIdentifierName(node);
            }
        }
    }
}
