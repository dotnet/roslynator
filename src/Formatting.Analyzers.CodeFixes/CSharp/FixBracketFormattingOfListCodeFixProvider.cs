using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixBracketFormattingOfListCodeFixProvider))]
[Shared]
public sealed class FixBracketFormattingOfListCodeFixProvider : BaseCodeFixProvider
{
    private const string Title = "Fix bracket formatting";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIdentifiers.FixBracketFormattingOfList);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
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
                        case SyntaxKind.TupleType:
                        case SyntaxKind.TupleExpression:
                        case SyntaxKind.ArrayInitializerExpression:
                        case SyntaxKind.CollectionInitializerExpression:
                        case SyntaxKind.CollectionExpression:
                        case SyntaxKind.ComplexElementInitializerExpression:
                        case SyntaxKind.ObjectInitializerExpression:
                            return true;
                        default:
                            return false;
                    }
                }
            )
        )
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
                        ct => Fix(parameterList, parameterList.OpenParenToken, parameterList.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case BracketedParameterListSyntax bracketedParameterList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(bracketedParameterList, bracketedParameterList.OpenBracketToken, bracketedParameterList.CloseBracketToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TypeParameterListSyntax typeParameterList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(typeParameterList, typeParameterList.LessThanToken, typeParameterList.GreaterThanToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case ArgumentListSyntax argumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(argumentList, argumentList.OpenParenToken, argumentList.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case BracketedArgumentListSyntax bracketedArgumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(bracketedArgumentList, bracketedArgumentList.OpenBracketToken, bracketedArgumentList.CloseBracketToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case AttributeArgumentListSyntax attributeArgumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(attributeArgumentList, attributeArgumentList.OpenParenToken, attributeArgumentList.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TypeArgumentListSyntax typeArgumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(typeArgumentList, typeArgumentList.LessThanToken, typeArgumentList.GreaterThanToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case AttributeListSyntax attributeList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(attributeList, attributeList.OpenBracketToken, attributeList.CloseBracketToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TupleTypeSyntax tupleType:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(tupleType, tupleType.OpenParenToken, tupleType.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TupleExpressionSyntax tupleExpression:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(tupleExpression, tupleExpression.OpenParenToken, tupleExpression.CloseParenToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case InitializerExpressionSyntax initializerExpression:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(initializerExpression, initializerExpression.OpenBraceToken, initializerExpression.CloseBraceToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case CollectionExpressionSyntax collectionExpression:
                {
                    return CodeAction.Create(
                        Title,
                        ct => Fix(collectionExpression, collectionExpression.OpenBracketToken, collectionExpression.CloseBracketToken, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }

        async Task<Document> Fix(
            SyntaxNode listNode,
            SyntaxToken openNodeOrToken,
            SyntaxToken closeNodeOrToken,
            CancellationToken cancellationToken
        )
        {
            AnalyzerConfigOptions configOptions = document.GetConfigOptions(listNode.SyntaxTree);
            TargetBracesStyle bracesStyle = configOptions.GetTargetBracesStyle();

            if (bracesStyle == TargetBracesStyle.None)
            {
                return document;
            }

            string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(node).ToString();

            List<TextChange> textChanges = new ();

            if (bracesStyle.HasFlag(TargetBracesStyle.Opening))
            {
                SyntaxToken nextToken = openNodeOrToken.GetNextToken();

                int bracketLine = openNodeOrToken.GetSpanStartLine();
                int nextTokenLine = nextToken.GetSpanStartLine();

                if (bracketLine == nextTokenLine)
                {
                    string indentation = SyntaxTriviaAnalysis.GetIncreasedIndentation(listNode, configOptions, cancellationToken);

                    textChanges.Add(
                        new TextChange(
                            TextSpan.FromBounds(openNodeOrToken.Span.End, nextToken.SpanStart),
                            endOfLine + indentation
                        )
                    );
                }
            }

            if (bracesStyle.HasFlag(TargetBracesStyle.Closing))
            {
                SyntaxToken previousToken = closeNodeOrToken.GetPreviousToken();

                int bracketLine = closeNodeOrToken.GetSpanStartLine();
                int previousTokenLine = previousToken.GetSpanEndLine();

                if (bracketLine == previousTokenLine)
                {
                    string indentation = SyntaxTriviaAnalysis.DetermineIndentation(listNode, cancellationToken).ToString();

                    textChanges.Add(
                        new TextChange(
                            closeNodeOrToken.Span,
                            endOfLine + indentation + closeNodeOrToken
                        )
                    );
                }
                else
                {
                    SyntaxTrivia listNodeIndent = SyntaxTriviaAnalysis.DetermineIndentation(listNode);
                    SyntaxTrivia bracketIndent = SyntaxTriviaAnalysis.DetermineIndentation(closeNodeOrToken);
                    if (listNodeIndent.Span.Length != bracketIndent.Span.Length)
                    {
                        TextSpan span =
                            bracketIndent.Span.Length == 0 // there is no indentation
                                ? new TextSpan(closeNodeOrToken.Span.Start, 0)
                                : bracketIndent.Span;

                        textChanges.Add(
                            new TextChange(
                                span,
                                listNodeIndent.ToString()
                            )
                        );
                    }
                }
            }

            return await document.WithTextChangesAsync(textChanges, cancellationToken);
        }
    }
}