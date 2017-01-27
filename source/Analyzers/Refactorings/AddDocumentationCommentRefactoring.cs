// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

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

        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            SyntaxTriviaList leadingTrivia = memberDeclaration.GetLeadingTrivia();

            int index = 0;

            string indent = "";

            if (leadingTrivia.Any())
            {
                index = leadingTrivia.Count - 1;

                for (int i = leadingTrivia.Count - 1; i >= 0; i--)
                {
                    if (leadingTrivia[i].IsWhitespaceTrivia())
                    {
                        index = i;
                    }
                    else
                    {
                        break;
                    }
                }

                indent = string.Join("", leadingTrivia.Skip(index));
            }

            DocumentationCommentGeneratorSettings settings = (indent.Length > 0)
                ? new DocumentationCommentGeneratorSettings(indent)
                : null;

            SyntaxTriviaList comment = DocumentationCommentGenerator.Generate(memberDeclaration, settings);

            SyntaxTriviaList newLeadingTrivia = leadingTrivia.InsertRange(index, comment);

            MemberDeclarationSyntax newNode = memberDeclaration.WithLeadingTrivia(newLeadingTrivia);

            return document.ReplaceNodeAsync(memberDeclaration, newNode, cancellationToken);
        }
    }
}
