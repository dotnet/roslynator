// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class AssignmentAndIfElseToAssignmentWithConditionalExpressionAnalysis : ToAssignmentWithConditionalExpressionAnalysis<ExpressionStatementSyntax>
    {
        internal AssignmentAndIfElseToAssignmentWithConditionalExpressionAnalysis(
            ExpressionStatementSyntax statement,
            ExpressionSyntax right,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            SemanticModel semanticModel) : base(statement, ifStatement, whenTrue, whenFalse, semanticModel)
        {
            Left = right;
        }

        public ExpressionSyntax Left { get; }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.AssignmentAndIfElseToAssignmentWithConditionalExpression; }
        }
    }
}
