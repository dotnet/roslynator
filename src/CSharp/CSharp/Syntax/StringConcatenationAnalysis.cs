// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    [DebuggerDisplay("{Flags}")]
    internal readonly struct StringConcatenationAnalysis
    {
        private StringConcatenationAnalysis(StringConcatenationFlags flags)
        {
            Flags = flags;
        }

        private StringConcatenationFlags Flags { get; }

        public bool ContainsNonStringLiteral => (Flags & StringConcatenationFlags.ContainsNonStringLiteral) != 0;

        public bool ContainsStringLiteral => (Flags & StringConcatenationFlags.ContainsStringLiteral) != 0;

        public bool ContainsInterpolatedString => (Flags & StringConcatenationFlags.ContainsInterpolatedString) != 0;

        public bool ContainsNonVerbatimExpression => (Flags & StringConcatenationFlags.ContainsNonVerbatimExpression) != 0;

        public static StringConcatenationAnalysis Create(in StringConcatenationExpressionInfo stringConcatenation)
        {
            var flags = StringConcatenationFlags.None;

            foreach (ExpressionSyntax expression in stringConcatenation.AsChain().Reverse())
            {
                StringLiteralExpressionInfo stringLiteral = SyntaxInfo.StringLiteralExpressionInfo(expression);

                if (stringLiteral.Success)
                {
                    if (stringLiteral.IsVerbatim)
                    {
                        flags |= StringConcatenationFlags.ContainsVerbatimStringLiteral;
                    }
                    else
                    {
                        flags |= StringConcatenationFlags.ContainsRegularStringLiteral;
                    }
                }
                else if (expression.Kind() == SyntaxKind.InterpolatedStringExpression)
                {
                    if (((InterpolatedStringExpressionSyntax)expression).IsVerbatim())
                    {
                        flags |= StringConcatenationFlags.ContainsVerbatimInterpolatedString;
                    }
                    else
                    {
                        flags |= StringConcatenationFlags.ContainsRegularInterpolatedString;
                    }
                }
                else
                {
                    flags |= StringConcatenationFlags.ContainsUnspecifiedExpression;
                }
            }

            return new StringConcatenationAnalysis(flags);
        }

        [Flags]
        private enum StringConcatenationFlags
        {
            None = 0,
            ContainsUnspecifiedExpression = 1,
            ContainsRegularStringLiteral = 2,
            ContainsVerbatimStringLiteral = 4,
            ContainsStringLiteral = ContainsRegularStringLiteral | ContainsVerbatimStringLiteral,
            ContainsRegularInterpolatedString = 8,
            ContainsNonVerbatimExpression = ContainsRegularStringLiteral | ContainsRegularInterpolatedString,
            ContainsVerbatimInterpolatedString = 16,
            ContainsVerbatimExpression = ContainsVerbatimStringLiteral | ContainsVerbatimInterpolatedString,
            ContainsInterpolatedString = ContainsRegularInterpolatedString | ContainsVerbatimInterpolatedString,
            ContainsNonStringLiteral = ContainsInterpolatedString | ContainsUnspecifiedExpression,
        }
    }
}
