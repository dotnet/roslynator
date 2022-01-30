// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using System.Diagnostics.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CheckParameterForNullRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ParameterSyntax parameter, SemanticModel semanticModel)
        {
            if (!IsValid(parameter, semanticModel, context.CancellationToken))
                return;

            if (!CanRefactor(parameter, semanticModel, context.CancellationToken))
                return;

            RegisterRefactoring(context, parameter, semanticModel);
        }

        [SuppressMessage("Simplification", "RCS1180:Inline lazy initialization.")]
        public static void ComputeRefactoring(RefactoringContext context, SeparatedSyntaxListSelection<ParameterSyntax> selectedParameters, SemanticModel semanticModel)
        {
            ParameterSyntax singleParameter = null;
            ImmutableArray<ParameterSyntax>.Builder builder = default;

            foreach (ParameterSyntax parameter in selectedParameters)
            {
                if (IsValid(parameter, semanticModel, context.CancellationToken)
                    && CanRefactor(parameter, semanticModel, context.CancellationToken))
                {
                    if (singleParameter == null)
                    {
                        singleParameter = parameter;
                    }
                    else
                    {
                        if (builder == null)
                            builder = ImmutableArray.CreateBuilder<ParameterSyntax>(selectedParameters.Count);

                        builder.Add(singleParameter);
                        builder.Add(parameter);
                    }
                }
            }

            if (builder != null)
            {
                RegisterRefactoring(context, builder.ToImmutableArray(), "parameters", semanticModel);
            }
            else if (singleParameter != null)
            {
                RegisterRefactoring(context, singleParameter, semanticModel);
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ParameterSyntax parameter, SemanticModel semanticModel)
        {
            RegisterRefactoring(context, ImmutableArray.Create(parameter), $"'{parameter.Identifier.ValueText}'", semanticModel);
        }

        private static void RegisterRefactoring(
            RefactoringContext context,
            ImmutableArray<ParameterSyntax> parameters,
            string name,
            SemanticModel semanticModel)
        {
            context.RegisterRefactoring(
                $"Check {name} for null",
                ct => RefactorAsync(
                    context.Document,
                    parameters,
                    semanticModel,
                    ct),
                RefactoringDescriptors.CheckParameterForNull);
        }

        public static bool CanRefactor(
            ParameterSyntax parameter,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            BlockSyntax body = GetBody(parameter);

            if (body == null)
                return false;

            IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, cancellationToken);

            if (parameterSymbol?.Type.IsReferenceTypeOrNullableType() != true)
                return false;

            foreach (StatementSyntax statement in body.Statements)
            {
                NullCheckExpressionInfo nullCheck = GetNullCheckExpressionInfo(statement, semanticModel, cancellationToken);

                if (nullCheck.Success)
                {
                    if (string.Equals(((IdentifierNameSyntax)nullCheck.Expression).Identifier.ValueText, parameter.Identifier.ValueText, StringComparison.Ordinal))
                        return false;
                }
                else
                {
                    break;
                }
            }

            return true;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ImmutableArray<ParameterSyntax> parameters,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            BlockSyntax body = GetBody(parameters[0]);

            SyntaxList<StatementSyntax> statements = body.Statements;

            int count = statements
                .TakeWhile(f => GetNullCheckExpressionInfo(f, semanticModel, cancellationToken).Success)
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

            return document.ReplaceNodeAsync(body, newBody, cancellationToken);
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
                            type: ParseName("System.ArgumentNullException").WithSimplifierAnnotation(),
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

        private static NullCheckExpressionInfo GetNullCheckExpressionInfo(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default)
        {
            if (statement is not IfStatementSyntax ifStatement)
                return default;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, NullCheckStyles.EqualsToNull | NullCheckStyles.IsNull);

            if (!nullCheck.Success)
                return default;

            if (nullCheck.Expression.Kind() != SyntaxKind.IdentifierName)
                return default;

            var throwStatement = ifStatement.SingleNonBlockStatementOrDefault() as ThrowStatementSyntax;

            if (throwStatement?.Expression?.Kind() != SyntaxKind.ObjectCreationExpression)
                return default;

            var objectCreation = (ObjectCreationExpressionSyntax)throwStatement.Expression;

            ISymbol type = semanticModel.GetSymbol(objectCreation.Type, cancellationToken);

            if (!string.Equals(type?.Name, "ArgumentNullException", StringComparison.Ordinal))
                return default;

            if (!type.HasMetadataName(MetadataNames.System_ArgumentNullException))
                return default;

            return nullCheck;
        }

        private static BlockSyntax GetBody(ParameterSyntax parameter)
        {
            SyntaxNode parent = parameter.Parent;

            if (!parent.IsKind(SyntaxKind.ParameterList))
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
#if DEBUG
            switch (parent.Kind())
            {
                case SyntaxKind.ParenthesizedLambdaExpression:
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.LocalFunctionStatement:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    {
                        break;
                    }
                default:
                    {
                        SyntaxDebug.Fail(parent);
                        break;
                    }
            }
#endif
            return null;
        }

        private static bool IsValid(ParameterSyntax parameter, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            return parameter.Type != null
                && !parameter.Identifier.IsMissing
                && parameter.IsParentKind(SyntaxKind.ParameterList)
                && parameter.Default?.Value?.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression, SyntaxKind.DefaultExpression) != true
                && !CSharpUtility.IsNullableReferenceType(parameter.Type, semanticModel, cancellationToken);
        }
    }
}
