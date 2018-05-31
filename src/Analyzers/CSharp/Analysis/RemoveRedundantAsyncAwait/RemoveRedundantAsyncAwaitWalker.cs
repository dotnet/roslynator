// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.RemoveRedundantAsyncAwait
{
    internal class RemoveRedundantAsyncAwaitWalker : SkipFunctionWalker
    {
        private bool _shouldStop;

        public HashSet<AwaitExpressionSyntax> AwaitExpressions { get; } = new HashSet<AwaitExpressionSyntax>();

        public TextSpan Span { get; private set; }

        public bool StopOnFirstAwaitExpression { get; private set; }

        public void SetValues(TextSpan span, bool stopOnFirstAwaitExpression = false)
        {
            _shouldStop = false;
            Span = span;
            StopOnFirstAwaitExpression = stopOnFirstAwaitExpression;
            AwaitExpressions.Clear();
        }

        public void Clear()
        {
            SetValues(default(TextSpan));
        }

        public override void Visit(SyntaxNode node)
        {
            if (_shouldStop)
                return;

            TextSpan span = node.Span;

            if (Span.OverlapsWith(span)
                || (span.IsEmpty && Span.IntersectsWith(span)))
            {
                base.Visit(node);
            }
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            _shouldStop = true;

            if (StopOnFirstAwaitExpression)
            {
                Debug.Assert(AwaitExpressions.Count == 0);

                AwaitExpressions.Add(node);
            }
            else
            {
                AwaitExpressions.Clear();
            }
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            Debug.Assert(!StopOnFirstAwaitExpression);

            if (node.Expression is AwaitExpressionSyntax awaitExpression)
            {
                Visit(awaitExpression.Expression);

                if (!_shouldStop)
                    AwaitExpressions.Add(awaitExpression);
            }
            else
            {
                _shouldStop = true;
                AwaitExpressions.Clear();
            }
        }
    }
}
