// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.If
{
    internal abstract class ToAssignmentWithConditionalExpressionAnalysis : IfAnalysis
    {
        protected ToAssignmentWithConditionalExpressionAnalysis(
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            SemanticModel semanticModel) : base(ifStatement, semanticModel)
        {
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        public override string Title
        {
            get { return "Use conditional expression"; }
        }

        public ExpressionSyntax WhenTrue { get; }

        public ExpressionSyntax WhenFalse { get; }
    }
}
