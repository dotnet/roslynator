// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertBodyAndExpressionBodyRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
        {
            if (selectedMembers.Count <= 1)
                return;

            TextSpan span = context.Span;

            string refactoringId = GetRefactoringId(selectedMembers.First());

            if (refactoringId == null)
                return;

            if (refactoringId != GetRefactoringId(selectedMembers.Last()))
                return;

            if (refactoringId == RefactoringIdentifiers.ConvertBlockBodyToExpressionBody
                && context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertBlockBodyToExpressionBody))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, selectedMembers, ct),
                    RefactoringIdentifiers.ConvertBlockBodyToExpressionBody);
            }
            else if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertExpressionBodyToBlockBody))
            {
                context.RegisterRefactoring(
                    ConvertExpressionBodyToBlockBodyRefactoring.Title,
                    ct => ConvertExpressionBodyToBlockBodyRefactoring.RefactorAsync(context.Document, selectedMembers, ct),
                    RefactoringIdentifiers.ConvertExpressionBodyToBlockBody);
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
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(methodDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(propertyDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(indexerDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
                case OperatorDeclarationSyntax operatorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(operatorDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
                case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = conversionOperatorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(conversionOperatorDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
                case ConstructorDeclarationSyntax constructorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(constructorDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
                case DestructorDeclarationSyntax destructorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringIdentifiers.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(destructorDeclaration))
                        {
                            return RefactoringIdentifiers.ConvertBlockBodyToExpressionBody;
                        }

                        return null;
                    }
            }

            return null;
        }
    }
}
