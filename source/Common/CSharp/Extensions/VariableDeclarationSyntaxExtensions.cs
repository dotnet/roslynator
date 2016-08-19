// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class VariableDeclarationSyntaxExtensions
    {
        public static VariableDeclaratorSyntax SingleVariableOrDefault(this VariableDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

            if (variables.Count == 1)
                return variables[0];

            return null;
        }
    }
}
