// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp;

internal static class CodeActionFactory
{
    public static async Task CreateAndRegisterCodeActionForBlankLineAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);
        Diagnostic diagnostic = context.Diagnostics[0];

        TextChange textChange = GetTextChangeForBlankLine(root, context.Span.Start);

        CodeAction codeAction = CodeAction.Create(
            (textChange.NewText.Length == 0) ? "Remove blank line" : "Add blank line",
            ct => context.Document.WithTextChangeAsync(textChange, ct),
            EquivalenceKey.Create(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    public static TextChange GetTextChangeForBlankLine(SyntaxNode root, int position)
    {
        SyntaxToken token = root.FindToken(position);
        SyntaxToken first;
        SyntaxToken second;

        if (token.Span.End <= position)
        {
            first = token;
            second = token.GetNextToken();
        }
        else
        {
            second = token;
            first = token.GetPreviousToken();
        }

        string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(first).ToString();
        TriviaBetweenAnalysis analysis = TriviaBetweenAnalysis.Create(first, second);

        Debug.Assert(position == analysis.Position);

        switch (analysis.Kind)
        {
            case TriviaBetweenKind.NoNewLine:
                {
                    TriviaBetweenAnalysis.Enumerator en = analysis.GetEnumerator();
                    string newText = endOfLine;

                    if (!en.MoveNext())
                    {
                        newText += endOfLine;
                    }
                    else if (en.Current.IsWhitespaceTrivia())
                    {
                        position = en.Current.Span.End;
                    }

                    return new TextChange(new TextSpan(position, 0), newText);
                }
            case TriviaBetweenKind.NewLine:
                {
                    TriviaBetweenAnalysis.Enumerator en = analysis.GetEnumerator();

                    while (en.MoveNext()
                        && !en.Current.IsEndOfLineTrivia())
                    {
                    }

                    return new TextChange(new TextSpan(en.Current.Span.End, 0), endOfLine);
                }
            case TriviaBetweenKind.BlankLine:
            case TriviaBetweenKind.BlankLines:
                {
                    TriviaBetweenAnalysis.Enumerator en = analysis.GetEnumerator();

                    while (en.MoveNext()
                        && en.Current.SpanStart != position)
                    {
                    }

                    int end = en.Current.Span.End;

                    while (en.MoveNext())
                    {
                        if (en.Current.IsWhitespaceTrivia() && !en.MoveNext())
                            break;

                        if (!en.Current.IsEndOfLineTrivia())
                            break;

                        end = en.Current.Span.End;
                    }

                    return new TextChange(TextSpan.FromBounds(position, end), "");
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    public static async Task CreateAndRegisterCodeActionForNewLineAsync(
        CodeFixContext context,
        string title = null,
        string indentation = null,
        CodeActionNewLineOptions options = CodeActionNewLineOptions.None)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);
        Diagnostic diagnostic = context.Diagnostics[0];

        TextChange textChange = GetTextChangeForNewLine(
            root,
            context.Span.Start,
            context.Document.GetConfigOptions(root.SyntaxTree),
            indentation,
            options,
            context.CancellationToken);

        CodeAction codeAction = CodeAction.Create(
            title ?? ((textChange.NewText.Length == 0) ? "Remove newline" : "Add newline"),
            ct => context.Document.WithTextChangeAsync(textChange, ct),
            EquivalenceKey.Create(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    public static TextChange GetTextChangeForNewLine(
        SyntaxNode root,
        int position,
        AnalyzerConfigOptions configOptions,
        string indentation = null,
        CodeActionNewLineOptions options = CodeActionNewLineOptions.None,
        CancellationToken cancellationToken = default)
    {
        SyntaxToken token = root.FindToken(position);
        SyntaxToken first;
        SyntaxToken second;

        if (token.Span.End <= position)
        {
            first = token;
            second = token.GetNextToken();
        }
        else
        {
            second = token;
            first = token.GetPreviousToken();
        }

        TriviaBetweenAnalysis analysis = TriviaBetweenAnalysis.Create(first, second);
        string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(first).ToString();

        Debug.Assert(position == analysis.Position);

        switch (analysis.Kind)
        {
            case TriviaBetweenKind.NoNewLine:
                {
                    TriviaBetweenAnalysis.Enumerator en = analysis.GetEnumerator();
                    int end = position;

                    if (en.MoveNext()
                        && en.Current.IsWhitespaceTrivia())
                    {
                        end = en.Current.Span.End;
                    }

                    indentation ??= ((options & CodeActionNewLineOptions.IncreaseIndentation) != 0)
                        ? SyntaxTriviaAnalysis.GetIncreasedIndentation(first.Parent, configOptions, cancellationToken)
                        : SyntaxTriviaAnalysis.DetermineIndentation(first, cancellationToken).ToString();

                    return new TextChange(TextSpan.FromBounds(position, end), endOfLine + indentation);
                }
            case TriviaBetweenKind.NewLine:
                {
                    TriviaBetweenAnalysis.Enumerator en = analysis.GetEnumerator();
                    int start = first.Span.End;
                    int end = start;

                    while (en.MoveNext())
                    {
                        if (en.Current.IsEndOfLineTrivia())
                        {
                            end = en.Current.Span.End;
                            if (en.MoveNext()
                                && en.Current.IsWhitespaceTrivia())
                            {
                                end = en.Current.Span.End;
                            }

                            break;
                        }
                    }

                    return new TextChange(TextSpan.FromBounds(start, end), " ");
                }
            case TriviaBetweenKind.BlankLine:
            case TriviaBetweenKind.BlankLines:
                {
                    TriviaBetweenAnalysis.Enumerator en = analysis.GetEnumerator();

                    while (en.MoveNext()
                        && en.Current.SpanStart != position)
                    {
                    }

                    int end = en.Current.Span.End;

                    while (en.MoveNext()
                        && en.Current.IsWhitespaceOrEndOfLineTrivia())
                    {
                        end = en.Current.Span.End;
                    }

                    return new TextChange(TextSpan.FromBounds(first.Span.End, end), " ");
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    public static CodeAction Create(
        string title,
        Func<CancellationToken, Task<Solution>> createChangedSolution,
        RefactoringDescriptor descriptor,
        string additionalEquivalenceKey1 = null,
        string additionalEquivalenceKey2 = null)
    {
        return CodeAction.Create(
            title,
            createChangedSolution,
            EquivalenceKey.Create(descriptor, additionalEquivalenceKey1, additionalEquivalenceKey2));
    }

    public static CodeAction Create(
        string title,
        Func<CancellationToken, Task<Document>> createChangedDocument,
        RefactoringDescriptor descriptor,
        string additionalEquivalenceKey1 = null,
        string additionalEquivalenceKey2 = null)
    {
        return CodeAction.Create(
            title,
            createChangedDocument,
            EquivalenceKey.Create(descriptor, additionalEquivalenceKey1, additionalEquivalenceKey2));
    }

    public static CodeAction ChangeTypeToVar(
        Document document,
        TypeSyntax type,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? "Use implicit type",
            ct => DocumentRefactorings.ChangeTypeToVarAsync(document, type, ct),
            equivalenceKey);
    }

    public static CodeAction ChangeTypeToVar(
        Document document,
        TupleExpressionSyntax tupleExpression,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? "Use implicit type",
            ct => DocumentRefactorings.ChangeTypeToVarAsync(document, tupleExpression, ct),
            equivalenceKey);
    }

    public static CodeAction UseExplicitType(
        Document document,
        TypeSyntax type,
        ITypeSymbol newTypeSymbol,
        SemanticModel semanticModel,
        string equivalenceKey = null)
    {
        return ChangeType(document, type, newTypeSymbol, semanticModel, title: "Use explicit type", equivalenceKey: equivalenceKey);
    }

    public static CodeAction ChangeType(
        Document document,
        TypeSyntax type,
        ITypeSymbol newTypeSymbol,
        SemanticModel semanticModel,
        string title = null,
        string equivalenceKey = null)
    {
        if (title is null)
        {
            SymbolDisplayFormat format = GetSymbolDisplayFormat(type, newTypeSymbol, semanticModel);

            string newTypeName = SymbolDisplay.ToMinimalDisplayString(newTypeSymbol, semanticModel, type.SpanStart, format);

            if ((type.Parent is MethodDeclarationSyntax methodDeclaration && methodDeclaration.ReturnType == type)
                || (type.Parent is LocalFunctionStatementSyntax localFunction && localFunction.ReturnType == type))
            {
                title = $"Change return type to '{newTypeName}'";
            }
            else
            {
                title = $"Change type to '{newTypeName}'";
            }
        }

        return ChangeType(document, type, newTypeSymbol, title, equivalenceKey);
    }

    private static CodeAction ChangeType(
        Document document,
        TypeSyntax type,
        ITypeSymbol newTypeSymbol,
        string title,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title,
            ct => DocumentRefactorings.ChangeTypeAsync(document, type, newTypeSymbol, ct),
            equivalenceKey);
    }

    private static SymbolDisplayFormat GetSymbolDisplayFormat(
        ExpressionSyntax expression,
        ITypeSymbol newTypeSymbol,
        SemanticModel semanticModel)
    {
        if (newTypeSymbol.NullableAnnotation == NullableAnnotation.Annotated
            && (semanticModel.GetNullableContext(expression.SpanStart) & NullableContext.WarningsEnabled) != 0)
        {
            return SymbolDisplayFormats.FullName;
        }
        else
        {
            return SymbolDisplayFormats.FullName_WithoutNullableReferenceTypeModifier;
        }
    }

    public static CodeAction AddExplicitCast(
        Document document,
        ExpressionSyntax expression,
        ITypeSymbol destinationType,
        SemanticModel semanticModel,
        string title = null,
        string equivalenceKey = null)
    {
        SymbolDisplayFormat format = GetSymbolDisplayFormat(expression, destinationType, semanticModel);

        string typeName = SymbolDisplay.ToMinimalDisplayString(destinationType, semanticModel, expression.SpanStart, format);

        TypeSyntax newType = ParseTypeName(typeName);

        return CodeAction.Create(
            title ?? "Add explicit cast",
            ct => DocumentRefactorings.AddExplicitCastAsync(document, expression, newType, ct),
            equivalenceKey);
    }

    public static CodeAction RemoveMemberDeclaration(
        Document document,
        MemberDeclarationSyntax memberDeclaration,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? $"Remove {CSharpFacts.GetTitle(memberDeclaration)}",
            ct => document.RemoveMemberAsync(memberDeclaration, ct),
            equivalenceKey);
    }

    public static CodeAction RemoveStatement(
        Document document,
        StatementSyntax statement,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? $"Remove {CSharpFacts.GetTitle(statement)}",
            ct => document.RemoveStatementAsync(statement, ct),
            equivalenceKey);
    }

    public static CodeAction ReplaceNullWithDefaultValue(
        Document document,
        ExpressionSyntax expression,
        ITypeSymbol typeSymbol,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? "Replace 'null' with default value",
            ct =>
            {
                ExpressionSyntax defaultValue = typeSymbol
                    .GetDefaultValueSyntax(document.GetDefaultSyntaxOptions())
                    .WithTriviaFrom(expression);

                return document.ReplaceNodeAsync(expression, defaultValue, ct);
            },
            equivalenceKey);
    }

    public static CodeAction RemoveAsyncAwait(
        Document document,
        SyntaxToken asyncKeyword,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? "Remove async/await",
            ct => DocumentRefactorings.RemoveAsyncAwaitAsync(document, asyncKeyword, ct),
            equivalenceKey);
    }

    public static CodeAction RemoveParentheses(
        Document document,
        ParenthesizedExpressionSyntax parenthesizedExpression,
        string title = null,
        string equivalenceKey = null)
    {
        return CodeAction.Create(
            title ?? "Remove parentheses",
            ct => DocumentRefactorings.RemoveParenthesesAsync(document, parenthesizedExpression, ct),
            equivalenceKey);
    }
}
