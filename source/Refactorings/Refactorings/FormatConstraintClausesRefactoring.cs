// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatConstraintClausesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, TypeParameterConstraintClauseSyntax constraintClause)
        {
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = constraintClause.GetContainingList();

            if (constraintClauses.IsSingleLine())
            {
                if (constraintClauses.Count > 1)
                {
                    context.RegisterRefactoring(
                        "Format constraints on separate lines",
                        cancellationToken =>
                        {
                            SyntaxNode parent = constraintClause.Parent;

                            SyntaxNode newNode = ToMultiLine(parent, constraintClauses);

                            return context.Document.ReplaceNodeAsync(parent, newNode, cancellationToken);
                        });
                }
            }
            else
            {
                context.RegisterRefactoring(
                    "Format constraints on a single line",
                    cancellationToken =>
                    {
                        SyntaxNode parent = constraintClause.Parent;

                        SyntaxNode newNode = ToSingleLine(parent, constraintClauses);

                        return context.Document.ReplaceNodeAsync(parent, newNode, cancellationToken);
                    });
            }
        }

        private static SyntaxNode ToSingleLine(SyntaxNode node, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            SyntaxToken previousToken = node.FindToken(constraintClauses.First().FullSpan.Start - 1);

            node = node.ReplaceToken(previousToken, previousToken.WithTrailingTrivia(TriviaList(ElasticSpace)));

            int count = constraintClauses.Count;

            for (int i = 0; i < count; i++)
            {
                TypeParameterConstraintClauseSyntax constraintClause = constraintClauses[i];

                TextSpan? span = null;

                if (i == count - 1)
                    span = TextSpan.FromBounds(constraintClause.FullSpan.Start, constraintClause.Span.End);

                TypeParameterConstraintClauseSyntax newNode = constraintClause
                    .RemoveWhitespaceOrEndOfLineTrivia(span)
                    .WithFormatterAnnotation();

                constraintClauses = constraintClauses.ReplaceAt(i, newNode);
            }

            return WithConstraintClauses(node, constraintClauses);
        }

        private static SyntaxNode ToMultiLine(SyntaxNode node, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            TypeParameterConstraintClauseSyntax first = constraintClauses.First();

            SyntaxToken previousToken = node.FindToken(first.FullSpan.Start - 1);

            node = node.ReplaceToken(previousToken, previousToken.WithTrailingTrivia(TriviaList(NewLine())));

            SyntaxTriviaList leadingTrivia = node
                .FindToken(node.SpanStart)
                .LeadingTrivia;

            SyntaxTriviaList trivia = IncreaseIndentation(leadingTrivia.LastOrDefault());

            int count = constraintClauses.Count;

            for (int i = 0; i < count; i++)
            {
                TypeParameterConstraintClauseSyntax newNode = constraintClauses[i].WithLeadingTrivia(trivia);

                if (i < count - 1)
                    newNode = newNode.WithTrailingTrivia(NewLine());

                constraintClauses = constraintClauses.ReplaceAt(i, newNode);
            }

            return WithConstraintClauses(node, constraintClauses);
        }

        private static SyntaxNode WithConstraintClauses(SyntaxNode node, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithConstraintClauses(constraintClauses);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithConstraintClauses(constraintClauses);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithConstraintClauses(constraintClauses);
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).WithConstraintClauses(constraintClauses);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithConstraintClauses(constraintClauses);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithConstraintClauses(constraintClauses);
            }

            Debug.Fail(node.Kind().ToString());

            return node;
        }
    }
}