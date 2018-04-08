// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfElseToReturnWithConditionalExpressionAnalysis : IfToReturnWithConditionalExpressionAnalysis
    {
        public IfElseToReturnWithConditionalExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            SemanticModel semanticModel) : base(ifStatement, expression1, expression2, semanticModel)
        {
        }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.IfElseToReturnWithConditionalExpression; }
        }

        public override string Title
        {
            get { return "Use conditional expression"; }
        }
    }
}