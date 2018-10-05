// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractEventHandlerMethodRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
        {
            ParameterListSyntax parameterList = parenthesizedLambda.ParameterList;

            if (parameterList.Parameters.Count != 2)
                return;

            if (!(parenthesizedLambda.WalkUpParentheses().Parent is AssignmentExpressionSyntax assignmentExpression))
                return;

            if (assignmentExpression.Kind() != SyntaxKind.AddAssignmentExpression)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            IMethodSymbol methodSymbol = semanticModel.GetMethodSymbol(assignmentExpression, context.CancellationToken);

            if (methodSymbol?.MethodKind != MethodKind.EventAdd)
                return;

            if (!(semanticModel.GetSymbol(assignmentExpression.Left, context.CancellationToken) is IEventSymbol eventSymbol))
                return;

            MemberDeclarationSyntax memberDeclaration = assignmentExpression.FirstAncestor<MemberDeclarationSyntax>();

            if (memberDeclaration == null)
                return;

            Debug.Assert(memberDeclaration.Parent is TypeDeclarationSyntax);

            if (!(memberDeclaration.Parent is TypeDeclarationSyntax typeDeclaration))
                return;

            context.CancellationToken.ThrowIfCancellationRequested();

            context.RegisterRefactoring(
                "Extract event handler method",
                ct => RefactorAsync(context.Document, parenthesizedLambda, memberDeclaration, typeDeclaration, eventSymbol.Name, semanticModel, ct),
                RefactoringIdentifiers.ExtractEventHandlerMethod);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ParenthesizedLambdaExpressionSyntax parenthesizedLambda,
            MemberDeclarationSyntax memberDeclaration,
            TypeDeclarationSyntax typeDeclaration,
            string methodName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            methodName = NameGenerator.Default.EnsureUniqueLocalName(methodName, semanticModel, parenthesizedLambda.SpanStart, cancellationToken: cancellationToken);

            MemberDeclarationSyntax newMemberDeclaration = memberDeclaration.ReplaceNode(parenthesizedLambda, IdentifierName(methodName).WithTriviaFrom(parenthesizedLambda));

            IMethodSymbol lambdaSymbol = semanticModel.GetMethodSymbol(parenthesizedLambda, cancellationToken);

            ParameterListSyntax parameterList = parenthesizedLambda.ParameterList;

            SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

            ImmutableArray<IParameterSymbol> parameterSymbols = lambdaSymbol.Parameters;

            parameters = parameters
                .ReplaceAt(0, AddTypeIfMissing(parameters[0], parameterSymbols[0]))
                .ReplaceAt(1, AddTypeIfMissing(parameters[1], parameterSymbols[1]));

            cancellationToken.ThrowIfCancellationRequested();

            MethodDeclarationSyntax newMethodDeclaration = MethodDeclaration(
                (SyntaxInfo.ModifierListInfo(memberDeclaration).IsStatic) ? Modifiers.Private_Static() : Modifiers.Private(),
                VoidType(),
                Identifier(methodName).WithRenameAnnotation(),
                parameterList.WithParameters(parameters),
                CreateMethodBody(parenthesizedLambda.Body)).WithFormatterAnnotation();

            SyntaxList<MemberDeclarationSyntax> newMembers = typeDeclaration.Members.Replace(memberDeclaration, newMemberDeclaration);

            newMembers = MemberDeclarationInserter.Default.Insert(newMembers, newMethodDeclaration);

            return document.ReplaceNodeAsync(typeDeclaration, typeDeclaration.WithMembers(newMembers), cancellationToken);

            BlockSyntax CreateMethodBody(CSharpSyntaxNode lambdaBody)
            {
                switch (lambdaBody)
                {
                    case BlockSyntax block:
                        return block;
                    case ExpressionSyntax expression:
                        return Block(ExpressionStatement(expression));
                    default:
                        return Block();
                }
            }

            ParameterSyntax AddTypeIfMissing(ParameterSyntax parameter, IParameterSymbol parameterSymbol)
            {
                TypeSyntax type = parameter.Type;

                if (type?.IsMissing == false)
                    return parameter;

                type = parameterSymbol.Type.ToMinimalTypeSyntax(semanticModel, typeDeclaration.OpenBraceToken.Span.End);

                return parameter.WithType(type);
            }
        }
    }
}