// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.ReturnTaskInsteadOfNull
{
    internal class ReturnTaskInsteadOfNullWalker : StatementWalker
    {
        [ThreadStatic]
        private static ReturnTaskInsteadOfNullWalker _cachedInstance;

        public List<ExpressionSyntax> Expressions { get; private set; }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression?.WalkDownParentheses().IsKind(
                SyntaxKind.NullLiteralExpression,
                SyntaxKind.DefaultExpression,
                SyntaxKind.DefaultLiteralExpression,
                SyntaxKind.ConditionalAccessExpression) == true)
            {
                (Expressions ??= new List<ExpressionSyntax>()).Add(expression);
            }
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
        }

        public static ReturnTaskInsteadOfNullWalker GetInstance()
        {
            ReturnTaskInsteadOfNullWalker walker = _cachedInstance;

            if (walker != null)
            {
                Debug.Assert(walker.Expressions == null || walker.Expressions.Count == 0);

                _cachedInstance = null;
                return walker;
            }

            return new ReturnTaskInsteadOfNullWalker();
        }

        public static void Free(ReturnTaskInsteadOfNullWalker walker)
        {
            walker.Expressions?.Clear();

            _cachedInstance = walker;
        }
    }
}
