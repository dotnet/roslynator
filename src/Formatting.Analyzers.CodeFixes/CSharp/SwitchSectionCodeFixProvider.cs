// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using System.Xml.Linq;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SwitchSectionCodeFixProvider))]
[Shared]
public sealed class SwitchSectionCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get
        {
            return ImmutableArray.Create(
                DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections,
                DiagnosticIdentifiers.BlankLineBetweenSwitchSections);
        }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out SwitchSectionSyntax switchSection))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.AddBlankLineBetweenSwitchSections:
                {
                    CodeAction codeAction = CodeAction.Create(
                        CodeFixTitles.AddNewLine,
                        ct => CodeFixHelpers.AppendEndOfLineAsync(document, switchSection, ct),
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
            case DiagnosticIdentifiers.BlankLineBetweenSwitchSections:
                {
                    CodeAction codeAction = CodeAction.Create(
                        CodeFixTitles.AddBlankLine,
                        ct =>
                        {
                            SyntaxTriviaList leading = switchSection.GetLeadingTrivia();

                            if (leading.Span.Contains(context.Span))
                            {
                                SyntaxTriviaList newLeading = leading.TrimStart();

                                if (!newLeading.Any()
                                    && leading.Last().IsKind(SyntaxKind.WhitespaceTrivia))
                                {
                                    newLeading = SyntaxFactory.TriviaList(leading.Last());
                                }

                                SwitchSectionSyntax newSwitchSection = switchSection.WithLeadingTrivia(newLeading);

                                return document.ReplaceNodeAsync(switchSection, newSwitchSection, ct);
                            }
                            else
                            {
                                var switchStatement = (SwitchStatementSyntax)switchSection.Parent;
                                int index = switchStatement.Sections.IndexOf(switchSection);
                                SwitchSectionSyntax nextSection = switchStatement.Sections[index + 1];
                                SyntaxTriviaList trailing = switchSection.GetTrailingTrivia();
                                leading = nextSection.GetLeadingTrivia();

                                SyntaxTrivia endOfLine = SyntaxTriviaAnalysis.DetermineEndOfLine(switchSection);
                                SyntaxTriviaList newLeading = leading.Insert(0, endOfLine);

                                SwitchStatementSyntax newSwitchStatement = switchStatement.ReplaceNode(nextSection, nextSection.WithLeadingTrivia(newLeading));

                                if (!trailing.Last().IsKind(SyntaxKind.EndOfLineTrivia))
                                {
                                    SyntaxTriviaList newTrailing = trailing.Add(endOfLine);

                                    switchSection = newSwitchStatement.Sections[index];
                                    newSwitchStatement = newSwitchStatement.ReplaceNode(switchSection, switchSection.WithTrailingTrivia(newTrailing));
                                }

                                return document.ReplaceNodeAsync(switchStatement, newSwitchStatement, ct);
                            }
                        },
                        GetEquivalenceKey(diagnostic));

                    context.RegisterCodeFix(codeAction, diagnostic);
                    break;
                }
        }
    }
}
