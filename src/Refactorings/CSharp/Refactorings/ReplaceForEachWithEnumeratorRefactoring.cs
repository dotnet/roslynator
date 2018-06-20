// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceForEachWithEnumeratorRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ForEachStatementSyntax forEachStatement)
        {
            if (forEachStatement.Expression?.IsMissing != false)
                return;

            if (forEachStatement.Statement?.IsMissing != false)
                return;

            context.RegisterRefactoring(
                "Replace foreach with enumerator",
                ct => RefactorAsync(context.Document, forEachStatement, ct),
                RefactoringIdentifiers.ReplaceForEachWithEnumerator);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ForEachStatementSyntax forEachStatement,
            CancellationToken cancellationToken)
        {
            int position = forEachStatement.SpanStart;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string name = NameGenerator.Default.EnsureUniqueLocalName(DefaultNames.EnumeratorVariable, semanticModel, position, cancellationToken: cancellationToken);

            InvocationExpressionSyntax expression = SimpleMemberInvocationExpression(forEachStatement.Expression, IdentifierName(WellKnownMemberNames.GetEnumeratorMethodName));

            VariableDeclarationSyntax variableDeclaration = VariableDeclaration(VarType(), Identifier(name).WithRenameAnnotation(), expression);

            MemberAccessExpressionSyntax currentExpression = SimpleMemberAccessExpression(IdentifierName(name), IdentifierName("Current"));

            ILocalSymbol localSymbol = semanticModel.GetDeclaredSymbol(forEachStatement, cancellationToken);

            StatementSyntax statement = forEachStatement.Statement;

            StatementSyntax newStatement = statement.ReplaceNodes(
            statement
                .DescendantNodes()
                .Where(node => node.Kind() == SyntaxKind.IdentifierName && localSymbol.Equals(semanticModel.GetSymbol(node, cancellationToken))),
            (node, _) => currentExpression.WithTriviaFrom(node));

            WhileStatementSyntax whileStatement = WhileStatement(
                SimpleMemberInvocationExpression(IdentifierName(name), IdentifierName("MoveNext")),
                newStatement);

            if (semanticModel
                .GetSpeculativeMethodSymbol(position, expression)?
                .ReturnType
                .Implements(SpecialType.System_IDisposable, allInterfaces: true) == true)
            {
                UsingStatementSyntax usingStatement = UsingStatement(
                    variableDeclaration,
                    default(ExpressionSyntax),
                    Block(whileStatement));

                usingStatement = usingStatement
                    .WithLeadingTrivia(forEachStatement.GetLeadingTrivia())
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(forEachStatement, usingStatement, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                LocalDeclarationStatementSyntax localDeclaration = LocalDeclarationStatement(variableDeclaration)
                    .WithLeadingTrivia(forEachStatement.GetLeadingTrivia())
                    .WithFormatterAnnotation();

                var newStatements = new StatementSyntax[] { localDeclaration, whileStatement.WithFormatterAnnotation() };

                return await document.ReplaceNodeAsync(forEachStatement, newStatements, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}