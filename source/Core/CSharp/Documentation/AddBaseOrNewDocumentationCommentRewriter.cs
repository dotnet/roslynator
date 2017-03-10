// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Documentation
{
    internal class AddBaseOrNewDocumentationCommentRewriter : AddNewDocumentationCommentRewriter
    {
        public AddBaseOrNewDocumentationCommentRewriter(DocumentationCommentGeneratorSettings settings, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
            : base(settings)
        {
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public SemanticModel SemanticModel { get; }
        public CancellationToken CancellationToken { get; }

        protected override MemberDeclarationSyntax AddDocumentationComment(MemberDeclarationSyntax memberDeclaration)
        {
            return DocumentationCommentGenerator.AddNewDocumentationComment(memberDeclaration, Settings, SemanticModel, CancellationToken);
        }
    }
}
