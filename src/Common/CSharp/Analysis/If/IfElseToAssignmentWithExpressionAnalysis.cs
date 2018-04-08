// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class IfElseToAssignmentWithExpressionAnalysis : IfAnalysis
    {
        public IfElseToAssignmentWithExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionStatementSyntax expressionStatement,
            SemanticModel semanticModel) : base(ifStatement, semanticModel)
        {
            ExpressionStatement = expressionStatement;
        }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.IfElseToAssignmentWithExpression; }
        }

        public override string Title
        {
            get { return "Replace if-else with assignment"; }
        }

        public ExpressionStatementSyntax ExpressionStatement { get; }
    }
}