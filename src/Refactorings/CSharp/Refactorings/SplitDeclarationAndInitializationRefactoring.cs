// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SplitDeclarationAndInitializationRefactoring
    {
        public static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            LocalDeclarationStatementSyntax localDeclaration)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

            if (!statementsInfo.Success)
                return;

            SingleLocalDeclarationStatementInfo localInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(localDeclaration);

            if (!localInfo.Success)
                return;

            if (!context.Span.IsEmptyAndContainedInSpan(localInfo.EqualsToken))
                return;

            ExpressionSyntax value = localInfo.Value;

            if (value == null)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            TypeSyntax type = localInfo.Type;

            if (type.IsVar)
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(value, context.CancellationToken);

                if (typeSymbol?.SupportsExplicitDeclaration() != true)
                    return;

                type = typeSymbol.ToMinimalTypeSyntax(semanticModel, type.SpanStart);
            }
            else
            {
                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, context.CancellationToken);

                if (typeSymbol?.IsErrorType() != false)
                    return;
            }

            context.RegisterRefactoring(
                "Split declaration and initialization",
                ct => RefactorAsync(context.Document, localInfo, type, statementsInfo, ct));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SingleLocalDeclarationStatementInfo localInfo,
            TypeSyntax type,
            StatementListInfo statementsInfo,
            CancellationToken cancellationToken)
        {
            LocalDeclarationStatementSyntax localStatement = localInfo.Statement;

            int index = statementsInfo.IndexOf(localStatement);

            VariableDeclaratorSyntax declarator = localInfo.Declarator;

            VariableDeclaratorSyntax newDeclarator = declarator.WithInitializer(null);

            VariableDeclarationSyntax newDeclaration = localInfo.Declaration.ReplaceNode(declarator, newDeclarator);

            if (type != null)
                newDeclaration = newDeclaration.WithType(type.WithTriviaFrom(newDeclaration.Type));

            LocalDeclarationStatementSyntax newLocalStatement = localStatement
                .WithDeclaration(newDeclaration)
                .WithTrailingTrivia(NewLine())
                .WithFormatterAnnotation();

            ExpressionStatementSyntax assignmentStatement = SimpleAssignmentStatement(IdentifierName(localInfo.Identifier), localInfo.Initializer.Value)
                .WithTrailingTrivia(localStatement.GetTrailingTrivia())
                .WithFormatterAnnotation();

            StatementListInfo newStatementsInfo = statementsInfo
                .Insert(index + 1, assignmentStatement)
                .ReplaceAt(index, newLocalStatement);

            return document.ReplaceStatementsAsync(statementsInfo, newStatementsInfo, cancellationToken);
        }
    }
}
