// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

            RefactoringDescriptor refactoring = GetRefactoringDescriptor(selectedMembers.First());

            if (refactoring.Id == null)
                return;

            if (!RefactoringDescriptorComparer.Id.Equals(refactoring, GetRefactoringDescriptor(selectedMembers.Last())))
                return;

            if (RefactoringDescriptorComparer.Id.Equals(refactoring, RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, selectedMembers, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }
            else if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertExpressionBodyToBlockBody))
            {
                context.RegisterRefactoring(
                    ConvertExpressionBodyToBlockBodyRefactoring.Title,
                    ct => ConvertExpressionBodyToBlockBodyRefactoring.RefactorAsync(context.Document, selectedMembers, ct),
                    RefactoringDescriptors.ConvertExpressionBodyToBlockBody);
            }
        }

        private static RefactoringDescriptor GetRefactoringDescriptor(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = methodDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(methodDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
                case PropertyDeclarationSyntax propertyDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = propertyDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(propertyDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
                case IndexerDeclarationSyntax indexerDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = indexerDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(indexerDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
                case OperatorDeclarationSyntax operatorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = operatorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(operatorDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
                case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = conversionOperatorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(conversionOperatorDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
                case ConstructorDeclarationSyntax constructorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = constructorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(constructorDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
                case DestructorDeclarationSyntax destructorDeclaration:
                    {
                        ArrowExpressionClauseSyntax expressionBody = destructorDeclaration.ExpressionBody;

                        if (expressionBody != null)
                        {
                            if (ExpandExpressionBodyAnalysis.IsFixable(expressionBody))
                                return RefactoringDescriptors.ConvertExpressionBodyToBlockBody;
                        }
                        else if (ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(destructorDeclaration))
                        {
                            return RefactoringDescriptors.ConvertBlockBodyToExpressionBody;
                        }

                        return default;
                    }
            }

            return default;
        }
    }
}
