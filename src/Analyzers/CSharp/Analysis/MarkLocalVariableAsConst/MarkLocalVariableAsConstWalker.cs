// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.MarkLocalVariableAsConst
{
    internal class MarkLocalVariableAsConstWalker : AssignedExpressionWalker
    {
        public HashSet<string> Identifiers { get; } = new HashSet<string>();

        public bool IsMatch { get; private set; }

        public void Reset()
        {
            Identifiers.Clear();
            IsMatch = false;
        }

        public override void Visit(SyntaxNode node)
        {
            if (!IsMatch)
                base.Visit(node);
        }

        public override void VisitAssignedExpression(ExpressionSyntax expression)
        {
            if (expression is IdentifierNameSyntax identifierName
                && Identifiers.Contains(identifierName.Identifier.ValueText))
            {
                IsMatch = true;
            }
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.AddressOfExpression
                && node.Operand is IdentifierNameSyntax identifierName)
            {
                if (Identifiers.Contains(identifierName.Identifier.ValueText))
                    IsMatch = true;
            }
            else
            {
                base.VisitPrefixUnaryExpression(node);
            }
        }
    }
}
