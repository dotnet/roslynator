// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Documentation
{
    internal class AddBaseOrNewDocumentationCommentRewriter : AddNewDocumentationCommentRewriter
    {
        public AddBaseOrNewDocumentationCommentRewriter(SemanticModel semanticModel, DocumentationCommentGeneratorSettings settings = null, bool skipNamespaceDeclaration = true, CancellationToken cancellationToken = default(CancellationToken))
            : base(settings, skipNamespaceDeclaration)
        {
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;
        }

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        protected override MemberDeclarationSyntax AddDocumentationComment(MemberDeclarationSyntax memberDeclaration)
        {
            return memberDeclaration.WithBaseOrNewSingleLineDocumentationComment(SemanticModel, Settings, CancellationToken);
        }
    }
}
