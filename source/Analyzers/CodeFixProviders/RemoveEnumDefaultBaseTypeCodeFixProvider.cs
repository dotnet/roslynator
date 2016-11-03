// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemoveEnumDefaultBaseTypeCodeFixProvider))]
    [Shared]
    public class RemoveEnumDefaultBaseTypeCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType);

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            SimpleBaseTypeSyntax baseType = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SimpleBaseTypeSyntax>();

            if (baseType == null)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Remove default underlying type",
                cancellationToken => RemoveDefaultUnderlyingTypeAsync(context.Document, baseType, cancellationToken),
                DiagnosticIdentifiers.RemoveEnumDefaultUnderlyingType + EquivalenceKeySuffix);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static async Task<Document> RemoveDefaultUnderlyingTypeAsync(
            Document document,
            BaseTypeSyntax baseType,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var baseList = (BaseListSyntax)baseType.Parent;
            var enumDeclaration = (EnumDeclarationSyntax)baseList.Parent;

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration
                .RemoveNode(GetNodeToRemove(baseType, baseList), SyntaxRemoveOptions.KeepExteriorTrivia)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(enumDeclaration, newEnumDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode GetNodeToRemove(BaseTypeSyntax baseType, BaseListSyntax baseList)
        {
            if (baseList.Types.Count == 1)
                return baseList;

            return baseType;
        }
    }
}
