// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CallFindInsteadOfFirstOrDefaultRefactoring
    {
        public static void Analyze(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocation,
            MemberAccessExpressionSyntax memberAccess)
        {
            SemanticModel semanticModel = context.SemanticModel;
            CancellationToken cancellationToken = context.CancellationToken;

            if (semanticModel.TryGetExtensionMethodInfo(invocation, out MethodInfo methodInfo, ExtensionMethodKind.Reduced, cancellationToken)
                && methodInfo.IsLinqExtensionOfIEnumerableOfTWithPredicate("FirstOrDefault"))
            {
                ExpressionSyntax expression = memberAccess.Expression;

                if (expression != null)
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

                    if (typeSymbol != null)
                    {
                        if (typeSymbol.IsConstructedFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Generic_List_T)))
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.CallFindInsteadOfFirstOrDefault,
                                memberAccess.Name);
                        }
                        else if (typeSymbol.IsArrayType())
                        {
                            var arrayType = (IArrayTypeSymbol)typeSymbol;

                            if (arrayType.Rank == 1)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.CallFindInsteadOfFirstOrDefault,
                                    memberAccess.Name);
                            }
                        }
                    }
                }
            }
        }

        internal static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocation,
            CancellationToken cancellationToken)
        {
            var memberAccess = (MemberAccessExpressionSyntax)invocation.Expression;
            ExpressionSyntax expression = memberAccess.Expression;
            SimpleNameSyntax name = memberAccess.Name;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, cancellationToken);

            if (typeSymbol.IsConstructedFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Collections_Generic_List_T)))
            {
                IdentifierNameSyntax newName = IdentifierName("Find").WithTriviaFrom(name);

                return await document.ReplaceNodeAsync(name, newName, cancellationToken).ConfigureAwait(false);
            }
            else if (typeSymbol.IsArrayType()
                && ((IArrayTypeSymbol)typeSymbol).Rank == 1)
            {
                NameSyntax arrayName = ParseName("System.Array")
                    .WithLeadingTrivia(invocation.GetLeadingTrivia())
                    .WithSimplifierAnnotation();

                MemberAccessExpressionSyntax newMemberAccess = SimpleMemberAccessExpression(
                    arrayName,
                    memberAccess.OperatorToken,
                    IdentifierName("Find").WithTriviaFrom(name));

                ArgumentListSyntax argumentList = invocation.ArgumentList;

                InvocationExpressionSyntax newInvocation = InvocationExpression(
                    newMemberAccess,
                    ArgumentList(
                        Argument(expression.WithoutTrivia()),
                        argumentList.Arguments.First()
                    ).WithTriviaFrom(argumentList));

                return await document.ReplaceNodeAsync(invocation, newInvocation, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                Debug.Fail(typeSymbol.ToString());
                return document;
            }
        }
    }
}
