// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class VariableDeclaratorSyntaxExtensions
    {
        public static VariableDeclaratorSyntax WithInitializer(
            this VariableDeclaratorSyntax variableDeclarator,
            ExpressionSyntax value)
        {
            if (variableDeclarator == null)
                throw new ArgumentNullException(nameof(variableDeclarator));

            return variableDeclarator.WithInitializer(EqualsValueClause(value));
        }
    }
}
