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

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExtractDeclarationFromUsingStatementCodeFixProvider))]
    [Shared]
    public class ExtractDeclarationFromUsingStatementCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.TypeUsedInUsingStatementMustBeImplicitlyConvertibleToIDisposable); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.ExtractDeclarationFromUsingStatement))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            UsingStatementSyntax usingStatement = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<UsingStatementSyntax>();

            Debug.Assert(usingStatement != null, $"{nameof(usingStatement)} is null");

            if (usingStatement == null
                || usingStatement.ContainsDiagnostics)
            {
                return;
            }

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.TypeUsedInUsingStatementMustBeImplicitlyConvertibleToIDisposable:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Extract declaration from using statement",
                                cancellationToken => ExtractDeclarationFromUsingStatementRefactoring.RefactorAsync(context.Document, usingStatement, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
