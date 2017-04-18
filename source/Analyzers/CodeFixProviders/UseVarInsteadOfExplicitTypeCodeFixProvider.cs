// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseVarInsteadOfExplicitTypeCodeFixProvider))]
    [Shared]
    public class UseVarInsteadOfExplicitTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsObvious,
                    DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeWhenTypeIsNotObvious,
                    DiagnosticIdentifiers.UseVarInsteadOfExplicitTypeInForEach);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            TypeSyntax type = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<TypeSyntax>();

            Debug.Assert(type != null, $"{nameof(type)} is null");

            if (type == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                CodeAction codeAction = CodeAction.Create(
                    "Change type to 'var'",
                    cancellationToken => ChangeTypeRefactoring.ChangeTypeToVarAsync(context.Document, type, cancellationToken),
                    diagnostic.Id + EquivalenceKeySuffix);

                context.RegisterCodeFix(codeAction, context.Diagnostics);
            }
        }
    }
}