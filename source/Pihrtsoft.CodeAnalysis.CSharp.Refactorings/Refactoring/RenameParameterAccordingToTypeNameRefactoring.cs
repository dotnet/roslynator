// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class RenameParameterAccordingToTypeNameRefactoring
    {
        public static void Refactor(
            CodeRefactoringContext context,
            ParameterSyntax parameter,
            SemanticModel semanticModel)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            if (parameter.Type == null)
                return;

            if (parameter.Identifier.IsMissing)
            {
                if (TextSpan.FromBounds(parameter.Type.Span.End, parameter.Span.End).Contains(context.Span))
                {
                    string name = CreateParameterName(parameter, semanticModel);
                    if (name != null)
                    {
                        context.RegisterRefactoring(
                            $"Add parameter name '{name}'",
                            cancellationToken => AddParameterNameAccordingToTypeNameRefactorAsync(context.Document, parameter, name, cancellationToken));
                    }
                }
            }
            else
            {
                if (parameter.Identifier.Span.Contains(context.Span))
                {
                    string name = CreateParameterName(parameter, semanticModel);
                    if (name != null)
                    {
                        ISymbol symbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                        context.RegisterRefactoring(
                            $"Rename parameter to '{name}'",
                            cancellationToken => symbol.RenameAsync(name, context.Document, cancellationToken));
                    }
                }
            }
        }

        private static string CreateParameterName(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            string name = NamingHelper.CreateIdentifierName(
                parameter.Type,
                semanticModel,
                firstCharToLower: true);

            if (string.Equals(parameter.Identifier.ToString(), name, StringComparison.Ordinal))
                return null;

            return name;
        }

        private static async Task<Document> AddParameterNameAccordingToTypeNameRefactorAsync(
            Document document,
            ParameterSyntax parameter,
            string name,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ParameterSyntax newParameter = parameter
                .WithType(parameter.Type.WithoutTrailingTrivia())
                .WithIdentifier(SyntaxFactory.Identifier(name).WithTrailingTrivia(parameter.Type.GetTrailingTrivia()))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(parameter, newParameter);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
