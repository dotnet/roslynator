// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.Formatting.CSharp;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterConstraintClauseSyntaxCodeFixProvider))]
[Shared]
public sealed class TypeParameterConstraintClauseSyntaxCodeFixProvider : BaseCodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds
    {
        get { return ImmutableArray.Create(DiagnosticIdentifiers.PutTypeParameterConstraintOnItsOwnLine); }
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterConstraintClauseSyntax constraintClause))
            return;

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];

        CodeAction codeAction = CodeAction.Create(
            CodeFixTitles.AddNewLine,
            ct =>
            {
                AnalyzerConfigOptions configOptions = document.GetConfigOptions(constraintClause.SyntaxTree);

                return CodeFixHelpers.AddNewLineBeforeAndIncreaseIndentationAsync(
                    document,
                    constraintClause.WhereKeyword,
                    SyntaxTriviaAnalysis.AnalyzeIndentation(constraintClause.Parent, configOptions, ct),
                    ct);
            },
            GetEquivalenceKey(diagnostic));

        context.RegisterCodeFix(codeAction, diagnostic);
    }
}
