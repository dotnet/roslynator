// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class RefactoringUtility
    {
        public static IEnumerable<AttributeListSyntax> SplitAttributes(AttributeListSyntax attributeList)
        {
            SeparatedSyntaxList<AttributeSyntax> attributes = attributeList.Attributes;

            for (int i = 0; i < attributes.Count; i++)
            {
                AttributeListSyntax list = AttributeList(attributes[i]);

                if (i == 0)
                    list = list.WithLeadingTrivia(attributeList.GetLeadingTrivia());

                if (i == attributes.Count - 1)
                    list = list.WithTrailingTrivia(attributeList.GetTrailingTrivia());

                yield return list;
            }
        }

        public static AttributeListSyntax MergeAttributes(IList<AttributeListSyntax> lists)
        {
            AttributeListSyntax list = lists[0];

            for (int i = 1; i < lists.Count; i++)
                list = list.AddAttributes(lists[i].Attributes.ToArray());

            return list
                .WithLeadingTrivia(lists[0].GetLeadingTrivia())
                .WithTrailingTrivia(lists.Last().GetTrailingTrivia());
        }

        public static InvocationExpressionSyntax ChangeInvokedMethodName(InvocationExpressionSyntax invocation, string newName)
        {
            ExpressionSyntax expression = invocation.Expression;

            if (expression != null)
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.SimpleMemberAccessExpression)
                {
                    var memberAccess = (MemberAccessExpressionSyntax)expression;
                    SimpleNameSyntax simpleName = memberAccess.Name;

                    if (simpleName != null)
                    {
                        SimpleNameSyntax newSimpleName = ChangeName(simpleName);

                        return invocation.WithExpression(memberAccess.WithName(newSimpleName));
                    }
                }
                else if (kind == SyntaxKind.MemberBindingExpression)
                {
                    var memberBinding = (MemberBindingExpressionSyntax)expression;
                    SimpleNameSyntax simpleName = memberBinding.Name;

                    if (simpleName != null)
                    {
                        SimpleNameSyntax newSimpleName = ChangeName(simpleName);

                        return invocation.WithExpression(memberBinding.WithName(newSimpleName));
                    }
                }
                else
                {
                    if (expression is SimpleNameSyntax simpleName)
                    {
                        SimpleNameSyntax newSimpleName = ChangeName(simpleName);

                        return invocation.WithExpression(newSimpleName);
                    }

                    Debug.Fail(kind.ToString());
                }
            }

            return invocation;

            SimpleNameSyntax ChangeName(SimpleNameSyntax simpleName)
            {
                return simpleName.WithIdentifier(
                    Identifier(
                        simpleName.GetLeadingTrivia(),
                        newName,
                        simpleName.GetTrailingTrivia()));
            }
        }

        public static bool ContainsOutArgumentWithLocal(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (SyntaxNode node in expression.DescendantNodes())
            {
                if (node.Kind() == SyntaxKind.Argument)
                {
                    var argument = (ArgumentSyntax)node;

                    if (argument.RefOrOutKeyword.Kind() == SyntaxKind.OutKeyword)
                    {
                        ExpressionSyntax argumentExpression = argument.Expression;

                        if (argumentExpression?.IsMissing == false
                            && semanticModel.GetSymbol(argumentExpression, cancellationToken)?.Kind == SymbolKind.Local)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static (SyntaxKind contentKind, string methodName, ImmutableArray<ArgumentSyntax> arguments)
            ConvertInterpolatedStringToStringBuilderMethod(InterpolatedStringContentSyntax content, bool isVerbatim)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            SyntaxKind kind = content.Kind();

            switch (kind)
            {
                case SyntaxKind.Interpolation:
                    {
                        var interpolation = (InterpolationSyntax)content;

                        InterpolationAlignmentClauseSyntax alignmentClause = interpolation.AlignmentClause;
                        InterpolationFormatClauseSyntax formatClause = interpolation.FormatClause;

                        if (alignmentClause != null
                            || formatClause != null)
                        {
                            StringBuilder sb = StringBuilderCache.GetInstance();

                            sb.Append("\"{0");

                            if (alignmentClause != null)
                            {
                                sb.Append(',');
                                sb.Append(alignmentClause.Value.ToString());
                            }

                            if (formatClause != null)
                            {
                                sb.Append(':');
                                sb.Append(formatClause.FormatStringToken.Text);
                            }

                            sb.Append("}\"");

                            ExpressionSyntax expression = ParseExpression(StringBuilderCache.GetStringAndFree(sb));

                            return (kind, "AppendFormat", ImmutableArray.Create(Argument(expression), Argument(interpolation.Expression)));
                        }
                        else
                        {
                            return (kind, "Append", ImmutableArray.Create(Argument(interpolation.Expression)));
                        }
                    }
                case SyntaxKind.InterpolatedStringText:
                    {
                        var interpolatedStringText = (InterpolatedStringTextSyntax)content;

                        string text = interpolatedStringText.TextToken.Text;

                        text = (isVerbatim)
                            ? "@\"" + text + "\""
                            : "\"" + text + "\"";

                        ExpressionSyntax stringLiteral = ParseExpression(text);

                        return (kind, "Append", ImmutableArray.Create(Argument(stringLiteral)));
                    }
                default:
                    {
                        throw new ArgumentException("", nameof(content));
                    }
            }
        }

        public static ExpressionSyntax RemoveInvocation(InvocationExpressionSyntax invocation)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;

            ArgumentListSyntax argumentList = invocation.ArgumentList;

            SyntaxToken closeParen = argumentList.CloseParenToken;

            return memberAccess.Expression
                .AppendToTrailingTrivia(
                    memberAccess.OperatorToken.GetAllTrivia()
                        .Concat(memberAccess.Name.GetLeadingAndTrailingTrivia())
                        .Concat(argumentList.OpenParenToken.GetAllTrivia())
                        .Concat(closeParen.LeadingTrivia)
                        .ToSyntaxTriviaList()
                        .EmptyIfWhitespace()
                        .AddRange(closeParen.TrailingTrivia));
        }

        public static BlockSyntax RemoveUnsafeContext(UnsafeStatementSyntax unsafeStatement)
        {
            SyntaxToken keyword = unsafeStatement.UnsafeKeyword;

            BlockSyntax block = unsafeStatement.Block;

            IEnumerable<SyntaxTrivia> leadingTrivia = keyword.LeadingTrivia
                .AddRange(keyword.TrailingTrivia.EmptyIfWhitespace())
                .AddRange(block.GetLeadingTrivia().EmptyIfWhitespace());

            return block.WithLeadingTrivia(leadingTrivia);
        }
    }
}
