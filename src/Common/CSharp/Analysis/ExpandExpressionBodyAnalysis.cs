// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class ExpandExpressionBodyAnalysis
    {
        public static bool IsFixable(ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            SyntaxNode parent = arrowExpressionClause.Parent;

            return parent != null
                && CSharpFacts.CanHaveExpressionBody(parent.Kind());
        }
    }
}
