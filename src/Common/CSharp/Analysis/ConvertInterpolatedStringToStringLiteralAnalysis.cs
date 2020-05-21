// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    internal static class ConvertInterpolatedStringToStringLiteralAnalysis
    {
        public static bool IsFixable(InterpolatedStringExpressionSyntax interpolatedString)
        {
            SyntaxList<InterpolatedStringContentSyntax> contents = interpolatedString.Contents;

            return IsFixable(contents);
        }

        public static bool IsFixable(SyntaxList<InterpolatedStringContentSyntax> contents)
        {
            return !contents.Any()
                || contents.SingleOrDefault(shouldThrow: false)?.Kind() == SyntaxKind.InterpolatedStringText;
        }
    }
}
