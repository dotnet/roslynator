// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class EnumDeclarationExtensions
    {
        public static EnumDeclarationSyntax WithoutSemicolonToken(this EnumDeclarationSyntax enumDeclaration)
        {
            if (enumDeclaration == null)
                throw new ArgumentNullException(nameof(enumDeclaration));

            return enumDeclaration.WithSemicolonToken(CSharpFactory.NoneToken());
        }
    }
}
