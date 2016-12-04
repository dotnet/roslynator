// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddDefaultValueToParameterRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (parameter.Type != null
                && !parameter.Identifier.IsMissing
                && context.Span.Start >= parameter.Identifier.Span.Start)
            {
                EqualsValueClauseSyntax @default = parameter.Default;

                if (@default == null
                    || @default.IsMissing
                    || @default.Value == null
                    || @default.Value.IsMissing)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(parameter.Type, context.CancellationToken);

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        context.RegisterRefactoring(
                            "Add default value",
                            cancellationToken => RefactorAsync(context.Document, parameter, typeSymbol, cancellationToken));
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ParameterSyntax parameter,
            ITypeSymbol typeSymbol,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ParameterSyntax newParameter = GetNewParameter(parameter, typeSymbol);

            return await document.ReplaceNodeAsync(parameter, newParameter, cancellationToken).ConfigureAwait(false);
        }

        private static ParameterSyntax GetNewParameter(
            ParameterSyntax parameter,
            ITypeSymbol typeSymbol)
        {
            ExpressionSyntax value = CSharpFactory.DefaultValue(
                typeSymbol,
                parameter.Type.WithoutTrivia());

            EqualsValueClauseSyntax @default = EqualsValueClause(value);

            if (parameter.Default == null || parameter.IsMissing)
            {
                return parameter
                    .WithIdentifier(parameter.Identifier.WithoutTrailingTrivia())
                    .WithDefault(@default.WithTrailingTrivia(parameter.Identifier.TrailingTrivia));
            }
            else
            {
                return parameter
                    .WithDefault(@default.WithTriviaFrom(parameter.Default.EqualsToken));
            }
        }
    }
}
