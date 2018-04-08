// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis
{
    internal enum IfAnalysisKind
    {
        IfElseToAssignmentWithCoalesceExpression,
        IfElseToAssignmentWithConditionalExpression,
        IfElseToAssignmentWithExpression,
        IfElseToAssignmentWithCondition,

        AssignmentAndIfElseToAssignmentWithConditionalExpression,
        LocalDeclarationAndIfElseAssignmentWithConditionalExpression,

        IfElseToReturnWithCoalesceExpression,
        IfElseToReturnWithConditionalExpression,
        IfElseToReturnWithBooleanExpression,
        IfElseToReturnWithExpression,

        IfElseToYieldReturnWithCoalesceExpression,
        IfElseToYieldReturnWithConditionalExpression,
        IfElseToYieldReturnWithBooleanExpression,
        IfElseToYieldReturnWithExpression,

        IfReturnToReturnWithCoalesceExpression,
        IfReturnToReturnWithConditionalExpression,
        IfReturnToReturnWithBooleanExpression,
        IfReturnToReturnWithExpression,
    }
}
