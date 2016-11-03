// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal class StringLiteralChain
    {
        private const string Quote = "\"";
        private const string AmpersandQuote = "@" + Quote;

        public StringLiteralChain(BinaryExpressionSyntax addExpression)
        {
            AddExpression = addExpression;

            List<LiteralExpressionSyntax> literals = GetLiterals(addExpression);

            for (int i = 0; i < literals.Count; i++)
            {
                if (literals[i].IsVerbatimStringLiteral())
                    IsAnyVerbatim = true;
                else
                    IsAllVerbatim = false;
            }

            if (IsAllVerbatim)
                IsAnyVerbatim = false;

            Literals = ImmutableArray.CreateRange(literals);
        }

        private static List<LiteralExpressionSyntax> GetLiterals(BinaryExpressionSyntax addExpression)
        {
            var literals = new List<LiteralExpressionSyntax>();

            var addExpressions = new Stack<BinaryExpressionSyntax>();
            addExpressions.Push(addExpression);

            while (addExpressions.Count > 0)
            {
                addExpression = addExpressions.Pop();

                literals.Add((LiteralExpressionSyntax)addExpression.Right);

                if (addExpression.Left.IsKind(SyntaxKind.StringLiteralExpression))
                    literals.Add((LiteralExpressionSyntax)addExpression.Left);
                else
                    addExpressions.Push((BinaryExpressionSyntax)addExpression.Left);
            }

            return literals;
        }

        public ImmutableArray<LiteralExpressionSyntax> Literals { get; }

        public BinaryExpressionSyntax AddExpression { get; }

        public bool IsAllVerbatim { get; } = true;

        public bool IsAnyVerbatim { get; }

        public static bool IsStringLiteralChain(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.IsKind(SyntaxKind.AddExpression)
                && binaryExpression.Right?.IsKind(SyntaxKind.StringLiteralExpression) == true)
            {
                switch (binaryExpression.Left?.Kind())
                {
                    case SyntaxKind.StringLiteralExpression:
                        return true;
                    case SyntaxKind.AddExpression:
                        return IsStringLiteralChain((BinaryExpressionSyntax)binaryExpression.Left);
                }
            }

            return false;
        }

        public LiteralExpressionSyntax Merge()
        {
            var sb = new StringBuilder();

            for (int i = Literals.Length - 1; i >= 0; i--)
            {
                if (IsAnyVerbatim && Literals[i].IsVerbatimStringLiteral())
                {
                    sb.Append(EscapeQuotes(Literals[i].Token.ValueText));
                }
                else
                {
                    sb.Append(GetInnerText(Literals[i].Token.Text));
                }
            }

            return ParseExpression(sb.ToString(), start: (IsAllVerbatim) ? AmpersandQuote : Quote);
        }

        public LiteralExpressionSyntax MergeMultiline()
        {
            var sb = new StringBuilder();

            for (int i = Literals.Length - 1; i >= 0; i--)
            {
                string s = DoubleQuotes(Literals[i].Token.ValueText);

                int charCount = 0;

                if (s.Length > 0
                    && s[s.Length - 1] == '\n')
                {
                    if (s.Length > 1
                        && s[s.Length - 2] == '\r')
                    {
                        charCount = 2;
                    }
                    else
                    {
                        charCount = 1;
                    }
                }

                sb.Append(s, 0, s.Length - charCount);

                if (charCount > 0
                    || (i > 0
                        && Literals[i].GetSpanEndLine() != Literals[i - 1].GetSpanStartLine()))
                {
                    sb.Append("\r\n");
                }
            }

            return ParseExpression(sb.ToString(), start: AmpersandQuote);
        }

        private LiteralExpressionSyntax ParseExpression(string text, string start)
        {
            var literal = (LiteralExpressionSyntax)SyntaxFactory.ParseExpression(start + text + Quote);

            return literal
                .WithLeadingTrivia(Literals[Literals.Length - 1].GetLeadingTrivia())
                .WithTrailingTrivia(Literals[0].GetTrailingTrivia());
        }

        private static string EscapeQuotes(string value)
        {
            return value.Replace(Quote, @"\" + Quote);
        }

        private static string DoubleQuotes(string value)
        {
            return value.Replace(Quote, Quote + Quote);
        }

        private static string GetInnerText(string s)
        {
            if (s[0] == '@')
                return s.Substring(2, s.Length - 3);

            return s.Substring(1, s.Length - 2);
        }
    }
}
