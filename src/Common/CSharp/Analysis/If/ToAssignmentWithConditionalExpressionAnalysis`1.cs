// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal abstract class ToAssignmentWithConditionalExpressionAnalysis<TStatement> : ToAssignmentWithConditionalExpressionAnalysis
        where TStatement : StatementSyntax
    {
        protected ToAssignmentWithConditionalExpressionAnalysis(
            TStatement statement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            SemanticModel semanticModel) : base(ifStatement, whenTrue, whenFalse, semanticModel)
        {
            Statement = statement;
        }

        public TStatement Statement { get; }
    }
}
