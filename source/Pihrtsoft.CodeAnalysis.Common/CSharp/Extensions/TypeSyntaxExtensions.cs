// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class TypeSyntaxExtensions
    {
        public static bool IsVoid(this TypeSyntax type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsKind(SyntaxKind.PredefinedType)
                && ((PredefinedTypeSyntax)type).Keyword.IsKind(SyntaxKind.VoidKeyword);
        }
    }
}
