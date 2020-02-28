// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
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

        public bool Result { get; set; }

        public ISymbol Symbol { get; private set; }

        public SemanticModel SemanticModel { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        protected override bool ShouldVisit
        {
            get { return !Result; }
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

        public void VisitList<TNode>(SyntaxList<TNode> statements, int startIndex) where TNode : SyntaxNode
        {
            VisitList(statements, startIndex, statements.Count - startIndex);
        }

        public void VisitList<TNode>(SyntaxList<TNode> statements, int startIndex, int count) where TNode : SyntaxNode
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > statements.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (startIndex + count > statements.Count)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            for (int i = startIndex; i < startIndex + count; i++)
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

        public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements, int startIndex) where TNode : SyntaxNode
        {
            VisitList(statements, startIndex, statements.Count - startIndex);
        }

        public void VisitList<TNode>(SeparatedSyntaxList<TNode> statements, int startIndex, int count) where TNode : SyntaxNode
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex > statements.Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (startIndex + count > statements.Count)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
                return;

            for (int i = startIndex; i < startIndex + count; i++)
            {
                Visit(statements[i]);

                if (Result)
                    break;
            }
        }

        public static ContainsLocalOrParameterReferenceWalker GetInstance(
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            ContainsLocalOrParameterReferenceWalker walker = _cachedInstance;

            if (walker != null)
            {
                Debug.Assert(walker.Symbol == null);
                Debug.Assert(walker.SemanticModel == null);
                Debug.Assert(walker.CancellationToken == default);

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
            walker.Symbol = null;
            walker.SemanticModel = null;
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
