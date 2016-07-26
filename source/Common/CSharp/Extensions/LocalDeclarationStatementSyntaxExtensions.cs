// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class LocalDeclarationStatementSyntaxExtensions
    {
        public static LocalDeclarationStatementSyntax WithSemicolonToken(this LocalDeclarationStatementSyntax localDeclarationStatement)
        {
            if (localDeclarationStatement == null)
                throw new ArgumentNullException(nameof(localDeclarationStatement));

            return localDeclarationStatement.WithSemicolonToken(SemicolonToken());
        }
    }
}
