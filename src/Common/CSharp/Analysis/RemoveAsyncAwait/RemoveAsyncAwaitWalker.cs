// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.RemoveAsyncAwait
{
    internal class RemoveAsyncAwaitWalker : CSharpSyntaxNodeWalker
    {
        public HashSet<AwaitExpressionSyntax> AwaitExpressions { get; } = new HashSet<AwaitExpressionSyntax>();

        public bool StopOnFirstAwaitExpression { get; set; }

        public bool ShouldStop { get; private set; }

        protected override bool ShouldVisit
        {
            get { return !ShouldStop; }
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            ShouldStop = true;

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

                if (!ShouldStop)
                    AwaitExpressions.Add(awaitExpression);
            }
            else
            {
                ShouldStop = true;
                AwaitExpressions.Clear();
            }
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
        }

        [ThreadStatic]
        private static RemoveAsyncAwaitWalker _cachedInstance;

        public static RemoveAsyncAwaitWalker GetInstance()
        {
            RemoveAsyncAwaitWalker walker = _cachedInstance;

            if (walker != null)
            {
                _cachedInstance = null;
                return walker;
            }
            else
            {
                return new RemoveAsyncAwaitWalker();
            }
        }

        public static void Free(RemoveAsyncAwaitWalker walker)
        {
            walker.ShouldStop = false;
            walker.StopOnFirstAwaitExpression = false;
            walker.AwaitExpressions.Clear();

            _cachedInstance = walker;
        }
    }
}
