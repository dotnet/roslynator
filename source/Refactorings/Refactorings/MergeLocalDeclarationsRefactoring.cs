// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeLocalDeclarationsRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, StatementsSelection selectedStatements)
        {
            if (selectedStatements.Count > 1)
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (AreLocalDeclarations(selectedStatements, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Merge local declarations",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                selectedStatements.Info,
                                selectedStatements.Cast<LocalDeclarationStatementSyntax>().ToArray(),
                                cancellationToken);
                        });
                }
            }
        }

        private static bool AreLocalDeclarations(
            IEnumerable<StatementSyntax> statements,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ITypeSymbol prevTypeSymbol = null;

            using (IEnumerator<StatementSyntax> en = statements.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    if (!en.Current.IsKind(SyntaxKind.LocalDeclarationStatement))
                        return false;

                    var localDeclaration = (LocalDeclarationStatementSyntax)en.Current;

                    TypeSyntax type = localDeclaration.Declaration?.Type;

                    if (type == null)
                        return false;

                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

                    if (typeSymbol == null || typeSymbol.IsErrorType())
                        return false;

                    if (prevTypeSymbol != null && prevTypeSymbol != typeSymbol)
                        return false;

                    prevTypeSymbol = typeSymbol;
                }
            }

            return true;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementsInfo statementsInfo,
            LocalDeclarationStatementSyntax[] localDeclarations,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LocalDeclarationStatementSyntax localDeclaration = localDeclarations[0];

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

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

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }
    }
}
