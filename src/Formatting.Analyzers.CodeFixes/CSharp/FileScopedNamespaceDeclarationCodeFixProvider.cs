// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;
using Roslynator.CSharp.CodeStyle;
using System.Diagnostics;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileScopedNamespaceDeclarationCodeFixProvider))]
[Shared]
public sealed class FileScopedNamespaceDeclarationCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out FileScopedNamespaceDeclarationSyntax fileScopedNamespace))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        switch (diagnostic.Id)
        {
            case DiagnosticIdentifiers.BlankLineAfterFileScopedNamespaceDeclaration:
                {
                    MemberDeclarationSyntax member = fileScopedNamespace.Members[0];

                    BlankLineStyle style = BlankLineAfterFileScopedNamespaceDeclarationAnalyzer.GetCurrentStyle(fileScopedNamespace, member);

                    if (style == BlankLineStyle.Add)
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.AddBlankLine,
                            ct =>
                            {
                                MemberDeclarationSyntax newMember;
                                if (!fileScopedNamespace.SemicolonToken.TrailingTrivia.Contains(SyntaxKind.EndOfLineTrivia))
                                {
                                    newMember = member.PrependToLeadingTrivia(new SyntaxTrivia[] { CSharpFactory.NewLine(), CSharpFactory.NewLine() });
                                }
                                else
                                {
                                    newMember = member.PrependEndOfLineToLeadingTrivia();
                                }

                                return document.ReplaceNodeAsync(member, newMember, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                    }
                    else if (style == BlankLineStyle.Remove)
                    {
                        CodeAction codeAction = CodeAction.Create(
                            CodeFixTitles.RemoveBlankLine,
                            ct => CodeFixHelpers.RemoveBlankLinesBeforeAsync(document, member.GetFirstToken(), ct),
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                    }
                    else
                    {
                        Debug.Fail("");
                    }

                    break;
                }
        }
    }
}
