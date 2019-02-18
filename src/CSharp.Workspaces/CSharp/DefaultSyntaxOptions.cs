// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    [Flags]
    internal enum DefaultSyntaxOptions
    {
        /// <summary>
        /// No option specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use <see cref="SyntaxKind.DefaultExpression"/> or <see cref="SyntaxKind.DefaultLiteralExpression"/>.
        /// </summary>
        UseDefault = 1,

        /// <summary>
        /// Prefer <see cref="SyntaxKind.DefaultLiteralExpression"/> to <see cref="SyntaxKind.DefaultExpression"/>.
        /// </summary>
        PreferDefaultLiteral = 2,
    }
}
