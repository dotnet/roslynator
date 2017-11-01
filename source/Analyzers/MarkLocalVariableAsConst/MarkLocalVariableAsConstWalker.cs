// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analyzers.MarkLocalVariableAsConst
{
    internal class MarkLocalVariableAsConstWalker : AssignedExpressionWalker
    {
        public HashSet<string> Assigned { get; private set; }

        public void Clear()
        {
            Assigned?.Clear();
        }

        public override void VisitAssignedExpression(ExpressionSyntax expression)
        {
            if (expression is IdentifierNameSyntax identifierName)
            {
                (Assigned ?? (Assigned = new HashSet<string>())).Add(identifierName.Identifier.ValueText);
            }
        }
    }
}
