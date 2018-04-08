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
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CheckParameterForNullRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterSyntax parameter)
        {
            if (!parameter.Identifier.Span.Contains(context.Span))
                return;

            if (!IsValid(parameter))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!CanRefactor(parameter, semanticModel, context.CancellationToken))
                return;

            RegisterRefactoring(context, parameter);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ParameterListSyntax parameterList)
        {
            if (!SeparatedSyntaxListSelection<ParameterSyntax>.TryCreate(parameterList.Parameters, context.Span, out SeparatedSyntaxListSelection<ParameterSyntax> selection))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ImmutableArray<ParameterSyntax> parameters = selection
                .Where(parameter => IsValid(parameter) && CanRefactor(parameter, semanticModel, context.CancellationToken))
                .ToImmutableArray();

            if (parameters.Length == 1)
            {
                RegisterRefactoring(context, parameters[0]);
            }
            else if (parameters.Length > 0)
            {
                RegisterRefactoring(context, parameters, "parameters");
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
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            BlockSyntax body = GetBody(parameters[0]);

            SyntaxList<StatementSyntax> statements = body.Statements;

            int count = statements
                .TakeWhile(f => IsNullCheck(f, semanticModel, cancellationToken))
                .Count();

            List<IfStatementSyntax> ifStatements = CreateNullChecks(parameters);

            if (statements.Any())
            {
                if (count > 0)
                    ifStatements[0] = ifStatements[0].WithLeadingTrivia(NewLine());

                if (count != statements.Count)
                {
                    int start = (count > 0)
                        ? statements[count - 1].Span.End
                        : body.OpenBraceToken.Span.End;

                    int end = (count > 0)
                        ? statements[count].SpanStart
                        : statements[0].SpanStart;

                    int lineCount = body.SyntaxTree.GetLineCount(TextSpan.FromBounds(start, end), cancellationToken);

                    if (lineCount <= 2)
                    {
                        ifStatements[ifStatements.Count - 1] = ifStatements[ifStatements.Count - 1].WithTrailingTrivia(TriviaList(NewLine(), NewLine()));
                    }
                    else if (lineCount == 3)
                    {
                        ifStatements[ifStatements.Count - 1] = ifStatements[ifStatements.Count - 1].WithTrailingTrivia(NewLine());
                    }
                }
            }

            BlockSyntax newBody = body
                .WithStatements(statements.InsertRange(count, ifStatements))
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(body, newBody, cancellationToken).ConfigureAwait(false);
        }

        private static List<IfStatementSyntax> CreateNullChecks(ImmutableArray<ParameterSyntax> parameters)
        {
            var ifStatements = new List<IfStatementSyntax>();

            for (int i = 0; i < parameters.Length; i++)
            {
                IfStatementSyntax ifStatement = IfStatement(
                    EqualsExpression(
                        IdentifierName(parameters[i].Identifier.ValueText),
                        NullLiteralExpression()),
                    ThrowStatement(
                        ObjectCreationExpression(
                            type: ParseName(MetadataNames.System_ArgumentNullException).WithSimplifierAnnotation(),
                            argumentList: ArgumentList(Argument(NameOfExpression(parameters[i].Identifier.ValueText))),
                            initializer: default(InitializerExpressionSyntax))));

                if (i > 0)
                {
                    ifStatements[i - 1] = ifStatements[i - 1].WithTrailingTrivia(NewLine());
                    ifStatement = ifStatement.WithLeadingTrivia(NewLine());
                }

                ifStatements.Add(ifStatement);
            }

            return ifStatements;
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
            if (!(statement is IfStatementSyntax ifStatement))
                return false;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, NullCheckStyles.EqualsToNull | NullCheckStyles.IsNull);

            if (!nullCheck.Success)
                return false;

            if (nullCheck.Expression.Kind() != SyntaxKind.IdentifierName)
                return false;

            var throwStatement = ifStatement.SingleNonBlockStatementOrDefault() as ThrowStatementSyntax;

            if (throwStatement?.Expression?.Kind() != SyntaxKind.ObjectCreationExpression)
                return false;

            var objectCreation = (ObjectCreationExpressionSyntax)throwStatement.Expression;

            INamedTypeSymbol exceptionType = semanticModel.GetTypeByMetadataName(MetadataNames.System_ArgumentNullException);

            ISymbol type = semanticModel.GetSymbol(objectCreation.Type, cancellationToken);

            return type?.Equals(exceptionType) == true;
        }

        private static BlockSyntax GetBody(ParameterSyntax parameter)
        {
            SyntaxNode parent = parameter.Parent;

            if (parent?.Kind() != SyntaxKind.ParameterList)
                return null;

            parent = parent.Parent;

            Debug.Assert(parent != null);

            if (parent == null)
                return null;

            switch (parent.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)parent).Body;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)parent).Body;
            }

            Debug.Assert(parent.IsKind(
                SyntaxKind.ParenthesizedLambdaExpression,
                SyntaxKind.AnonymousMethodExpression,
                SyntaxKind.LocalFunctionStatement,
                SyntaxKind.DelegateDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.ConversionOperatorDeclaration), parent.Kind().ToString());

            return null;
        }

        private static bool IsValid(ParameterSyntax parameter)
        {
            return parameter.Type != null
                && !parameter.Identifier.IsMissing
                && parameter.IsParentKind(SyntaxKind.ParameterList)
                && parameter.Default?.Value?.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultExpression) != true;
        }
    }
}
