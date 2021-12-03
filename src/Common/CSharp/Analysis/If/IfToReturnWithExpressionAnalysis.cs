// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfToReturnWithExpressionAnalysis : IfAnalysis
    {
        public IfToReturnWithExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            bool isYield,
            bool invert,
            SemanticModel semanticModel) : base(ifStatement, semanticModel)
        {
            Expression = expression;
            Invert = invert;
            IsYield = isYield;
        }

        public ExpressionSyntax Expression { get; }

        public bool Invert { get; }

        public bool IsYield { get; }

        public override IfAnalysisKind Kind
        {
            get
            {
                if (IsYield)
                    return IfAnalysisKind.IfElseToYieldReturnWithExpression;

                if (IfStatement.IsSimpleIf())
                    return IfAnalysisKind.IfReturnToReturnWithExpression;

                return IfAnalysisKind.IfElseToReturnWithExpression;
            }
        }

        public override string Title
        {
            get
            {
                if (IsYield)
                    return "Convert 'if' to 'yield return'";

                if (IfStatement.IsSimpleIf())
                    return "Convert 'if-return' to 'return'";

                return "Convert 'if' to 'return'";
            }
        }
    }
}
