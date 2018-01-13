// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.InlineDefinition
{
    internal struct ExpressionOrStatements
    {
        public ExpressionOrStatements(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public ExpressionOrStatements(SyntaxList<StatementSyntax> statements)
        {
            Expression = null;
            Statements = statements;
        }

        public ExpressionSyntax Expression { get; }

        public SyntaxList<StatementSyntax> Statements { get; }
    }
}
