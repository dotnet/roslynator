// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WrapConstraintClausesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, TypeParameterConstraintClauseSyntax constraintClause)
        {
            GenericInfo genericInfo = SyntaxInfo.GenericInfo(constraintClause);

            if (!genericInfo.Success)
                return;

            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = genericInfo.ConstraintClauses;

            if (constraintClauses.IsSingleLine())
            {
                if (constraintClauses.Count > 1)
                {
                    context.RegisterRefactoring(
                        "Wrap constraints",
                        ct =>
                        {
                            GenericInfo newInfo = WrapConstraints(genericInfo);

                            return context.Document.ReplaceNodeAsync(genericInfo.Node, newInfo.Node, ct);
                        },
                        RefactoringIdentifiers.WrapConstraintClauses);
                }
            }
            else if (constraintClause.DescendantTrivia(constraintClause.Span).All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && constraintClauses[0].GetFirstToken().GetPreviousToken().TrailingTrivia.IsEmptyOrWhitespace())
            {
                context.RegisterRefactoring(
                    "Unwrap constraints",
                    ct =>
                    {
                        GenericInfo newInfo = UnwrapConstraints(genericInfo);

                        return context.Document.ReplaceNodeAsync(genericInfo.Node, newInfo.Node, ct);
                    },
                    RefactoringIdentifiers.WrapConstraintClauses);
            }
        }

        private static GenericInfo UnwrapConstraints(in GenericInfo info)
        {
            SyntaxNode declaration = info.Node;

            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = info.ConstraintClauses;

            SyntaxToken previousToken = declaration.FindToken(constraintClauses[0].FullSpan.Start - 1);

            declaration = declaration.ReplaceToken(previousToken, previousToken.WithTrailingTrivia(TriviaList(ElasticSpace)));

            int count = constraintClauses.Count;

            for (int i = 0; i < count; i++)
            {
                TypeParameterConstraintClauseSyntax constraintClause = constraintClauses[i];

                TextSpan? span = null;

                if (i == count - 1)
                    span = TextSpan.FromBounds(constraintClause.FullSpan.Start, constraintClause.Span.End);

                TypeParameterConstraintClauseSyntax newNode = constraintClause
                    .RemoveWhitespace(span)
                    .WithFormatterAnnotation();

                constraintClauses = constraintClauses.ReplaceAt(i, newNode);
            }

            return SyntaxInfo.GenericInfo(declaration).WithConstraintClauses(constraintClauses);
        }

        private static GenericInfo WrapConstraints(in GenericInfo info)
        {
            SyntaxNode declaration = info.Node;
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = info.ConstraintClauses;

            TypeParameterConstraintClauseSyntax first = constraintClauses[0];

            SyntaxToken previousToken = declaration.FindToken(first.FullSpan.Start - 1);

            declaration = declaration.ReplaceToken(previousToken, previousToken.WithTrailingTrivia(TriviaList(NewLine())));

            SyntaxTrivia trivia = SyntaxTriviaAnalysis.GetIncreasedIndentationTrivia(declaration);

            int count = constraintClauses.Count;

            for (int i = 0; i < count; i++)
            {
                TypeParameterConstraintClauseSyntax newNode = constraintClauses[i].WithLeadingTrivia(trivia);

                if (i < count - 1)
                    newNode = newNode.WithTrailingTrivia(NewLine());

                constraintClauses = constraintClauses.ReplaceAt(i, newNode);
            }

            return SyntaxInfo.GenericInfo(declaration).WithConstraintClauses(constraintClauses);
        }
    }
}