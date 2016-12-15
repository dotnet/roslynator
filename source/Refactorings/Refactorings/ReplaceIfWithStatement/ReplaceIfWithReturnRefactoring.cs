// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceIfWithStatement
{
    internal class ReplaceIfWithReturnRefactoring : ReplaceIfWithStatementRefactoring<ReturnStatementSyntax>
    {
        public override SyntaxKind StatementKind
        {
            get { return SyntaxKind.ReturnStatement; }
        }

        public override string StatementTitle
        {
            get { return "return"; }
        }

        public override ReturnStatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return ReturnStatement(expression);
        }

        public override ExpressionSyntax GetExpression(ReturnStatementSyntax statement)
        {
            return statement.Expression;
        }
    }
}
