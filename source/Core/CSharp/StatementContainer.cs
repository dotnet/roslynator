// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public static class StatementContainer
    {
        public static bool TryCreate(StatementSyntax statement, out IStatementContainer container)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxNode parent = statement.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        container = new BlockStatementContainer((BlockSyntax)parent);
                        return true;
                    }
                case SyntaxKind.SwitchSection:
                    {
                        container = new SwitchSectionStatementContainer((SwitchSectionSyntax)parent);
                        return true;
                    }
                default:
                    {
                        container = null;
                        return false;
                    }
            }
        }

        public static IStatementContainer Create(StatementSyntax statement)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            SyntaxNode parent = statement.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.Block:
                    return new BlockStatementContainer((BlockSyntax)parent);
                case SyntaxKind.SwitchSection:
                    return new SwitchSectionStatementContainer((SwitchSectionSyntax)parent);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
