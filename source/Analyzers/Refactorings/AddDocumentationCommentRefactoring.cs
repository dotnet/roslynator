// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Documentation;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDocumentationCommentRefactoring
    {
        public static bool CanRefactor(ClassDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(ConstructorDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(ConversionOperatorDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Type, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(DelegateDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(DestructorDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(EnumDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(EventDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(EventFieldDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Declaration, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(FieldDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Declaration, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(IndexerDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.ThisKeyword, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(InterfaceDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(MethodDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(OperatorDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.OperatorToken, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(PropertyDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        public static bool CanRefactor(StructDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            return CanRefactor(declaration.Identifier, semanticModel, cancellationToken);
        }

        private static bool CanRefactor(VariableDeclarationSyntax declaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            VariableDeclaratorSyntax declarator = declaration?.Variables.FirstOrDefault();

            return declarator != null
                && CanRefactor(declarator, semanticModel, cancellationToken);
        }

        private static bool CanRefactor(SyntaxNode node, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return node != null
                && CanRefactor(node.Span, semanticModel, cancellationToken);
        }

        private static bool CanRefactor(SyntaxToken token, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return CanRefactor(token.Span, semanticModel, cancellationToken);
        }

        private static bool CanRefactor(TextSpan span, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return semanticModel.ContainsCompilerDiagnostic(CSharpErrorCodes.MissingXmlComment, span, cancellationToken);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            bool copyCommentFromBaseIfAvailable,
            CancellationToken cancellationToken)
        {
            MemberDeclarationSyntax newNode = null;

            if (copyCommentFromBaseIfAvailable
                && DocumentationCommentGenerator.CanGenerateFromBase(memberDeclaration.Kind()))
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                BaseDocumentationCommentData data = DocumentationCommentGenerator.GenerateFromBase(memberDeclaration, semanticModel, cancellationToken);

                if (data.Success)
                    newNode = memberDeclaration.WithDocumentationComment(data.Comment, indent: true);
            }

            newNode = newNode ?? memberDeclaration.WithNewSingleLineDocumentationComment();

            return await document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
