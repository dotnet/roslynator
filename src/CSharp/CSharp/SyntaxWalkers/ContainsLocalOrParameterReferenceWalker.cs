// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal sealed class ContainsLocalOrParameterReferenceWalker : LocalOrParameterReferenceWalker
    {
        [ThreadStatic]
        private static ContainsLocalOrParameterReferenceWalker _cachedInstance;

        public ContainsLocalOrParameterReferenceWalker(
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            Symbol = symbol;
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public bool Result { get; private set; }

        public ISymbol Symbol { get; private set; }

        public SemanticModel SemanticModel { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        protected override bool ShouldVisit
        {
            get { return !Result; }
        }

        public static bool ContainsReference(
            SyntaxNode node,
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            bool result = false;
            ContainsLocalOrParameterReferenceWalker walker = null;

            try
            {
                walker = GetInstance(
                    symbol,
                    semanticModel,
                    cancellationToken);

                walker.Visit(node);

                result = walker.Result;
            }
            finally
            {
                if (walker != null)
                    Free(walker);
            }

            return result;
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (string.Equals(node.Identifier.ValueText, Symbol.Name, StringComparison.Ordinal)
                && SemanticModel.GetSymbol(node, CancellationToken)?.Equals(Symbol) == true)
            {
                Result = true;
            }
        }

        public void VisitList<TNode>(SyntaxList<TNode> statements) where TNode : SyntaxNode
        {
            VisitList(statements, 0);
        }

        public void VisitList<TNode>(SyntaxList<TNode> statements, int firstIndex) where TNode : SyntaxNode
        {
            VisitList(statements, firstIndex, statements.Count - 1);
        }

        public void VisitList<TNode>(SyntaxList<TNode> statements, int firstIndex, int lastIndex) where TNode : SyntaxNode
        {
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                Visit(statements[i]);

                if (Result)
                    break;
            }
        }

        public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements) where TNode : SyntaxNode
        {
            VisitList(statements, 0);
        }

        public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements, int firstIndex) where TNode : SyntaxNode
        {
            VisitList(statements, firstIndex, statements.Count - 1);
        }

        public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements, int firstIndex, int lastIndex) where TNode : SyntaxNode
        {
            for (int i = firstIndex; i <= lastIndex; i++)
            {
                Visit(statements[i]);

                if (Result)
                    break;
            }
        }

        public static ContainsLocalOrParameterReferenceWalker GetInstance(
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ContainsLocalOrParameterReferenceWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                walker.Symbol = symbol;
                walker.SemanticModel = semanticModel;
                walker.CancellationToken = cancellationToken;
                return walker;
            }

            return new ContainsLocalOrParameterReferenceWalker(symbol, semanticModel, cancellationToken);
        }

        public static void Free(ContainsLocalOrParameterReferenceWalker walker)
        {
            walker.Result = false;
            walker.Symbol = default;
            walker.SemanticModel = default;
            walker.CancellationToken = default;
            _cachedInstance = walker;
        }

        public static bool GetResultAndFree(ContainsLocalOrParameterReferenceWalker walker)
        {
            bool result = walker.Result;

            Free(walker);

            return result;
        }
    }
}
