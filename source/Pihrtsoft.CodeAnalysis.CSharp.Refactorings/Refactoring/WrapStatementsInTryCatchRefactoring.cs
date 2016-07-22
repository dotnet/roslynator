// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class WrapStatementsInTryCatchRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, BlockSyntax block)
        {
            return GetSelectedStatements(block, context.Span).Any();
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax[] statements = GetSelectedStatements(block, span).ToArray();

            int index = block.Statements.IndexOf(statements[0]);

            SyntaxList<StatementSyntax> newStatements = block.Statements;

            int cnt = statements.Length;

            while (cnt > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                cnt--;
            }

            TryStatementSyntax tryStatement = TryStatement(
                Block(statements),
                CatchClause(
                    CatchDeclaration(
                        ParseName("System.Exception").WithSimplifierAnnotation(),
                        Identifier("ex")),
                    null,
                    Block()));

            tryStatement = tryStatement.WithFormatterAnnotation();

            newStatements = newStatements.Insert(index, tryStatement);

            root = root.ReplaceNode(block, block.WithStatements(newStatements));

            return document.WithSyntaxRoot(root);
        }

        private static IEnumerable<StatementSyntax> GetSelectedStatements(BlockSyntax block, TextSpan span)
        {
            return block.Statements
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }
    }
}
