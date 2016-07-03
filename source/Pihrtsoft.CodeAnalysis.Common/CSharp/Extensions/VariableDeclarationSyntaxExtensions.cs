// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class VariableDeclarationSyntaxExtensions
    {
        public static VariableDeclarationSyntax WithVariable(
            this VariableDeclarationSyntax fieldDeclaration,
            VariableDeclaratorSyntax variableDeclarator)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return fieldDeclaration.WithVariables(SingletonSeparatedList(variableDeclarator));
        }
    }
}
