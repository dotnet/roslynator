// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class GenericSyntax
    {
        public static TypeParameterListSyntax GetTypeParameterList(SyntaxNode node)
        {
            TypeParameterListSyntax typeParameterList;

            if (!TryGetTypeParameterList(node, out typeParameterList))
                throw new ArgumentException("", nameof(node));

            return typeParameterList;
        }

        public static bool TryGetTypeParameterList(SyntaxNode node, out TypeParameterListSyntax typeParameterList)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        typeParameterList = ((ClassDeclarationSyntax)node).TypeParameterList;
                        return true;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        typeParameterList = ((DelegateDeclarationSyntax)node).TypeParameterList;
                        return true;
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        typeParameterList = ((InterfaceDeclarationSyntax)node).TypeParameterList;
                        return true;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        typeParameterList = ((LocalFunctionStatementSyntax)node).TypeParameterList;
                        return true;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        typeParameterList = ((MethodDeclarationSyntax)node).TypeParameterList;
                        return true;
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        typeParameterList = ((StructDeclarationSyntax)node).TypeParameterList;
                        return true;
                    }
            }

            typeParameterList = null;
            return false;
        }

        public static SyntaxList<TypeParameterConstraintClauseSyntax> GetConstraintClauses(SyntaxNode node)
        {
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses;

            if (!TryGetConstraintClauses(node, out constraintClauses))
                throw new ArgumentException("", nameof(node));

            return constraintClauses;
        }

        public static bool HasConstraintClauses(SyntaxNode node)
        {
            return TryGetConstraintClauses(node, out SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses);
        }

        public static bool TryGetConstraintClauses(SyntaxNode node, out SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        constraintClauses = ((ClassDeclarationSyntax)node).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        constraintClauses = ((DelegateDeclarationSyntax)node).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        constraintClauses = ((InterfaceDeclarationSyntax)node).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        constraintClauses = ((LocalFunctionStatementSyntax)node).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        constraintClauses = ((MethodDeclarationSyntax)node).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        constraintClauses = ((StructDeclarationSyntax)node).ConstraintClauses;
                        return true;
                    }
            }

            constraintClauses = default(SyntaxList<TypeParameterConstraintClauseSyntax>);
            return false;
        }

        public static SeparatedSyntaxList<TypeParameterConstraintSyntax> GetContainingList(TypeParameterConstraintSyntax constraint)
        {
            SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints;

            if (!TryGetContainingList(constraint, out constraints))
                throw new ArgumentException("", nameof(constraint));

            return constraints;
        }

        public static bool TryGetContainingList(TypeParameterConstraintSyntax constraint, out SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            if (constraint.IsParentKind(SyntaxKind.TypeParameterConstraintClause))
            {
                var constraintClause = (TypeParameterConstraintClauseSyntax)constraint.Parent;

                constraints = constraintClause.Constraints;
                return true;
            }

            constraints = default(SeparatedSyntaxList<TypeParameterConstraintSyntax>);
            return false;
        }

        public static SyntaxNode WithTypeParameterList(SyntaxNode node, TypeParameterListSyntax typeParameterList)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithTypeParameterList(typeParameterList);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithTypeParameterList(typeParameterList);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithTypeParameterList(typeParameterList);
                case SyntaxKind.LocalFunctionStatement:
                    return ((LocalFunctionStatementSyntax)node).WithTypeParameterList(typeParameterList);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithTypeParameterList(typeParameterList);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithTypeParameterList(typeParameterList);
            }

            Debug.Fail(node.Kind().ToString());

            return node;
        }

        public static SyntaxNode WithConstraintClauses(SyntaxNode node, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
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

        public static SyntaxNode RemoveConstraintClauses(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)node;

                        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = classDeclaration.ConstraintClauses;

                        if (!constraintClauses.Any())
                            break;

                        TypeParameterConstraintClauseSyntax first = constraintClauses.First();

                        SyntaxToken token = first.WhereKeyword.GetPreviousToken();

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(constraintClauses.Last().GetTrailingTrivia());

                        return classDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)node;

                        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = interfaceDeclaration.ConstraintClauses;

                        if (!constraintClauses.Any())
                            break;

                        TypeParameterConstraintClauseSyntax first = constraintClauses.First();

                        SyntaxToken token = first.WhereKeyword.GetPreviousToken();

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(constraintClauses.Last().GetTrailingTrivia());

                        return interfaceDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)node;

                        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = structDeclaration.ConstraintClauses;

                        if (!constraintClauses.Any())
                            break;

                        TypeParameterConstraintClauseSyntax first = constraintClauses.First();

                        SyntaxToken token = first.WhereKeyword.GetPreviousToken();

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(constraintClauses.Last().GetTrailingTrivia());

                        return structDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)node;

                        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = delegateDeclaration.ConstraintClauses;

                        if (!constraintClauses.Any())
                            break;

                        TypeParameterConstraintClauseSyntax first = constraintClauses.First();

                        SyntaxToken token = first.WhereKeyword.GetPreviousToken();

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(constraintClauses.Last().GetTrailingTrivia());

                        return delegateDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;

                        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = methodDeclaration.ConstraintClauses;

                        if (!constraintClauses.Any())
                            break;

                        TypeParameterConstraintClauseSyntax first = constraintClauses.First();

                        SyntaxToken token = first.WhereKeyword.GetPreviousToken();

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(constraintClauses.Last().GetTrailingTrivia());

                        return methodDeclaration
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)node;

                        SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = localFunctionStatement.ConstraintClauses;

                        if (!constraintClauses.Any())
                            break;

                        TypeParameterConstraintClauseSyntax first = constraintClauses.First();

                        SyntaxToken token = first.WhereKeyword.GetPreviousToken();

                        SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                            .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                            .AddRange(constraintClauses.Last().GetTrailingTrivia());

                        return localFunctionStatement
                            .ReplaceToken(token, token.WithTrailingTrivia(trivia))
                            .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
                    }
            }

            return node;
        }

        public static SyntaxList<TypeParameterConstraintClauseSyntax> GetContainingList(TypeParameterConstraintClauseSyntax constraintClause)
        {
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses;

            if (!TryGetContainingList(constraintClause, out constraintClauses))
                throw new ArgumentException("", nameof(constraintClause));

            return constraintClauses;
        }

        public static bool TryGetContainingList(TypeParameterConstraintClauseSyntax constraintClause, out SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            SyntaxNode parent = constraintClause.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        constraintClauses = ((ClassDeclarationSyntax)parent).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        constraintClauses = ((DelegateDeclarationSyntax)parent).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        constraintClauses = ((InterfaceDeclarationSyntax)parent).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        constraintClauses = ((LocalFunctionStatementSyntax)parent).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        constraintClauses = ((MethodDeclarationSyntax)parent).ConstraintClauses;
                        return true;
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        constraintClauses = ((StructDeclarationSyntax)parent).ConstraintClauses;
                        return true;
                    }
            }

            constraintClauses = default(SyntaxList<TypeParameterConstraintClauseSyntax>);
            return false;
        }
    }
}