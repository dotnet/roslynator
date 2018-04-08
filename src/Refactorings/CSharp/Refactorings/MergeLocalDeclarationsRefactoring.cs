// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeLocalDeclarationsRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Count <= 1)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!AreLocalDeclarations(selectedStatements, semanticModel, context.CancellationToken))
                return;

            context.RegisterRefactoring(
                "Merge local declarations",
                cancellationToken => RefactorAsync(context.Document, selectedStatements, cancellationToken));
        }

        private static bool AreLocalDeclarations(
            StatementListSelection statements,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol prevTypeSymbol = null;

            for (int i = 0; i < statements.Count; i++)
            {
                StatementSyntax statement = statements[i];

                if (!(statement is LocalDeclarationStatementSyntax localDeclaration))
                    return false;

                TypeSyntax type = localDeclaration.Declaration?.Type;

                if (type == null)
                    return false;

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                if (typeSymbol == null)
                    return false;

                if (typeSymbol.IsErrorType())
                    return false;

                if (prevTypeSymbol != null && prevTypeSymbol != typeSymbol)
                    return false;

                prevTypeSymbol = typeSymbol;
            }

            return true;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LocalDeclarationStatementSyntax[] localDeclarations = selectedStatements
                .Cast<LocalDeclarationStatementSyntax>()
                .ToArray();

            LocalDeclarationStatementSyntax localDeclaration = localDeclarations[0];

            SyntaxList<StatementSyntax> statements = selectedStatements.UnderlyingList;

            int index = statements.IndexOf(localDeclaration);

            VariableDeclaratorSyntax[] variables = localDeclarations
                .Skip(1)
                .Select(f => f.Declaration)
                .SelectMany(f => f.Variables)
                .ToArray();

            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration
                .AddDeclarationVariables(variables)
                .WithTrailingTrivia(localDeclarations[localDeclarations.Length - 1].GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> newStatements = statements.Replace(
                localDeclaration,
                newLocalDeclaration);

            for (int i = 1; i < localDeclarations.Length; i++)
                newStatements = newStatements.RemoveAt(index + 1);

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
        }
    }
}
