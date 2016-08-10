// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class UsingDirectiveSyntaxExtensions
    {
        public static bool IsSystem(this UsingDirectiveSyntax usingDirective)
        {
            if (usingDirective == null)
                throw new ArgumentNullException(nameof(usingDirective));

            string name = usingDirective.Name.ToString();

            return string.Equals(name, "System", StringComparison.Ordinal)
                || name.StartsWith("System.", StringComparison.Ordinal);
        }
    }
}
