// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InvertBodyAndExpressionBodyRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
        {
            if (selectedMembers.Count <= 1)
                return;

            SyntaxListSelection<MemberDeclarationSyntax>.Enumerator en = selectedMembers.GetEnumerator();

            if (!en.MoveNext())
                return;

            TextSpan span = context.Span;

            string refactoringId = GetRefactoringId(en.Current);

            if (refactoringId == null)
                return;

            while (en.MoveNext())
            {
                if (refactoringId != GetRefactoringId(en.Current))
                    return;
            }

            if (refactoringId == RefactoringIdentifiers.UseExpressionBodiedMember
                && context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember))
            {
                context.RegisterRefactoring(
                    UseExpressionBodiedMemberRefactoring.Title,
                    ct => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, selectedMembers, ct),
                    RefactoringIdentifiers.UseExpressionBodiedMember);
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandExpressionBody))
            {
                context.RegisterRefactoring(
                    ExpandExpressionBodyRefactoring.Title,
                    ct => ExpandExpressionBodyRefactoring.RefactorAsync(context.Document, selectedMembers, ct),
                    RefactoringIdentifiers.ExpandExpressionBody);
            }
        }

        private static string GetRefactoringId(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(methodDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var propertyDeclaration = (PropertyDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(propertyDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(indexerDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(operatorDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = conversionOperatorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(conversionOperatorDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(constructorDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
                case SyntaxKind.DestructorDeclaration:
                    {
                        var destructorDeclaration = (DestructorDeclarationSyntax)memberDeclaration;

                        ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ExpandExpressionBody;
                        }
                        else if (UseExpressionBodiedMemberRefactoring.CanRefactor(destructorDeclaration))
                        {
                            return RefactoringIdentifiers.UseExpressionBodiedMember;
                        }

                        return null;
                    }
            }

            return null;
        }
    }
}
