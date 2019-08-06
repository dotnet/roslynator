// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            switch (memberDeclaration)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
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
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
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
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
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
                case OperatorDeclarationSyntax operatorDeclaration:
                    {
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
                case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
                    {
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
                case ConstructorDeclarationSyntax constructorDeclaration:
                    {
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
                case DestructorDeclarationSyntax destructorDeclaration:
                    {
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
