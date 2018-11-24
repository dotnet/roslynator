// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Documentation;
using Roslynator.Documentation;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDocumentationCommentRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            bool copyCommentFromBaseIfAvailable,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationSyntax newNode = null;

            if (copyCommentFromBaseIfAvailable
                && DocumentationCommentGenerator.CanGenerateFromBase(memberDeclaration.Kind()))
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                DocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(memberDeclaration, semanticModel, cancellationToken);

                if (data.Success)
                {
                    SyntaxTrivia comment = DocumentationCommentTriviaFactory.Parse(data.RawXml, semanticModel, memberDeclaration.SpanStart);

                    newNode = memberDeclaration.WithDocumentationComment(comment, indent: true);
                }
            }

            newNode = newNode ?? memberDeclaration.WithNewSingleLineDocumentationComment();

            return await document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
