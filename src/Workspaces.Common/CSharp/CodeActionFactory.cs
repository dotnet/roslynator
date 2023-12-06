// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
    public static async Task RegisterCodeActionForNewLineAroundTokenAsync(
        CodeFixContext context,
        Func<SyntaxToken, bool> tokenPredicate,
        string newLineReplacement = " ")
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);
        Diagnostic diagnostic = context.Diagnostics[0];
        int position = context.Span.Start;
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

        SyntaxToken third;
        if (tokenPredicate(first))
        {
            third = second;
            second = first;
            first = second.GetPreviousToken();
        }
        else
        {
            third = second.GetNextToken();
        }

        AnalyzerConfigOptions configOptions = context.Document.GetConfigOptions(root.SyntaxTree);
        var textChanges = new List<TextChange>();

        TriviaBlockAnalysis analysis1 = TriviaBlockAnalysis.FromBetween(first, second);

        if (!analysis1.ContainsComment)
        {
            string indentation = null;

            if (analysis1.Kind == TriviaBlockKind.NoNewLine)
                indentation = GetIncreasedIndentation(analysis1.First, configOptions, context.CancellationToken);

            TextChange textChange = GetTextChangeForNewLine(analysis1, configOptions, indentation, newLineReplacement: newLineReplacement, cancellationToken: context.CancellationToken);
            textChanges.Add(textChange);
        }

        TriviaBlockAnalysis analysis2 = TriviaBlockAnalysis.FromBetween(second, third);

        if (!analysis2.ContainsComment)
        {
            TextChange textChange = GetTextChangeForNewLine(analysis2, configOptions, newLineReplacement: newLineReplacement, cancellationToken: context.CancellationToken);
            textChanges.Add(textChange);
        }

        string title = (position >= second.Span.End)
            ? $"Place new line before '{second}'"
            : $"Place new line after '{second}'";

        CodeAction codeAction = CodeAction.Create(
            title,
            ct => context.Document.WithTextChangesAsync(textChanges, ct),
            EquivalenceKey.Create(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    public static async Task RegisterCodeActionForBlankLineAsync(CodeFixContext context)
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
        TriviaBlockAnalysis analysis = Analyze(root, position);

        string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(analysis.FirstOrSecond).ToString();

        Debug.Assert(position == analysis.Position);

        switch (analysis.Kind)
        {
            case TriviaBlockKind.NoNewLine:
                {
                    TriviaBlockAnalysis.Enumerator en = analysis.GetEnumerator();

                    if (en.ReadWhiteSpaceTrivia())
                        position = en.Current.Span.End;

                    return new TextChange(new TextSpan(position, 0), endOfLine + endOfLine);
                }
            case TriviaBlockKind.NewLine:
                {
                    return new TextChange(new TextSpan(position, 0), endOfLine);
                }
            case TriviaBlockKind.BlankLine:
                {
                    TriviaBlockAnalysis.Enumerator en = analysis.GetEnumerator();

                    en.ReadTo(analysis.Position);
                    en.ReadBlankLines();

                    return new TextChange(TextSpan.FromBounds(position, en.Current.Span.End), "");
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    public static async Task RegisterCodeActionForNewLineAsync(
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

    private static TextChange GetTextChangeForNewLine(
        SyntaxNode root,
        int position,
        AnalyzerConfigOptions configOptions,
        string indentation = null,
        CodeActionNewLineOptions options = CodeActionNewLineOptions.None,
        CancellationToken cancellationToken = default)
    {
        TriviaBlockAnalysis analysis = Analyze(root, position);

        Debug.Assert(position == analysis.Position);

        return GetTextChangeForNewLine(analysis, configOptions, indentation: indentation, options: options, cancellationToken: cancellationToken);
    }

    private static TextChange GetTextChangeForNewLine(
        TriviaBlockAnalysis analysis,
        AnalyzerConfigOptions configOptions,
        string indentation = null,
        string newLineReplacement = " ",
        CodeActionNewLineOptions options = CodeActionNewLineOptions.None,
        CancellationToken cancellationToken = default)
    {
        switch (analysis.Kind)
        {
            case TriviaBlockKind.NoNewLine:
                {
                    TriviaBlockAnalysis.Enumerator en = analysis.GetEnumerator();

                    int end = (en.ReadWhiteSpaceTrivia()) ? en.Current.Span.End : analysis.Position;

                    string endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(analysis.FirstOrSecond).ToString();

                    indentation ??= ((options & CodeActionNewLineOptions.IncreaseIndentation) != 0)
                        ? GetIncreasedIndentation(analysis.FirstOrSecond, configOptions, cancellationToken)
                        : SyntaxTriviaAnalysis.DetermineIndentation(analysis.FirstOrSecond, cancellationToken).ToString();

                    return new TextChange(TextSpan.FromBounds(analysis.Position, end), endOfLine + indentation);
                }
            case TriviaBlockKind.NewLine:
                {
                    TriviaBlockAnalysis.Enumerator en = analysis.GetEnumerator();

                    en.ReadWhiteSpaceOrEndOfLineTrivia();

                    return new TextChange(TextSpan.FromBounds(analysis.Position, en.Current.Span.End), newLineReplacement);
                }
            case TriviaBlockKind.BlankLine:
                {
                    TriviaBlockAnalysis.Enumerator en = analysis.GetEnumerator();

                    en.ReadTo(analysis.Position);
                    en.ReadWhiteSpaceOrEndOfLineTrivia();

                    return new TextChange(TextSpan.FromBounds(analysis.FirstOrSecond.Span.End, en.Current.Span.End), newLineReplacement);
                }
            default:
                {
                    throw new InvalidOperationException();
                }
        }
    }

    private static TriviaBlockAnalysis Analyze(SyntaxNode root, int position)
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

        if (first.IsKind(SyntaxKind.None))
        {
            return TriviaBlockAnalysis.FromLeading(second);
        }
        else if (second.IsKind(SyntaxKind.None))
        {
            return TriviaBlockAnalysis.FromTrailing(first);
        }
        else
        {
            return TriviaBlockAnalysis.FromBetween(first, second);
        }
    }

    private static string GetIncreasedIndentation(SyntaxNodeOrToken nodeOrToken, AnalyzerConfigOptions configOptions, CancellationToken cancellationToken)
    {
        SyntaxNode node = (nodeOrToken.IsNode) ? nodeOrToken.AsNode() : nodeOrToken.AsToken().Parent;

        SyntaxNode statementOrMember = node.FirstAncestorOrSelf(f => f is StatementSyntax
            || f is MemberDeclarationSyntax
            || f is SwitchLabelSyntax);

        return SyntaxTriviaAnalysis.GetIncreasedIndentation(statementOrMember ?? node, configOptions, cancellationToken);
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
