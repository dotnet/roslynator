// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfToReturnWithCoalesceExpressionAnalysis : IfAnalysis
    {
        public IfToReturnWithCoalesceExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SemanticModel semanticModel,
            bool isYield) : base(ifStatement, semanticModel)
        {
            Left = left;
            Right = right;
            IsYield = isYield;
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right { get; }

        public bool IsYield { get; }

        public override IfAnalysisKind Kind
        {
            get
            {
                if (IsYield)
                    return IfAnalysisKind.IfElseToYieldReturnWithCoalesceExpression;

                if (IfStatement.IsSimpleIf())
                    return IfAnalysisKind.IfReturnToReturnWithCoalesceExpression;

                return IfAnalysisKind.IfElseToReturnWithCoalesceExpression;
            }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }
    }
}