// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfElseToAssignmentWithCoalesceExpressionAnalysis : IfAnalysis
    {
        public IfElseToAssignmentWithCoalesceExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right1,
            ExpressionSyntax right2,
            SemanticModel semanticModel) : base(ifStatement, semanticModel)
        {
            Left = left;
            Right1 = right1;
            Right2 = right2;
        }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.IfElseToAssignmentWithCoalesceExpression; }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right1 { get; }

        public ExpressionSyntax Right2 { get; }
    }
}