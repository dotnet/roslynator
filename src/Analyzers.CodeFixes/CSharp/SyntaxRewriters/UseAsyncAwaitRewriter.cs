// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Roslynator;
using Roslynator.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.SyntaxRewriters;

internal class UseAsyncAwaitRewriter : SkipFunctionRewriter
{
    private static readonly SyntaxAnnotation[] _asyncAwaitAnnotation = new[] { new SyntaxAnnotation() };

    private static readonly SyntaxAnnotation[] _asyncAwaitAnnotationAndFormatterAnnotation = new SyntaxAnnotation[] { _asyncAwaitAnnotation[0], Formatter.Annotation };

    public UseAsyncAwaitRewriter(bool keepReturnStatement)
    {
        KeepReturnStatement = keepReturnStatement;
    }

    public bool KeepReturnStatement { get; }

    public static UseAsyncAwaitRewriter Create(IMethodSymbol methodSymbol)
    {
        ITypeSymbol returnType = methodSymbol.ReturnType.OriginalDefinition;

        var keepReturnStatement = false;

        if (returnType.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_ValueTask_T)
            || returnType.EqualsOrInheritsFrom(MetadataNames.System_Threading_Tasks_Task_T))
        {
            keepReturnStatement = true;
        }

        return new UseAsyncAwaitRewriter(keepReturnStatement: keepReturnStatement);
    }

    public override SyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
    {
        ExpressionSyntax expression = node.Expression;

        if (expression?.IsKind(SyntaxKind.AwaitExpression) == false)
        {
            if (KeepReturnStatement)
            {
                return node.WithExpression(AwaitExpression(expression.WithoutTrivia().Parenthesize()).WithTriviaFrom(expression));
            }
            else
            {
                return ExpressionStatement(AwaitExpression(expression.WithoutTrivia().Parenthesize()).WithTriviaFrom(expression))
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithAdditionalAnnotations(_asyncAwaitAnnotationAndFormatterAnnotation);
            }
        }

        return base.VisitReturnStatement(node);
    }

    public override SyntaxNode VisitBlock(BlockSyntax node)
    {
        node = (BlockSyntax)base.VisitBlock(node);

        SyntaxList<StatementSyntax> statements = node.Statements;

        statements = RewriteStatements(statements, skipLastStatement: true);

        return node.WithStatements(statements);
    }

    public override SyntaxNode VisitSwitchSection(SwitchSectionSyntax node)
    {
        node = (SwitchSectionSyntax)base.VisitSwitchSection(node);

        SyntaxList<StatementSyntax> statements = node.Statements;

        statements = RewriteStatements(statements, skipLastStatement: false);

        return node.WithStatements(statements);
    }

    private static SyntaxList<StatementSyntax> RewriteStatements(SyntaxList<StatementSyntax> statements, bool skipLastStatement)
    {
        int startIndex = statements.Count - 1;

        if (skipLastStatement)
            startIndex--;

        for (int i = startIndex; i >= 0; i--)
        {
            StatementSyntax statement = statements[i];

            if (statement.HasAnnotation(_asyncAwaitAnnotation[0]))
            {
                statements = statements.Replace(
                    statement,
                    statement.WithoutAnnotations(_asyncAwaitAnnotation).WithTrailingTrivia(NewLine()));

                statements = statements.Insert(
                    i + 1,
                    ReturnStatement().WithTrailingTrivia(statement.GetTrailingTrivia()).WithFormatterAnnotation());
            }
        }

        return statements;
    }
}
