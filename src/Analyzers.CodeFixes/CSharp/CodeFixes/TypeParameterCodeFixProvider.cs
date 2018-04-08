// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;
using Roslynator.CSharp.Refactorings.DocumentationComment;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeParameterCodeFixProvider))]
    [Shared]
    public class TypeParameterCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.AddTypeParameterToDocumentationComment,
                    DiagnosticIdentifiers.UnusedTypeParameter);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeParameterSyntax typeParameter))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.AddTypeParameterToDocumentationComment:
                        {
                            var refactoring = new AddTypeParameterToDocumentationCommentRefactoring();

                            CodeAction codeAction = CodeAction.Create(
                                "Add type parameter to documentation comment",
                                cancellationToken => refactoring.RefactorAsync(context.Document, typeParameter, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UnusedTypeParameter:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove unused type parameter '{typeParameter.Identifier}'",
                                cancellationToken => UnusedParameterRefactoring.RefactorAsync(context.Document, typeParameter, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
