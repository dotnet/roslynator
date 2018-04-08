// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RenameFieldAccordingToCamelCaseWithUnderscoreCodeFixProvider))]
    [Shared]
    public class RenameFieldAccordingToCamelCaseWithUnderscoreCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out VariableDeclaratorSyntax declarator))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

            string oldName = declarator.Identifier.ValueText;
            string newName = StringUtility.ToCamelCase(oldName, prefixWithUnderscore: true);

            newName = NameGenerator.Default.EnsureUniqueMemberName(
                newName,
                semanticModel,
                declarator.Identifier.SpanStart,
                cancellationToken: context.CancellationToken);

            CodeAction codeAction = CodeAction.Create(
                $"Rename '{oldName}' to '{newName}'",
                cancellationToken => RenamePrivateFieldAccordingToCamelCaseWithUnderscoreRefactoring.RefactorAsync(context.Document, symbol, newName, cancellationToken),
                DiagnosticIdentifiers.RenamePrivateFieldAccordingToCamelCaseWithUnderscore);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}