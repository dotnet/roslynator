// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings;

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
                if (singleParameter is null)
                {
                    singleParameter = parameter;
                }
                else
                {
                    if (builder is null)
                        builder = ImmutableArray.CreateBuilder<ParameterSyntax>(selectedParameters.Count);

                    builder.Add(singleParameter);
                    builder.Add(parameter);
                }
            }
        }

        if (builder is not null)
        {
            RegisterRefactoring(context, builder.ToImmutableArray(), "parameters", semanticModel);
        }
        else if (singleParameter is not null)
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

        if (body is null)
            return false;

        IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, cancellationToken);

        if (parameterSymbol?.Type.IsReferenceTypeOrNullableType() != true)
            return false;

        foreach (StatementSyntax statement in body.Statements)
        {
            ArgumentNullCheckAnalysis nullCheck = ArgumentNullCheckAnalysis.Create(statement, semanticModel, parameter.Identifier.ValueText, cancellationToken);

            if (nullCheck.Style != ArgumentNullCheckStyle.None)
            {
                if (nullCheck.Success)
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
            .TakeWhile(f => ArgumentNullCheckAnalysis.IsArgumentNullCheck(f, semanticModel, cancellationToken))
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

    private static BlockSyntax GetBody(ParameterSyntax parameter)
    {
        SyntaxNode parent = parameter.Parent;

        if (!parent.IsKind(SyntaxKind.ParameterList))
            return null;

        parent = parent.Parent;

        Debug.Assert(parent is not null);

        if (parent is null)
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
        return parameter.Type is not null
            && !parameter.Identifier.IsMissing
            && parameter.IsParentKind(SyntaxKind.ParameterList)
            && parameter.Default?.Value?.IsKind(SyntaxKind.NullLiteralExpression, SyntaxKind.DefaultLiteralExpression, SyntaxKind.DefaultExpression) != true
            && !CSharpUtility.IsNullableReferenceType(parameter.Type, semanticModel, cancellationToken);
    }
}
