// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxWalkers
{
    internal class SkipFunctionWalker : CSharpSyntaxWalker
    {
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
    }
}
