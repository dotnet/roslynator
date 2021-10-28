// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    /// <summary>
    /// Defines how a syntax representing a default value of a type should look like.
    /// </summary>
    [Flags]
    public enum DefaultSyntaxOptions
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
        /// Allow <see cref="SyntaxKind.DefaultLiteralExpression"/> instead of <see cref="SyntaxKind.DefaultExpression"/>.
        /// </summary>
        AllowDefaultLiteral = 2,
    }
}
