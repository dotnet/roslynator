// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.WrapStatements
{
    internal class WrapInUsingStatementRefactoring : WrapStatementsRefactoring<UsingStatementSyntax>
    {
        public async Task ComputeRefactoringAsync(RefactoringContext context, StatementsSelection selectedStatements)
        {
            StatementSyntax statement = selectedStatements.FirstOrDefault();

            if (statement?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)statement;
                VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                if (declaration != null)
                {
                    VariableDeclaratorSyntax variable = declaration.Variables.SingleOrDefault(throwException: false);

                    if (variable?.Initializer?.Value?.IsKind(SyntaxKind.ObjectCreationExpression) == true
                        && declaration.Type != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(declaration.Type, context.CancellationToken);

                        if (typeSymbol?.IsNamedType() == true
                            && ((INamedTypeSymbol)typeSymbol).Implements(SpecialType.System_IDisposable))
                        {
                            context.RegisterRefactoring(
                                $"Using '{variable.Identifier.ValueText}'",
                                cancellationToken => RefactorAsync(context.Document, selectedStatements, cancellationToken));
                        }
                    }
                }
            }
        }

        public override UsingStatementSyntax CreateStatement(ImmutableArray<StatementSyntax> statements)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)statements[0];

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
