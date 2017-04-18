// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddReturnStatementThatReturnsDefaultValueRefactoring
    {
        public static bool CanRefactor(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (methodDeclaration.Body != null)
            {
                TypeSyntax returnType = methodDeclaration.ReturnType;

                if (returnType?.IsMissing == false)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(returnType, cancellationToken);

                    return typeSymbol?.IsErrorType() == false
                        && !typeSymbol.IsIEnumerableOrConstructedFromIEnumerableOfT()
                        && !typeSymbol.IsVoid()
                        && semanticModel.ContainsCompilerDiagnostic(CSharpErrorCodes.NotAllCodePathsReturnValue, methodDeclaration.Identifier.Span, cancellationToken);
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            MethodDeclarationSyntax methodDeclaration,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(methodDeclaration.ReturnType, cancellationToken);

            int position = -1;
            BlockSyntax body = methodDeclaration.Body;
            SyntaxList<StatementSyntax> statements = body.Statements;

            if (statements.Any())
            {
                position = statements.Last().FullSpan.End;
            }
            else
            {
                position = body.OpenBraceToken.FullSpan.End;
            }

            MethodDeclarationSyntax newNode = methodDeclaration
                .AddBodyStatements(ReturnStatement(typeSymbol.ToDefaultValueSyntax(semanticModel, position)));

            return await document.ReplaceNodeAsync(methodDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}