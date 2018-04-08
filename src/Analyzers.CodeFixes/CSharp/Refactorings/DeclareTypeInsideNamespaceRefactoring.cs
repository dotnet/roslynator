// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DeclareTypeInsideNamespaceRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Debug.Assert(member.IsParentKind(SyntaxKind.CompilationUnit), member.Parent?.Kind().ToString());

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string name = NameGenerator.Default.EnsureUniqueName(
                DefaultNames.Namespace,
                semanticModel.LookupNamespacesAndTypes(member.SpanStart));

            NamespaceDeclarationSyntax namespaceDeclaration = NamespaceDeclaration(
                IdentifierName(Identifier(name).WithRenameAnnotation()),
                default(SyntaxList<ExternAliasDirectiveSyntax>),
                default(SyntaxList<UsingDirectiveSyntax>),
                SingletonList(member));

            return await document.ReplaceNodeAsync(member, namespaceDeclaration.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
        }
    }
}
