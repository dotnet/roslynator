// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveAsyncAwait
{
    internal readonly struct RemoveAsyncAwaitResult
    {
        public RemoveAsyncAwaitResult(RemoveAsyncAwaitWalker walker)
        {
            Walker = walker;
            AwaitExpression = null;
        }

        public RemoveAsyncAwaitResult(AwaitExpressionSyntax awaitExpression)
        {
            AwaitExpression = awaitExpression;
            Walker = null;
        }

        public bool Success
        {
            get { return AwaitExpression != null || Walker?.AwaitExpressions.Count > 0; }
        }

        public AwaitExpressionSyntax AwaitExpression { get; }

        public RemoveAsyncAwaitWalker Walker { get; }
    }
}
