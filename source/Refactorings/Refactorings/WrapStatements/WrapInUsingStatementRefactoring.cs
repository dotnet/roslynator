// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.WrapStatements
{
    internal class WrapInUsingStatementRefactoring : WrapStatementsRefactoring<UsingStatementSyntax>
    {
        public async Task ComputeRefactoringAsync(RefactoringContext context, BlockSpan blockSpan)
        {
            StatementSyntax statement = blockSpan.FirstSelectedStatement;

            if (statement?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                if (localDeclaration.Declaration?.Variables.Count == 1
                    && localDeclaration.Declaration.Variables[0].Initializer?.Value?.IsKind(SyntaxKind.ObjectCreationExpression) == true
                    && localDeclaration.Declaration.Type != null)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ITypeSymbol type = semanticModel
                        .GetTypeInfo(localDeclaration.Declaration.Type)
                        .Type;

                    if (type?.IsNamedType() == true
                        && ((INamedTypeSymbol)type).Implements(SpecialType.System_IDisposable))
                    {
                        context.RegisterRefactoring(
                            "Wrap in 'using' statement",
                            cancellationToken => RefactorAsync(context.Document, (BlockSyntax)localDeclaration.Parent, context.Span, cancellationToken));
                    }
                }
            }
        }

        public override UsingStatementSyntax CreateStatement(ImmutableArray<StatementSyntax> statements)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)statements.First();

            UsingStatementSyntax usingStatement = UsingStatement(
                localDeclaration.Declaration.WithoutTrivia(),
                null,
                CreateBlock(statements, localDeclaration));

            return usingStatement
                .WithLeadingTrivia(localDeclaration.GetLeadingTrivia())
                .WithTrailingTrivia(statements.Last().GetTrailingTrivia())
                .WithFormatterAnnotation();
        }

        private static BlockSyntax CreateBlock(ImmutableArray<StatementSyntax> statements, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (statements.Length > 1)
            {
                var nodes = new List<StatementSyntax>(statements.Skip(1));

                nodes[0] = nodes[0].WithLeadingTrivia(
                    localDeclaration
                        .GetTrailingTrivia()
                        .AddRange(nodes[0].GetLeadingTrivia()));

                nodes[nodes.Count - 1] = nodes[nodes.Count - 1].WithTrailingTrivia();

                return Block(nodes);
            }

            return Block();
        }
    }
}
