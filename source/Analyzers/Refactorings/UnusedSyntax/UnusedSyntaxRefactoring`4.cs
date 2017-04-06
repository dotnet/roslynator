// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal abstract class UnusedSyntaxRefactoring<TNode, TListSyntax, TSyntax, TSymbol>
        where TNode : SyntaxNode
        where TListSyntax : SyntaxNode
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
    {
        protected abstract SyntaxTokenList GetModifiers(TNode node);

        protected abstract CSharpSyntaxNode GetBody(TNode node);

        protected abstract SeparatedSyntaxList<TSyntax> GetSeparatedList(TListSyntax list);

        protected abstract TListSyntax GetList(TNode node);

        protected abstract string GetIdentifier(TSyntax syntax);

        public ImmutableArray<TSyntax> FindUnusedSyntax(TNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            TListSyntax list = GetList(node);

            if (list != null)
            {
                SeparatedSyntaxList<TSyntax> separatedList = GetSeparatedList(list);

                if (separatedList.Any())
                    return FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
            }

            return ImmutableArray<TSyntax>.Empty;
        }

        protected virtual ImmutableArray<TSyntax> FindUnusedSyntax(
            TNode node,
            TListSyntax list,
            SeparatedSyntaxList<TSyntax> separatedList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int count = separatedList.Count(f => !f.IsMissing);

            if (count == 1)
            {
                TSyntax syntax = separatedList.First(f => !f.IsMissing);

                if (IsFixable(node, list, syntax, semanticModel, cancellationToken)
                    && !syntax.SpanContainsDirectives())
                {
                    return ImmutableArray.Create(syntax);
                }
            }
            else if (count > 1)
            {
                return FindFixableSyntaxes(node, list, separatedList, count, semanticModel, cancellationToken)
                    .Where(f => !f.SpanContainsDirectives())
                    .ToImmutableArray();
            }

            return ImmutableArray<TSyntax>.Empty;
        }

        private bool IsFixable(
            SyntaxNode node,
            TListSyntax list,
            TSyntax syntax,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            string name = GetIdentifier(syntax);

            var symbol = default(TSymbol);

            foreach (IdentifierNameSyntax identifierName in DescendantIdentifierNames(node, list.Span))
            {
                if (string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                {
                    if (EqualityComparer<TSymbol>.Default.Equals(symbol, default(TSymbol)))
                    {
                        symbol = (TSymbol)semanticModel.GetDeclaredSymbol(syntax, cancellationToken);

                        if (EqualityComparer<TSymbol>.Default.Equals(symbol, default(TSymbol)))
                            return false;
                    }

                    if (semanticModel.GetSymbol(identifierName, cancellationToken)?.Equals(symbol) == true)
                        return false;
                }
            }

            return true;
        }

        private IEnumerable<TSyntax> FindFixableSyntaxes(
            SyntaxNode node,
            TListSyntax list,
            SeparatedSyntaxList<TSyntax> separatedList,
            int count,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var names = new string[count];
            var infos = new SyntaxInfo<TSyntax, TSymbol>[count];

            {
                int i = 0;
                foreach (TSyntax syntax in separatedList)
                {
                    if (!syntax.IsMissing)
                    {
                        names[i] = GetIdentifier(syntax);
                        infos[i] = new SyntaxInfo<TSyntax, TSymbol>(syntax);

                        i++;
                    }
                }
            }

            foreach (IdentifierNameSyntax identifierName in DescendantIdentifierNames(node, list.Span))
            {
                string name = identifierName.Identifier.ValueText;

                for (int i = 0; i < names.Length; i++)
                {
                    if (names[i] != null
                        && string.Equals(names[i], name, StringComparison.Ordinal))
                    {
                        SyntaxInfo<TSyntax, TSymbol> info = infos[i];

                        if (EqualityComparer<TSymbol>.Default.Equals(info.Symbol, default(TSymbol)))
                        {
                            var symbol = (TSymbol)semanticModel.GetDeclaredSymbol(info.Syntax, cancellationToken);

                            if (!EqualityComparer<TSymbol>.Default.Equals(symbol, default(TSymbol)))
                            {
                                infos[i] = info.WithSymbol(symbol);
                            }
                            else
                            {
                                names[i] = null;
                            }
                        }

                        if (infos[i].Symbol?.Equals(semanticModel.GetSymbol(identifierName, cancellationToken)) == true)
                            names[i] = null;

                        break;
                    }
                }
            }

            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] != null)
                    yield return infos[i].Syntax;
            }
        }

        private static IEnumerable<IdentifierNameSyntax> DescendantIdentifierNames(SyntaxNode node, TextSpan excludedSpan)
        {
            foreach (SyntaxNode descendant in node.DescendantNodes(node.Span))
            {
                if (descendant.IsKind(SyntaxKind.IdentifierName)
                    && !excludedSpan.Contains(descendant.Span))
                {
                    yield return (IdentifierNameSyntax)descendant;
                }
            }
        }
    }
}
