// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analyzers.ReturnTaskInsteadOfNull
{
    internal class ReturnTaskInsteadOfNullWalker : CSharpSyntaxWalker
    {
        public List<ExpressionSyntax> Expressions { get; private set; }

        public void Clear()
        {
            Expressions?.Clear();
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            ExpressionSyntax expression = node.Expression;

            if (expression?.WalkDownParentheses().IsKind(
                SyntaxKind.NullLiteralExpression,
                SyntaxKind.DefaultExpression) == true)
            {
                (Expressions ?? (Expressions = new List<ExpressionSyntax>())).Add(expression);
            }
            else
            {
                base.VisitReturnStatement(node);
            }
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
        }
    }
}
