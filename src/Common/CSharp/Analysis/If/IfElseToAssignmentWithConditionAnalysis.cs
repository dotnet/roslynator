// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfElseToAssignmentWithConditionAnalysis : IfAnalysis
    {
        public IfElseToAssignmentWithConditionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right,
            SemanticModel semanticModel,
            bool invert) : base(ifStatement, semanticModel)
        {
            Left = left;
            Right = right;
            Invert = invert;
        }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.IfElseToAssignmentWithCondition; }
        }

        public override string Title
        {
            get { return "Convert 'if' to assignment"; }
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right { get; }

        public bool Invert { get; }
    }
}
