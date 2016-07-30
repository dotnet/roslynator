//// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
//using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

//TODO: 
//namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
//{
//    internal static class AddConfigureAwaitRefactoring
//    {
//        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InvocationExpressionSyntax invocation)
//        {
//            if (invocation.Parent?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == false)
//            {
//                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

//                IMethodSymbol methodSymbol = GetMethodOrLambdaSymbol(invocation, semanticModel, context.CancellationToken);

//                if (methodSymbol?.IsAsync == true
//                    && IsAwaitable(invocation, semanticModel, context.CancellationToken))
//                {
//                    context.RegisterRefactoring(
//                        "Add 'ConfigureAwait(false)'",
//                        cancellationToken =>
//                        {
//                            return RefactorAsync(
//                                context.Document,
//                                invocation,
//                                context.CancellationToken);
//                        });
//                }
//            }
//        }

//        public static bool IsAwaitable(
//            InvocationExpressionSyntax invocation,
//            SemanticModel semanticModel,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            INamedTypeSymbol taskSymbol = semanticModel
//                .Compilation
//                .GetTypeByMetadataName("System.Threading.Tasks.Task");

//            if (taskSymbol != null)
//            {
//                ISymbol symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol;

//                if (symbol?.IsMethod() == true)
//                {
//                    var methodSymbol = (IMethodSymbol)symbol;

//                    if (methodSymbol.ReturnType.Equals(taskSymbol))
//                        return true;

//                    if (methodSymbol.ReturnType.IsNamedType()
//                        && ((INamedTypeSymbol)methodSymbol.ReturnType)
//                            .ConstructedFrom
//                            .BaseTypes()
//                            .Any(baseType => baseType.Equals(taskSymbol)))
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        private static IMethodSymbol GetMethodOrLambdaSymbol(
//            SyntaxNode node,
//            SemanticModel semanticModel,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            foreach (SyntaxNode ancestor in node.Ancestors())
//            {
//                switch (ancestor.Kind())
//                {
//                    case SyntaxKind.MethodDeclaration:
//                        {
//                            return semanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)ancestor, cancellationToken);
//                        }
//                    case SyntaxKind.SimpleLambdaExpression:
//                    case SyntaxKind.ParenthesizedLambdaExpression:
//                        {
//                            return semanticModel.GetSymbolInfo(ancestor, cancellationToken).Symbol as IMethodSymbol;
//                        }
//                    case SyntaxKind.PropertyDeclaration:
//                    case SyntaxKind.IndexerDeclaration:
//                    case SyntaxKind.AnonymousMethodExpression:
//                    case SyntaxKind.ConstructorDeclaration:
//                    case SyntaxKind.DestructorDeclaration:
//                    case SyntaxKind.EventDeclaration:
//                    case SyntaxKind.ConversionOperatorDeclaration:
//                    case SyntaxKind.OperatorDeclaration:
//                    case SyntaxKind.IncompleteMember:
//                        return null;
//                }
//            }

//            return null;
//        }

//        private static async Task<Document> RefactorAsync(
//            Document document,
//            InvocationExpressionSyntax invocation,
//            CancellationToken cancellationToken = default(CancellationToken))
//        {
//            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

//            InvocationExpressionSyntax newInvocation = InvocationExpression(
//                SimpleMemberAccessExpression(invocation.WithoutTrailingTrivia(), IdentifierName("ConfigureAwait")),
//                ArgumentList(Argument(FalseLiteralExpression())));

//            newInvocation = newInvocation.WithTrailingTrivia(invocation.GetTrailingTrivia());

//            root = root.ReplaceNode(invocation, newInvocation);

//            return document.WithSyntaxRoot(root);
//        }
//    }
//}
