// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.SyntaxWalkers;

namespace Roslynator.CSharp.Analysis.MarkLocalVariableAsConst
{
    internal class MarkLocalVariableAsConstWalker : AssignedExpressionWalker
    {
        [ThreadStatic]
        private static MarkLocalVariableAsConstWalker _cachedInstance;

        public HashSet<string> Identifiers { get; } = new HashSet<string>();

        public bool Result { get; set; }

        protected override bool ShouldVisit
        {
            get { return !Result; }
        }

        public override void VisitAssignedExpression(ExpressionSyntax expression)
        {
            if (expression is IdentifierNameSyntax identifierName
                && Identifiers.Contains(identifierName.Identifier.ValueText))
            {
                Result = true;
            }
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.AddressOfExpression
                && node.Operand is IdentifierNameSyntax identifierName)
            {
                if (Identifiers.Contains(identifierName.Identifier.ValueText))
                    Result = true;
            }
            else
            {
                base.VisitPrefixUnaryExpression(node);
            }
        }

        public static MarkLocalVariableAsConstWalker GetInstance()
        {
            MarkLocalVariableAsConstWalker walker = _cachedInstance;

            if (walker != null)
            {
                Debug.Assert(walker.Identifiers.Count == 0);
                Debug.Assert(!walker.Result);

                _cachedInstance = null;
                return walker;
            }

            return new MarkLocalVariableAsConstWalker();
        }

        public static void Free(MarkLocalVariableAsConstWalker walker)
        {
            walker.Identifiers.Clear();
            walker.Result = false;

            _cachedInstance = walker;
        }
    }
}
