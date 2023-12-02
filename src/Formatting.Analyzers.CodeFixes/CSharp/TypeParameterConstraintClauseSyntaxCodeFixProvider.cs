// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        SyntaxNode member = root
            .FindNode(context.Span)?
            .FirstAncestorOrSelf(node => node is MemberDeclarationSyntax || node.IsKind(SyntaxKind.LocalFunctionStatement));

        string indentation = SyntaxTriviaAnalysis.GetIncreasedIndentation(member, context.Document.GetConfigOptions(member.SyntaxTree), context.CancellationToken);

        await CodeActionFactory.CreateAndRegisterCodeActionForNewLineAsync(context, indentation: indentation).ConfigureAwait(false);
    }
}
