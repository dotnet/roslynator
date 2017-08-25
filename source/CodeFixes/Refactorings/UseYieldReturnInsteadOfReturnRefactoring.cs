// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseYieldReturnInsteadOfReturnRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ReturnStatementSyntax returnStatement,
            SyntaxKind replacementKind,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SyntaxToken returnKeyword = returnStatement.ReturnKeyword;
            ExpressionSyntax expression = returnStatement.Expression;

            switch (replacementKind)
            {
                case SyntaxKind.YieldReturnStatement:
                    {
                        YieldStatementSyntax yieldReturnStatement = YieldStatement(
                            SyntaxKind.YieldReturnStatement,
                            Token(returnKeyword.LeadingTrivia, SyntaxKind.YieldKeyword, TriviaList(Space)),
                            returnKeyword.WithoutLeadingTrivia(),
                            expression,
                            returnStatement.SemicolonToken);

                        return document.ReplaceNodeAsync(returnStatement, yieldReturnStatement, cancellationToken);
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        string name = NameGenerator.Default.EnsureUniqueLocalName(
                            DefaultNames.ForEachVariable,
                            semanticModel,
                            returnStatement.SpanStart,
                            cancellationToken: cancellationToken);

                        YieldStatementSyntax yieldReturnStatement = YieldStatement(
                            SyntaxKind.YieldReturnStatement,
                            Token(default(SyntaxTriviaList), SyntaxKind.YieldKeyword, TriviaList(Space)),
                            returnKeyword.WithoutLeadingTrivia(),
                            IdentifierName(name),
                            returnStatement.SemicolonToken.WithoutTrailingTrivia());

                        StatementSyntax newNode = ForEachStatement(
                            VarType(),
                            name,
                            expression,
                            Block(yieldReturnStatement));

                        if (returnStatement.IsEmbedded())
                            newNode = Block(newNode);

                        newNode = newNode.WithTriviaFrom(returnStatement);

                        return document.ReplaceNodeAsync(returnStatement, newNode, cancellationToken);
                    }
                default:
                    {
                        Debug.Fail("");
                        return Task.FromResult(document);
                    }
            }
        }
    }
}
