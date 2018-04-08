// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceInterpolatedStringWithStringLiteralRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            InterpolatedStringExpressionSyntax interpolatedString,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            string s = UnescapeBraces(interpolatedString.ToString().Substring(1));

            var newNode = (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(s)
                .WithTriviaFrom(interpolatedString);

            return document.ReplaceNodeAsync(interpolatedString, newNode, cancellationToken);
        }

        private static string UnescapeBraces(string s)
        {
            var sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '{')
                {
                    if (i < s.Length - 1 && s[i + 1] == '{')
                    {
                        sb.Append('{');
                        i++;
                        continue;
                    }
                }
                else if (s[i] == '}')
                {
                    if (i < s.Length - 1 && s[i + 1] == '}')
                    {
                        sb.Append('}');
                        i++;
                        continue;
                    }
                }

                sb.Append(s[i]);
            }

            return sb.ToString();
        }
    }
}
