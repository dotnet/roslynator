// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifyNullableOfTCodeFixProvider))]
    [Shared]
    public class SimplifyNullableOfTCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.SimplifyNullableOfT); }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            TypeSyntax type = root
                .FindNode(context.Span, findInsideTrivia: true, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<TypeSyntax>();

            if (type == null)
                return;

            TypeSyntax nullableType = GetNullableType(type);

            CodeAction codeAction = CodeAction.Create(
                $"Simplify name '{type}'",
                cancellationToken => SimplifyNullableOfTRefactoring.RefactorAsync(context.Document, type, nullableType, cancellationToken),
                DiagnosticIdentifiers.SimplifyNullableOfT + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static TypeSyntax GetNullableType(TypeSyntax type)
        {
            if (type.IsKind(SyntaxKind.QualifiedName))
                type = ((QualifiedNameSyntax)type).Right;

            return ((GenericNameSyntax)type).TypeArgumentList.Arguments[0];
        }
    }
}
