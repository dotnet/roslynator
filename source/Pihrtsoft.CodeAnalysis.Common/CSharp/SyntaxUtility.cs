// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class SyntaxUtility
    {
        public static bool IsUsingStaticInScope(
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

            while (node != null)
            {
                if (node.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    var namespaceDeclaration = (NamespaceDeclarationSyntax)node;

                    if (ContainsUsingStatic(namespaceDeclaration.Usings, namedTypeSymbol, semanticModel, cancellationToken))
                        return true;
                }
                else if (node.IsKind(SyntaxKind.CompilationUnit))
                {
                    var compilationUnit = (CompilationUnitSyntax)node;

                    if (ContainsUsingStatic(compilationUnit.Usings, namedTypeSymbol, semanticModel, cancellationToken))
                        return true;
                }

                node = node.Parent;
            }

            return false;
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
                    ISymbol symbol = semanticModel
                        .GetSymbolInfo(usingDirective.Name, cancellationToken)
                        .Symbol;

                    if (symbol?.IsKind(SymbolKind.ErrorType) == false
                        && namedTypeSymbol.Equals(symbol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static SyntaxTriviaList GetIndentTrivia(SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            SyntaxTriviaList triviaList = GetNodeForLeadingTrivia(node).GetLeadingTrivia();

            return SyntaxFactory.TriviaList(
                triviaList
                    .Reverse()
                    .TakeWhile(f => f.IsKind(SyntaxKind.WhitespaceTrivia)));
        }

        private static SyntaxNode GetNodeForLeadingTrivia(this SyntaxNode node)
        {
            foreach (SyntaxNode ancestor in node.AncestorsAndSelf())
            {
                if (ancestor.IsKind(SyntaxKind.IfStatement))
                {
                    return ((IfStatementSyntax)ancestor).ParentElse() ?? ancestor;
                }
                else if (ancestor.IsMemberDeclaration())
                {
                    return ancestor;
                }
                else if (ancestor.IsStatement())
                {
                    return ancestor;
                }
            }

            return node;
        }

        public static IEnumerable<TNode> FindNodes<TNode>(
            SyntaxNode root,
            IEnumerable<ReferencedSymbol> referencedSymbols) where TNode : SyntaxNode
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            if (referencedSymbols == null)
                throw new ArgumentNullException(nameof(referencedSymbols));

            foreach (ReferencedSymbol referencedSymbol in referencedSymbols)
            {
                foreach (ReferenceLocation referenceLocation in referencedSymbol.Locations)
                {
                    if (referenceLocation.IsCandidateLocation)
                        continue;

                    TNode identifierName = root
                        .FindNode(referenceLocation.Location.SourceSpan, getInnermostNodeForTie: true)
                        .FirstAncestorOrSelf<TNode>();

                    if (identifierName != null)
                        yield return identifierName;
                }
            }
        }
    }
}
