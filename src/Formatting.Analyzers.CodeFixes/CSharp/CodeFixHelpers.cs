// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;
using Roslynator.Text;
using static Roslynator.CSharp.SyntaxTriviaAnalysis;
using static Roslynator.Formatting.CSharp.FixFormattingOfListAnalyzer;

namespace Roslynator.Formatting.CodeFixes.CSharp;

internal static class CodeFixHelpers
{
    public static Task<Document> AddNewLineBeforeAsync(
        Document document,
        SyntaxToken token,
        string indentation,
        CancellationToken cancellationToken = default)
    {
        return document.WithTextChangeAsync(
            GetNewLineBeforeTextChange(token, indentation),
            cancellationToken);
    }

    public static TextChange GetNewLineBeforeTextChange(SyntaxToken token, string indentation)
    {
        return new TextChange(
            TextSpan.FromBounds(token.GetPreviousToken().Span.End, token.SpanStart),
            DetermineEndOfLine(token).ToString() + indentation);
    }

    public static Task<Document> AddNewLineAfterAsync(
        Document document,
        SyntaxToken token,
        string indentation,
        CancellationToken cancellationToken = default)
    {
        return document.WithTextChangeAsync(
            GetNewLineAfterTextChange(token, indentation),
            cancellationToken);
    }

    public static TextChange GetNewLineAfterTextChange(SyntaxToken token, string indentation)
    {
        return new TextChange(
            TextSpan.FromBounds(token.Span.End, token.GetNextToken().SpanStart),
            DetermineEndOfLine(token).ToString() + indentation);
    }

    public static Task<Document> AddBlankLineBeforeDirectiveAsync(
        Document document,
        DirectiveTriviaSyntax directiveTrivia,
        CancellationToken cancellationToken = default)
    {
        SyntaxTrivia parentTrivia = directiveTrivia.ParentTrivia;
        SyntaxToken token = parentTrivia.Token;
        SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

        int index = leadingTrivia.IndexOf(parentTrivia);

        if (index > 0
            && leadingTrivia[index - 1].IsWhitespaceTrivia())
        {
            index--;
        }

        SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index, DetermineEndOfLine(token));

        SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

        return document.ReplaceTokenAsync(token, newToken, cancellationToken);
    }

    public static Task<Document> AddBlankLineAfterDirectiveAsync(
        Document document,
        DirectiveTriviaSyntax directiveTrivia,
        CancellationToken cancellationToken = default)
    {
        SyntaxTrivia parentTrivia = directiveTrivia.ParentTrivia;
        SyntaxToken token = parentTrivia.Token;
        SyntaxTriviaList leadingTrivia = token.LeadingTrivia;

        int index = leadingTrivia.IndexOf(parentTrivia);

        SyntaxTriviaList newLeadingTrivia = leadingTrivia.Insert(index + 1, DetermineEndOfLine(token));

        SyntaxToken newToken = token.WithLeadingTrivia(newLeadingTrivia);

        return document.ReplaceTokenAsync(token, newToken, cancellationToken);
    }

    public static Task<Document> FixCallChainAsync(
        Document document,
        ExpressionSyntax expression,
        CancellationToken cancellationToken = default)
    {
        return FixCallChainAsync(document, expression, expression.Span, cancellationToken);
    }

    public static Task<Document> FixCallChainAsync(
        Document document,
        ExpressionSyntax expression,
        TextSpan span,
        CancellationToken cancellationToken = default)
    {
        NewLinePosition conditionalAccessOperatorNewLinePosition = document.GetConfigOptions(expression.SyntaxTree).GetNullConditionalOperatorNewLinePosition(NewLinePosition.After);

        IndentationAnalysis indentationAnalysis = AnalyzeIndentation(expression, document.GetConfigOptions(expression.SyntaxTree), cancellationToken);
        string indentation = indentationAnalysis.GetIncreasedIndentation();
        string endOfLineAndIndentation = DetermineEndOfLine(expression).ToString() + indentation;

        var textChanges = new List<TextChange>();
        int prevIndex = expression.Span.End;

        foreach (SyntaxNode node in new MethodChain(expression))
        {
            SyntaxKind kind = node.Kind();

            if (kind == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)node;

                if (!SetIndentation(memberAccess.OperatorToken))
                    break;
            }
            else if (kind == SyntaxKind.MemberBindingExpression)
            {
                var memberBinding = (MemberBindingExpressionSyntax)node;

                if (!memberBinding.HasLeadingTrivia)
                {
                    SyntaxToken prevToken = memberBinding.GetFirstToken().GetPreviousToken();

                    if (prevToken.IsKind(SyntaxKind.QuestionToken)
                        && prevToken.IsParentKind(SyntaxKind.ConditionalAccessExpression))
                    {
                        var conditionalAccess = (ConditionalAccessExpressionSyntax)prevToken.Parent;

                        if (expression.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(conditionalAccess.Expression.Span.End, conditionalAccess.OperatorToken.SpanStart), cancellationToken))
                            continue;
                    }
                }

                if (conditionalAccessOperatorNewLinePosition == NewLinePosition.After
                    && !SetIndentation(memberBinding.OperatorToken))
                {
                    break;
                }
            }
            else if (kind == SyntaxKind.ConditionalAccessExpression)
            {
                var conditionalAccess = (ConditionalAccessExpressionSyntax)node;

                if (conditionalAccessOperatorNewLinePosition == NewLinePosition.Before
                    || expression.SyntaxTree.IsMultiLineSpan(TextSpan.FromBounds(conditionalAccess.Expression.Span.End, conditionalAccess.OperatorToken.SpanStart), cancellationToken))
                {
                    if (!SetIndentation(conditionalAccess.OperatorToken))
                        break;
                }
            }
        }

        FormattingVerifier.VerifyChangedSpansAreWhitespace(expression, textChanges);

        return document.WithTextChangesAsync(textChanges, cancellationToken);

        bool SetIndentation(SyntaxToken token)
        {
            if (token.Span.End > span.End)
                return true;

            if (token.SpanStart < span.Start)
                return false;

            SyntaxTriviaList leading = token.LeadingTrivia;
            SyntaxTriviaList.Reversed.Enumerator en = leading.Reverse().GetEnumerator();

            if (!en.MoveNext())
            {
                SyntaxTrivia trivia = expression.FindTrivia(token.SpanStart - 1);

                string newText = (trivia.IsEndOfLineTrivia()) ? indentation : endOfLineAndIndentation;

                textChanges.Add(new TextSpan(token.SpanStart, 0), newText);

                SetIndentation2(token, prevIndex);
                prevIndex = (trivia.IsEndOfLineTrivia()) ? trivia.SpanStart : token.SpanStart;
                return true;
            }

            SyntaxTrivia last = en.Current;

            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia)
            {
                if (en.Current.Span.Length != indentation.Length)
                {
                    if (!en.MoveNext()
                        || en.Current.IsEndOfLineTrivia())
                    {
                        SyntaxTrivia trivia = expression.FindTrivia(token.FullSpan.Start - 1);

                        if (trivia.IsEndOfLineTrivia())
                        {
                            textChanges.Add((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span, indentation);
                            SetIndentation2(token, prevIndex);
                            prevIndex = trivia.SpanStart;
                            return true;
                        }
                    }
                }
            }
            else if (kind == SyntaxKind.EndOfLineTrivia)
            {
                SyntaxTrivia trivia = expression.FindTrivia(token.FullSpan.Start - 1);

                if (trivia.IsEndOfLineTrivia())
                {
                    textChanges.Add((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span, indentation);
                    SetIndentation2(token, prevIndex);
                    prevIndex = trivia.SpanStart;
                    return true;
                }
            }

            prevIndex = leading.Span.Start - 1;
            return true;
        }

        void SetIndentation2(SyntaxToken token, int endIndex)
        {
            ImmutableArray<IndentationInfo> indentations = FindIndentations(
                expression,
                TextSpan.FromBounds(token.SpanStart, endIndex))
                .ToImmutableArray();

            if (!indentations.Any())
                return;

            int firstIndentationLength = indentations[0].Span.Length;

            for (int j = 0; j < indentations.Length; j++)
            {
                IndentationInfo indentationInfo = indentations[j];

                string replacement = indentation + indentationAnalysis.GetSingleIndentation();

                if (j > 0
                    && indentationInfo.Span.Length > firstIndentationLength)
                {
                    replacement += indentationInfo.ToString().Substring(firstIndentationLength);
                }

                if (indentationInfo.Span.Length != replacement.Length)
                    textChanges.Add(indentationInfo.Span, replacement);
            }
        }
    }

    public static Task<Document> FixBinaryExpressionAsync(
        Document document,
        BinaryExpressionSyntax binaryExpression,
        CancellationToken cancellationToken)
    {
        return FixBinaryExpressionAsync(document, binaryExpression, binaryExpression.Span, cancellationToken);
    }

    public static Task<Document> FixBinaryExpressionAsync(
        Document document,
        BinaryExpressionSyntax binaryExpression,
        TextSpan span,
        CancellationToken cancellationToken)
    {
        AnalyzerConfigOptions configOptions = document.GetConfigOptions(binaryExpression.SyntaxTree);

        IndentationAnalysis indentationAnalysis = AnalyzeIndentation(binaryExpression, configOptions, cancellationToken);

        string indentation;
        if (indentationAnalysis.Indentation == binaryExpression.GetLeadingTrivia().LastOrDefault()
            && document.GetConfigOptions(binaryExpression.SyntaxTree).GetBinaryOperatorNewLinePosition() == NewLinePosition.After)
        {
            indentation = indentationAnalysis.Indentation.ToString();
        }
        else
        {
            indentation = indentationAnalysis.GetIncreasedIndentation();
        }

        string endOfLineAndIndentation = DetermineEndOfLine(binaryExpression).ToString() + indentation;

        var textChanges = new List<TextChange>();
        int prevIndex = binaryExpression.Span.End;

        SyntaxKind binaryKind = binaryExpression.Kind();

        while (true)
        {
            SyntaxToken token = binaryExpression.OperatorToken;

            if (token.Span.End > span.End)
                continue;

            if (token.SpanStart < span.Start)
                break;

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxTriviaList leftTrailing = left.GetTrailingTrivia();
            SyntaxTriviaList tokenTrailing = token.TrailingTrivia;

            if (TriviaBlock.FromTrailing(left).IsWrapped)
            {
                if (!SetIndentation(token))
                    break;
            }
            else if (TriviaBlock.FromTrailing(token).IsWrapped)
            {
                if (!SetIndentation(right))
                    break;
            }
            else if (leftTrailing.IsEmptyOrWhitespace()
                && tokenTrailing.IsEmptyOrWhitespace())
            {
                if (document.GetConfigOptions(binaryExpression.SyntaxTree).GetBinaryOperatorNewLinePosition() == NewLinePosition.After)
                {
                    if (!SetIndentation(right))
                        break;
                }
                else if (!SetIndentation(token))
                {
                    break;
                }
            }

            if (!left.IsKind(binaryKind))
                break;

            binaryExpression = (BinaryExpressionSyntax)left;
        }

        if (textChanges.Count > 0)
        {
            SyntaxTriviaList leading = binaryExpression.GetLeadingTrivia();

            if (!leading.Any())
            {
                SyntaxTrivia trivia = binaryExpression.GetFirstToken().GetPreviousToken().TrailingTrivia.LastOrDefault();

                if (trivia.IsEndOfLineTrivia()
                    && trivia.Span.End == binaryExpression.SpanStart)
                {
                    textChanges.Add(new TextSpan(binaryExpression.SpanStart, 0), indentation);
                }
            }
        }

        FormattingVerifier.VerifyChangedSpansAreWhitespace(binaryExpression, textChanges);

        return document.WithTextChangesAsync(textChanges, cancellationToken);

        bool SetIndentation(SyntaxNodeOrToken nodeOrToken)
        {
            SyntaxTriviaList leading = nodeOrToken.GetLeadingTrivia();
            SyntaxTriviaList.Reversed.Enumerator en = leading.Reverse().GetEnumerator();

            if (!en.MoveNext())
            {
                SyntaxTrivia trivia = binaryExpression.FindTrivia(nodeOrToken.SpanStart - 1);

                string newText = (trivia.IsEndOfLineTrivia()) ? indentation : endOfLineAndIndentation;

                int start = (trivia.IsWhitespaceTrivia()) ? trivia.SpanStart : nodeOrToken.SpanStart;

                TextSpan span = (trivia.IsWhitespaceTrivia())
                    ? trivia.Span
                    : new TextSpan(nodeOrToken.SpanStart, 0);

                textChanges.Add(span, newText);
                SetIndentation2(nodeOrToken, prevIndex);
                prevIndex = start;
                return true;
            }

            SyntaxTrivia last = en.Current;

            SyntaxKind kind = en.Current.Kind();

            if (kind == SyntaxKind.WhitespaceTrivia)
            {
                if (en.Current.Span.Length != indentation.Length)
                {
                    if (!en.MoveNext()
                        || en.Current.IsEndOfLineTrivia())
                    {
                        SyntaxTrivia trivia = binaryExpression.FindTrivia(nodeOrToken.FullSpan.Start - 1);

                        if (trivia.IsEndOfLineTrivia())
                        {
                            AddTextChange((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span);
                            SetIndentation2(nodeOrToken, prevIndex);
                            prevIndex = trivia.SpanStart;
                            return true;
                        }
                    }
                }
            }
            else if (kind == SyntaxKind.EndOfLineTrivia)
            {
                SyntaxTrivia trivia = binaryExpression.FindTrivia(nodeOrToken.FullSpan.Start - 1);

                if (trivia.IsEndOfLineTrivia())
                {
                    AddTextChange((leading.IsEmptyOrWhitespace()) ? leading.Span : last.Span);
                    SetIndentation2(nodeOrToken, prevIndex);
                    prevIndex = trivia.SpanStart;
                    return true;
                }
            }

            prevIndex = leading.Span.Start - 1;
            return true;

            void AddTextChange(TextSpan span) => textChanges.Add(span, indentation);
        }

        void SetIndentation2(SyntaxNodeOrToken nodeOrToken, int endIndex)
        {
            ImmutableArray<IndentationInfo> indentations = FindIndentations(
                binaryExpression,
                TextSpan.FromBounds(nodeOrToken.SpanStart, endIndex))
                .ToImmutableArray();

            if (!indentations.Any())
                return;

            int firstIndentationLength = indentations[0].Span.Length;

            for (int j = 0; j < indentations.Length; j++)
            {
                IndentationInfo indentationInfo = indentations[j];

                string replacement = indentation + indentationAnalysis.GetSingleIndentation();

                if (j > 0
                    && indentationInfo.Span.Length > firstIndentationLength)
                {
                    replacement += indentationInfo.ToString().Substring(firstIndentationLength);
                }

                if (indentationInfo.Span.Length != replacement.Length)
                    textChanges.Add(indentationInfo.Span, replacement);
            }
        }
    }

    public static Task<Document> FixListAsync(
        Document document,
        ParameterListSyntax parameterList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            parameterList,
            parameterList.OpenParenToken,
            parameterList.Parameters,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        BracketedParameterListSyntax bracketedParameterList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            bracketedParameterList,
            bracketedParameterList.OpenBracketToken,
            bracketedParameterList.Parameters,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        TypeParameterListSyntax typeParameterList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            typeParameterList,
            typeParameterList.LessThanToken,
            typeParameterList.Parameters,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        ArgumentListSyntax argumentList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            argumentList,
            argumentList.OpenParenToken,
            argumentList.Arguments,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        BracketedArgumentListSyntax bracketedArgumentList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            bracketedArgumentList,
            bracketedArgumentList.OpenBracketToken,
            bracketedArgumentList.Arguments,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        AttributeArgumentListSyntax attributeArgumentList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            attributeArgumentList,
            attributeArgumentList.OpenParenToken,
            attributeArgumentList.Arguments,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        TypeArgumentListSyntax typeArgumentList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            typeArgumentList,
            typeArgumentList.LessThanToken,
            typeArgumentList.Arguments,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        AttributeListSyntax attributeList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            attributeList,
            attributeList.OpenBracketToken,
            attributeList.Attributes,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        BaseListSyntax baseList,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(document, baseList, baseList.ColonToken, baseList.Types, fixMode, cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        TupleTypeSyntax tupleType,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            tupleType,
            tupleType.OpenParenToken,
            tupleType.Elements,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        TupleExpressionSyntax tupleExpression,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            tupleExpression,
            tupleExpression.OpenParenToken,
            tupleExpression.Arguments,
            fixMode,
            cancellationToken);
    }

    public static Task<Document> FixListAsync(
        Document document,
        InitializerExpressionSyntax initializerExpression,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default)
    {
        return FixListAsync(
            document,
            initializerExpression,
            initializerExpression.OpenBraceToken,
            initializerExpression.Expressions,
            fixMode,
            cancellationToken);
    }

    private static Task<Document> FixListAsync<TNode>(
        Document document,
        SyntaxNode containingNode,
        SyntaxNodeOrToken openNodeOrToken,
        SeparatedSyntaxList<TNode> nodes,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default) where TNode : SyntaxNode
    {
        List<TextChange> textChanges = GetFixListChanges(
            document,
            containingNode,
            openNodeOrToken,
            nodes,
            fixMode,
            cancellationToken);

        return document.WithTextChangesAsync(
            textChanges,
            cancellationToken);
    }

    internal static List<TextChange> GetFixListChanges<TNode>(
        Document document,
        SyntaxNode containingNode,
        SyntaxNodeOrToken openNodeOrToken,
        IReadOnlyList<TNode> nodes,
        ListFixMode fixMode = ListFixMode.Fix,
        CancellationToken cancellationToken = default) where TNode : SyntaxNode
    {
        AnalyzerConfigOptions configOptions = document.GetConfigOptions(containingNode.SyntaxTree);

        IndentationAnalysis indentationAnalysis = AnalyzeIndentation(containingNode, configOptions, cancellationToken);

        string increasedIndentation = indentationAnalysis.GetIncreasedIndentation();

        bool isSingleLine;
        SeparatedSyntaxList<TNode> separatedList = default;

        if (nodes is SyntaxList<TNode> list)
        {
            isSingleLine = list.IsSingleLine(includeExteriorTrivia: false, cancellationToken: cancellationToken);
        }
        else
        {
            separatedList = (SeparatedSyntaxList<TNode>)nodes;

            isSingleLine = separatedList.IsSingleLine(
                includeExteriorTrivia: false,
                cancellationToken: cancellationToken);
        }

        if (isSingleLine
            && fixMode == ListFixMode.Fix)
        {
            TNode node = nodes[0];

            SyntaxTriviaList leading = node.GetLeadingTrivia();

            TextSpan span = (leading.Any() && leading.Last().IsWhitespaceTrivia())
                ? leading.Last().Span
                : new TextSpan(node.SpanStart, 0);

            return new List<TextChange>() { new TextChange(span, increasedIndentation) };
        }

        var textChanges = new List<TextChange>();
        TextLineCollection lines = null;
        string endOfLine = DetermineEndOfLine(containingNode).ToString();

        for (int i = 0; i < nodes.Count; i++)
        {
            SyntaxToken token;
            if (i == 0)
            {
                token = (openNodeOrToken.IsNode)
                    ? openNodeOrToken.AsNode().GetLastToken()
                    : openNodeOrToken.AsToken();
            }
            else
            {
                token = (list == default)
                    ? separatedList.GetSeparator(i - 1)
                    : list[i - 1].GetLastToken();
            }

            SyntaxTriviaList trailing = token.TrailingTrivia;
            TNode node = nodes[i];
            var indentationAdded = false;

            if (TriviaBlock.FromTrailing(token).IsWrapped)
            {
                SyntaxTrivia last = node.GetLeadingTrivia().LastOrDefault();

                if (last.IsWhitespaceTrivia())
                {
                    if (last.Span.Length == increasedIndentation.Length)
                        continue;

                    textChanges.Add(last.Span, increasedIndentation);
                }
                else
                {
                    textChanges.Add(new TextSpan(node.SpanStart, 0), increasedIndentation);
                }

                indentationAdded = true;
            }
            else
            {
                if (nodes.Count == 1
                    && node is ArgumentSyntax argument)
                {
                    LambdaBlock lambdaBlock = GetLambdaBlock(argument, lines ??= argument.SyntaxTree.GetText(cancellationToken).Lines);

                    if (lambdaBlock.Block is not null)
                        increasedIndentation = indentationAnalysis.Indentation.ToString();
                }

                if ((nodes.Count > 1 || fixMode == ListFixMode.Wrap)
                    && ShouldWrapAndIndent(containingNode, i))
                {
                    textChanges.Add(
                        (trailing.Any() && trailing.Last().IsWhitespaceTrivia())
                            ? trailing.Last().Span
                            : new TextSpan(token.FullSpan.End, 0),
                        endOfLine);

                    textChanges.Add(new TextSpan(node.FullSpan.Start, 0), increasedIndentation);

                    indentationAdded = true;
                }
            }

            ImmutableArray<IndentationInfo> indentations = FindIndentations(node, node.Span).ToImmutableArray();

            if (!indentations.Any())
                continue;

            LambdaBlock lambdaBlock2 = GetLambdaBlock(node, lines ??= node.SyntaxTree.GetText(cancellationToken).Lines);

            bool isLambdaBlockWithOpenBraceAtEndOfLine = lambdaBlock2.Token == indentations.Last().Token;

            int baseIndentationLength = (isLambdaBlockWithOpenBraceAtEndOfLine)
                ? indentations.Last().Span.Length
                : indentations[0].Span.Length;

            for (int j = indentations.Length - 1; j >= 0; j--)
            {
                IndentationInfo indentationInfo = indentations[j];

                if (indentationAdded
                    && node is ArgumentSyntax argument
                    && argument.Expression is AnonymousFunctionExpressionSyntax { Block: not null })
                {
                    indentationAdded = false;
                }

                string replacement = increasedIndentation;

                if (indentationAdded)
                    replacement += indentationAnalysis.GetSingleIndentation();

                if ((j > 0 || isLambdaBlockWithOpenBraceAtEndOfLine)
                    && indentationInfo.Span.Length > baseIndentationLength)
                {
                    replacement += indentationInfo.ToString().Substring(baseIndentationLength);
                }

                if (indentationInfo.Span.Length != replacement.Length)
                    textChanges.Add(indentationInfo.Span, replacement);
            }
        }

        FormattingVerifier.VerifyChangedSpansAreWhitespace(containingNode, textChanges);

        return textChanges;
    }
}
