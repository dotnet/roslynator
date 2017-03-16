// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Syntax;
using Roslynator.Extensions;

namespace Roslynator.CSharp
{
    public static class IfElseChain
    {
        public static IEnumerable<IfStatementOrElseClause> GetChain(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            yield return ifStatement;

            while (true)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    StatementSyntax statement = elseClause.Statement;

                    if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                    {
                        ifStatement = (IfStatementSyntax)statement;
                        yield return ifStatement;
                    }
                    else
                    {
                        yield return elseClause;
                        yield break;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public static IfStatementSyntax GetTopmostIf(ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            var ifStatement = elseClause.Parent as IfStatementSyntax;

            if (ifStatement != null)
                return GetTopmostIf(ifStatement);

            return null;
        }

        public static IfStatementSyntax GetTopmostIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            while (true)
            {
                IfStatementSyntax parentIf = GetPreviousIf(ifStatement);

                if (parentIf != null)
                {
                    ifStatement = parentIf;
                }
                else
                {
                    break;
                }
            }

            return ifStatement;
        }

        public static bool IsTopmostIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            return !ifStatement.IsParentKind(SyntaxKind.ElseClause);
        }

        public static IfStatementSyntax GetNextIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            StatementSyntax statement = ifStatement.Else?.Statement;

            if (statement?.IsKind(SyntaxKind.IfStatement) == true)
                return (IfStatementSyntax)statement;

            return null;
        }

        public static IfStatementSyntax GetPreviousIf(IfStatementSyntax ifStatement)
        {
            if (ifStatement == null)
                throw new ArgumentNullException(nameof(ifStatement));

            SyntaxNode parent = ifStatement.Parent;

            if (parent?.IsKind(SyntaxKind.ElseClause) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.IfStatement) == true)
                    return (IfStatementSyntax)parent;
            }

            return null;
        }

        public static bool IsEndOfChain(ElseClauseSyntax elseClause)
        {
            if (elseClause == null)
                throw new ArgumentNullException(nameof(elseClause));

            return elseClause.Statement?.IsKind(SyntaxKind.IfStatement) != true;
        }
    }
}
