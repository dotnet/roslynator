// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class IfStatementSyntaxExtensions
    {
        public static ElseClauseSyntax ParentElse(this IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            if (ifStatement.Parent == null)
                return null;

            if (!ifStatement.Parent.IsKind(SyntaxKind.ElseClause))
                return null;

            return (ElseClauseSyntax)ifStatement.Parent;
        }
    }
}
