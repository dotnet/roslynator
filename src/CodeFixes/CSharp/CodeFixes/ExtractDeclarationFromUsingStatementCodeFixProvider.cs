// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ExtractDeclarationFromUsingStatementCodeFixProvider))]
    [Shared]
    public sealed class ExtractDeclarationFromUsingStatementCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(CompilerDiagnosticIdentifiers.CS1674_TypeUsedInUsingStatementMustBeImplicitlyConvertibleToIDisposable); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics[0];

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ExtractDeclarationFromUsingStatement, context.Document, root.SyntaxTree))
                return;

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out UsingStatementSyntax usingStatement))
                return;

            if (usingStatement.ContainsDiagnostics)
                return;

            CodeAction codeAction = CodeAction.Create(
                "Extract declaration from using statement",
                ct => RefactorAsync(context.Document, usingStatement, ct),
                GetEquivalenceKey(diagnostic));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken = default)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(usingStatement);

            int index = statementsInfo.Statements.IndexOf(usingStatement);

            StatementListInfo newStatementsInfo = statementsInfo.RemoveAt(index);

            var statements = new List<StatementSyntax>() { SyntaxFactory.LocalDeclarationStatement(usingStatement.Declaration) };

            statements.AddRange(GetStatements(usingStatement));

            if (statements.Count > 0)
            {
                statements[0] = statements[0]
                    .WithLeadingTrivia(usingStatement.GetLeadingTrivia());

                statements[statements.Count - 1] = statements[statements.Count - 1]
                    .WithTrailingTrivia(usingStatement.GetTrailingTrivia());
            }

            newStatementsInfo = newStatementsInfo.WithStatements(newStatementsInfo.Statements.InsertRange(index, statements));

            return document.ReplaceNodeAsync(statementsInfo.Parent, newStatementsInfo.Parent.WithFormatterAnnotation(), cancellationToken);
        }

        private static IEnumerable<StatementSyntax> GetStatements(UsingStatementSyntax usingStatement)
        {
            StatementSyntax statement = usingStatement.Statement;

            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    foreach (StatementSyntax statement2 in ((BlockSyntax)statement).Statements)
                        yield return statement2;
                }
                else
                {
                    yield return statement;
                }
            }
        }
    }
}
