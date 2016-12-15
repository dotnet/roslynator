// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceIfWithStatement
{
    internal class ReplaceIfWithYieldReturnRefactoring : ReplaceIfWithStatementRefactoring<YieldStatementSyntax>
    {
        public override SyntaxKind StatementKind
        {
            get { return SyntaxKind.YieldReturnStatement; }
        }

        public override string StatementTitle
        {
            get { return "yield return"; }
        }

        public override YieldStatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return YieldReturnStatement(expression);
        }

        public override ExpressionSyntax GetExpression(YieldStatementSyntax statement)
        {
            return statement.Expression;
        }
    }
}
