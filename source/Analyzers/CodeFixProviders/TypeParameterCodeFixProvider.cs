// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.DocumentationComment;
using Roslynator.CSharp.Refactorings.UnusedSyntax;

namespace Roslynator.CSharp.CodeFixProviders
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

            TypeParameterSyntax typeParameter = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<TypeParameterSyntax>();

            Debug.Assert(typeParameter != null, $"{nameof(typeParameter)} is null");

            if (typeParameter == null)
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
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.UnusedTypeParameter:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                $"Remove unused type parameter '{typeParameter.Identifier}'",
                                cancellationToken => UnusedTypeParameterRefactoring.RefactorAsync(context.Document, typeParameter, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
