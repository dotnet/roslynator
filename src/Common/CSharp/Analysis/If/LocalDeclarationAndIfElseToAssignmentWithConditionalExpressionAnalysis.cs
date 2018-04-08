// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal sealed class LocalDeclarationAndIfElseToAssignmentWithConditionalExpressionAnalysis : ToAssignmentWithConditionalExpressionAnalysis<LocalDeclarationStatementSyntax>
    {
        internal LocalDeclarationAndIfElseToAssignmentWithConditionalExpressionAnalysis(
            LocalDeclarationStatementSyntax statement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            SemanticModel semanticModel) : base(statement, ifStatement, whenTrue, whenFalse, semanticModel)
        {
        }

        public override IfAnalysisKind Kind
        {
            get { return IfAnalysisKind.LocalDeclarationAndIfElseAssignmentWithConditionalExpression; }
        }
    }
}
