// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CheckParameterForNullRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (parameter.Identifier.Span.Contains(context.Span)
                && IsValid(parameter))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (CanRefactor(parameter, semanticModel, context.CancellationToken))
                    RegisterRefactoring(context, parameter);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterListSyntax parameterList)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ParameterSyntax[] parameters = GetSelectedParameters(parameterList, context.Span)
                .Where(parameter => IsValid(parameter) && CanRefactor(parameter, semanticModel, context.CancellationToken))
                .ToArray();

            if (parameters.Length == 1)
            {
                RegisterRefactoring(context, parameters[0]);
            }
            else if (parameters.Length > 0)
            {
                RegisterRefactoring(context, parameters.ToImmutableArray(), "parameters");
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ParameterSyntax parameter)
        {
            RegisterRefactoring(context, ImmutableArray.Create(parameter), $"'{parameter.Identifier.ValueText}'");
        }

        private static void RegisterRefactoring(RefactoringContext context, ImmutableArray<ParameterSyntax> parameters, string name)
        {
                context.RegisterRefactoring(
                $"Check {name} for null",
                cancellationToken => RefactorAsync(
                    context.Document,
                    parameters,
                    cancellationToken));
        }

        public static bool CanRefactor(
            ParameterSyntax parameter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            BlockSyntax body = GetBody(parameter);

            if (body != null)
            {
                IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, cancellationToken);

                return parameterSymbol?.Type?.IsReferenceType == true
                    && !ContainsNullCheck(body, parameter, semanticModel, cancellationToken);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ImmutableArray<ParameterSyntax> parameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            BlockSyntax body = GetBody(parameters[0]);

            int index = body.Statements
                .TakeWhile(f => IsNullCheck(f, semanticModel, cancellationToken))
                .Count();

            List<IfStatementSyntax> ifStatements = CreateNullChecks(parameters);

            if (index > 0)
                ifStatements[0] = ifStatements[0].WithLeadingTrivia(NewLineTrivia());

            BlockSyntax newBody = body
                .WithStatements(body.Statements.InsertRange(index, ifStatements))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(body, newBody);

            return document.WithSyntaxRoot(newRoot);
        }

        private static List<IfStatementSyntax> CreateNullChecks(ImmutableArray<ParameterSyntax> parameters)
        {
            var ifStatements = new List<IfStatementSyntax>();

            bool isFirst = true;

            foreach (ParameterSyntax parameter in parameters)
            {
                IfStatementSyntax ifStatement = CreateNullCheck(parameter.Identifier.ValueText);

                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    ifStatement = ifStatement.WithLeadingTrivia(NewLineTrivia());
                }

                ifStatements.Add(ifStatement);
            }

            ifStatements[ifStatements.Count - 1] = ifStatements[ifStatements.Count - 1].WithTrailingTrivia(NewLineTrivia(), NewLineTrivia());

            return ifStatements;
        }

        private static IfStatementSyntax CreateNullCheck(string identifier)
        {
            return IfStatement(
                EqualsExpression(
                    IdentifierName(identifier),
                    NullLiteralExpression()),
                ThrowStatement(
                    ObjectCreationExpression(
                        type: ParseName("System.ArgumentNullException").WithSimplifierAnnotation(),
                        argumentList: ArgumentList(Argument(NameOf(identifier))),
                        initializer: null)));
        }

        private static bool ContainsNullCheck(
            BlockSyntax body,
            ParameterSyntax parameter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<StatementSyntax> statements = body.Statements;

            for (int i = 0; i < statements.Count; i++)
            {
                if (IsNullCheck(statements[i], semanticModel, cancellationToken))
                {
                    if (IsNullCheck(statements[i], parameter.Identifier.ToString()))
                        return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private static bool IsNullCheck(StatementSyntax statement, string identifier)
        {
            var ifStatement = (IfStatementSyntax)statement;
            var binaryExpression = (BinaryExpressionSyntax)ifStatement.Condition;
            var identifierName = (IdentifierNameSyntax)binaryExpression.Left;

            return string.Equals(identifier, identifierName.Identifier.ToString(), StringComparison.Ordinal);
        }

        private static bool IsNullCheck(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (statement.IsKind(SyntaxKind.IfStatement))
            {
                var ifStatement = (IfStatementSyntax)statement;

                var binaryExpression = ifStatement.Condition as BinaryExpressionSyntax;

                if (binaryExpression?.Right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                {
                    ExpressionSyntax left = binaryExpression.Left;

                    if (left.IsKind(SyntaxKind.IdentifierName))
                    {
                        var throwStatement = GetSingleStatementOrDefault(ifStatement) as ThrowStatementSyntax;

                        if (throwStatement?.Expression?.IsKind(SyntaxKind.ObjectCreationExpression) == true)
                        {
                            var objectCreation = (ObjectCreationExpressionSyntax)throwStatement.Expression;

                            INamedTypeSymbol exceptionType = semanticModel.Compilation.GetTypeByMetadataName("System.ArgumentNullException");

                            ISymbol type = semanticModel.GetSymbolInfo(objectCreation.Type, cancellationToken).Symbol;

                            return type?.Equals(exceptionType) == true;
                        }
                    }
                }
            }

            return false;
        }

        private static StatementSyntax GetSingleStatementOrDefault(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1)
                    return statements[0];

                return null;
            }

            return statement;
        }

        private static BlockSyntax GetBody(ParameterSyntax parameter)
        {
            SyntaxNode parent = parameter.Parent;

            if (parent?.IsKind(SyntaxKind.ParameterList) == true)
            {
                parent = parent.Parent;

                switch (parent?.Kind())
                {
                    case SyntaxKind.MethodDeclaration:
                        return ((MethodDeclarationSyntax)parent).Body;
                    case SyntaxKind.ConstructorDeclaration:
                        return ((ConstructorDeclarationSyntax)parent).Body;
                    default:
                        {
                            Debug.Assert(false, parent?.Kind().ToString());
                            break;
                        }
                }
            }

            return null;
        }

        private static bool IsValid(ParameterSyntax parameter)
        {
            return parameter.Type != null
                && !parameter.Identifier.IsMissing
                && parameter.Parent?.IsKind(SyntaxKind.ParameterList) == true;
        }

        private static IEnumerable<ParameterSyntax> GetSelectedParameters(ParameterListSyntax parameterList, TextSpan span)
        {
            return parameterList.Parameters
                .SkipWhile(f => span.Start > f.Span.Start)
                .TakeWhile(f => span.End >= f.Span.End);
        }
    }
}
