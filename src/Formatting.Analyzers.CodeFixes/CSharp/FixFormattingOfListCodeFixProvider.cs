// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using Roslynator.Text;
using static Roslynator.CSharp.SyntaxTriviaAnalysis;
using static Roslynator.Formatting.CSharp.FixFormattingOfListAnalyzer;

namespace Roslynator.Formatting.CodeFixes.CSharp
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixFormattingOfListCodeFixProvider))]
    [Shared]
    internal class FixFormattingOfListCodeFixProvider : BaseCodeFixProvider
    {
        private const string Title = "Fix formatting";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.FixFormattingOfList); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.ParameterList:
                        case SyntaxKind.BracketedParameterList:
                        case SyntaxKind.TypeParameterList:
                        case SyntaxKind.ArgumentList:
                        case SyntaxKind.BracketedArgumentList:
                        case SyntaxKind.AttributeArgumentList:
                        case SyntaxKind.TypeArgumentList:
                        case SyntaxKind.AttributeList:
                        case SyntaxKind.BaseList:
                        case SyntaxKind.TupleType:
                        case SyntaxKind.TupleExpression:
                        case SyntaxKind.ArrayInitializerExpression:
                        case SyntaxKind.CollectionInitializerExpression:
                        case SyntaxKind.ComplexElementInitializerExpression:
                        case SyntaxKind.ObjectInitializerExpression:
                            return true;
                        default:
                            return false;
                    }
                }))
            {
                return;
            }

            Document document = context.Document;
            Diagnostic diagnostic = context.Diagnostics[0];
            CodeAction codeAction = CreateCodeAction();

            context.RegisterCodeFix(codeAction, diagnostic);

            CodeAction CreateCodeAction()
            {
                switch (node)
                {
                    case ParameterListSyntax parameterList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, parameterList, parameterList.OpenParenToken, parameterList.Parameters, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case BracketedParameterListSyntax bracketedParameterList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, bracketedParameterList, bracketedParameterList.OpenBracketToken, bracketedParameterList.Parameters, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case TypeParameterListSyntax typeParameterList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, typeParameterList, typeParameterList.LessThanToken, typeParameterList.Parameters, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case ArgumentListSyntax argumentList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, argumentList, argumentList.OpenParenToken, argumentList.Arguments, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case BracketedArgumentListSyntax bracketedArgumentList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, bracketedArgumentList, bracketedArgumentList.OpenBracketToken, bracketedArgumentList.Arguments, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case AttributeArgumentListSyntax attributeArgumentList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, attributeArgumentList, attributeArgumentList.OpenParenToken, attributeArgumentList.Arguments, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case TypeArgumentListSyntax typeArgumentList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, typeArgumentList, typeArgumentList.LessThanToken, typeArgumentList.Arguments, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case AttributeListSyntax attributeList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, attributeList, attributeList.OpenBracketToken, attributeList.Attributes, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case BaseListSyntax baseList:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, baseList, baseList.ColonToken, baseList.Types, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case TupleTypeSyntax tupleType:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, tupleType, tupleType.OpenParenToken, tupleType.Elements, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case TupleExpressionSyntax tupleExpression:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, tupleExpression, tupleExpression.OpenParenToken, tupleExpression.Arguments, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    case InitializerExpressionSyntax initializerExpression:
                        {
                            return CodeAction.Create(
                                Title,
                                ct => FixAsync(document, initializerExpression, initializerExpression.OpenBraceToken, initializerExpression.Expressions, ct),
                                GetEquivalenceKey(diagnostic));
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }

        private static Task<Document> FixAsync<TNode>(
            Document document,
            SyntaxNode containingNode,
            SyntaxNodeOrToken openNodeOrToken,
            SeparatedSyntaxList<TNode> nodes,
            CancellationToken cancellationToken) where TNode : SyntaxNode
        {
            IndentationAnalysis indentationAnalysis = AnalyzeIndentation(containingNode, cancellationToken);

            string increasedIndentation = indentationAnalysis.GetIncreasedIndentation();

            if (nodes.IsSingleLine(includeExteriorTrivia: false, cancellationToken: cancellationToken))
            {
                TNode node = nodes[0];

                SyntaxTriviaList leading = node.GetLeadingTrivia();

                TextSpan span = (leading.Any() && leading.Last().IsWhitespaceTrivia())
                    ? leading.Last().Span
                    : new TextSpan(node.SpanStart, 0);

                return document.WithTextChangeAsync(
                    new TextChange(span, increasedIndentation),
                    cancellationToken);
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
                    token = nodes.GetSeparator(i - 1);
                }

                SyntaxTriviaList trailing = token.TrailingTrivia;
                TNode node = nodes[i];
                var indentationAdded = false;

                if (IsOptionalWhitespaceThenOptionalSingleLineCommentThenEndOfLineTrivia(trailing))
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
                        LambdaBlock lambdaBlock = GetLambdaBlock(argument, lines ??= argument.SyntaxTree.GetText().Lines);

                        if (lambdaBlock.Block != null)
                            increasedIndentation = indentationAnalysis.Indentation.ToString();
                    }

                    if (nodes.Count > 1
                        && (i > 0 || !containingNode.IsKind(SyntaxKind.AttributeList)))
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

                LambdaBlock lambdaBlock2 = GetLambdaBlock(node, lines ??= node.SyntaxTree.GetText().Lines);

                bool isLambdaBlockWithOpenBraceAtEndOfLine = lambdaBlock2.Token == indentations.Last().Token;

                int baseIndentationLength = (isLambdaBlockWithOpenBraceAtEndOfLine)
                    ? indentations.Last().Span.Length
                    : indentations[0].Span.Length;

                for (int j = indentations.Length - 1; j >= 0; j--)
                {
                    IndentationInfo indentationInfo = indentations[j];

                    if (indentationAdded
                        && node is ArgumentSyntax argument
                        && (argument.Expression as AnonymousFunctionExpressionSyntax)?.Block != null)
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

            return document.WithTextChangesAsync(textChanges, cancellationToken);
        }
    }
}
