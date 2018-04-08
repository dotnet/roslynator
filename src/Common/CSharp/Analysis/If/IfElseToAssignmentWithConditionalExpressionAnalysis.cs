// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfElseToAssignmentWithConditionalExpressionAnalysis : ToAssignmentWithConditionalExpressionAnalysis
    {
        internal IfElseToAssignmentWithConditionalExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right1,
            ExpressionSyntax right2,
            SemanticModel semanticModel) : base(ifStatement, right1, right2, semanticModel)
        {
            Left = left;
        }

        public ExpressionSyntax Left { get; }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.IfElseToAssignmentWithConditionalExpression; }
        }
    }
}
