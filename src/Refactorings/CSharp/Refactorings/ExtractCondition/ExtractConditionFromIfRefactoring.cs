// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractCondition
{
    internal abstract class ExtractConditionFromIfRefactoring
        : ExtractConditionRefactoring<IfStatementSyntax>
    {
        public override SyntaxKind StatementKind
        {
            get { return SyntaxKind.IfStatement; }
        }

        public override StatementSyntax GetStatement(IfStatementSyntax statement)
        {
            return statement.Statement;
        }

        public override IfStatementSyntax SetStatement(IfStatementSyntax statement, StatementSyntax newStatement)
        {
            return statement.WithStatement(newStatement);
        }
    }
}
