// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddTypeParameterRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

            if (typeParameterList != null)
            {
                if (context.Span.IsEmptyAndContainedInSpan(typeParameterList))
                    RegisterRefactoring(context, classDeclaration);
            }
            else
            {
                TextSpan span = context.Span;

                SyntaxToken identifier = classDeclaration.Identifier;

                if (!identifier.IsMissing
                    && span.Start >= identifier.Span.End)
                {
                    SyntaxToken openBrace = classDeclaration.OpenBraceToken;

                    if (!openBrace.IsMissing
                        && span.End <= openBrace.Span.Start)
                    {
                        RegisterRefactoring(context, classDeclaration);
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

            if (typeParameterList != null)
            {
                if (context.Span.IsEmptyAndContainedInSpan(typeParameterList))
                    RegisterRefactoring(context, structDeclaration);
            }
            else
            {
                TextSpan span = context.Span;

                SyntaxToken identifier = structDeclaration.Identifier;

                if (!identifier.IsMissing
                    && span.Start >= identifier.Span.End)
                {
                    SyntaxToken openBrace = structDeclaration.OpenBraceToken;

                    if (!openBrace.IsMissing
                        && span.End <= openBrace.Span.Start)
                    {
                        RegisterRefactoring(context, structDeclaration);
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            TypeParameterListSyntax typeParameterList = interfaceDeclaration.TypeParameterList;

            if (typeParameterList != null)
            {
                if (context.Span.IsEmptyAndContainedInSpan(typeParameterList))
                    RegisterRefactoring(context, interfaceDeclaration);
            }
            else
            {
                TextSpan span = context.Span;

                SyntaxToken identifier = interfaceDeclaration.Identifier;

                if (!identifier.IsMissing
                    && span.Start >= identifier.Span.End)
                {
                    SyntaxToken openBrace = interfaceDeclaration.OpenBraceToken;

                    if (!openBrace.IsMissing
                        && span.End <= openBrace.Span.Start)
                    {
                        RegisterRefactoring(context, interfaceDeclaration);
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, DelegateDeclarationSyntax delegateDeclaration)
        {
            TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;

            if (typeParameterList != null)
            {
                if (context.Span.IsEmptyAndContainedInSpan(typeParameterList))
                    RegisterRefactoring(context, delegateDeclaration);
            }
            else
            {
                TextSpan span = context.Span;

                SyntaxToken identifier = delegateDeclaration.Identifier;

                if (!identifier.IsMissing
                    && span.Start >= identifier.Span.End)
                {
                    ParameterListSyntax parameterList = delegateDeclaration.ParameterList;

                    if (parameterList != null
                        && span.End <= parameterList.Span.Start)
                    {
                        RegisterRefactoring(context, delegateDeclaration);
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, MethodDeclarationSyntax methodDeclaration)
        {
            TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;

            if (typeParameterList != null)
            {
                if (context.Span.IsEmptyAndContainedInSpan(typeParameterList))
                    RegisterRefactoring(context, methodDeclaration);
            }
            else
            {
                TextSpan span = context.Span;

                SyntaxToken identifier = methodDeclaration.Identifier;

                if (!identifier.IsMissing
                    && span.Start >= identifier.Span.End)
                {
                    ParameterListSyntax parameterList = methodDeclaration.ParameterList;

                    if (parameterList != null
                        && span.End <= parameterList.Span.Start
                        && methodDeclaration.BodyOrExpressionBody() != null)
                    {
                        RegisterRefactoring(context, methodDeclaration);
                    }
                }
            }
        }

        internal static void ComputeRefactoring(RefactoringContext context, LocalFunctionStatementSyntax localFunctionStatement)
        {
            TypeParameterListSyntax typeParameterList = localFunctionStatement.TypeParameterList;

            if (typeParameterList != null)
            {
                if (context.Span.IsEmptyAndContainedInSpan(typeParameterList))
                    RegisterRefactoring(context, localFunctionStatement);
            }
            else
            {
                TextSpan span = context.Span;

                SyntaxToken identifier = localFunctionStatement.Identifier;

                if (!identifier.IsMissing
                    && span.Start >= identifier.Span.End)
                {
                    ParameterListSyntax parameterList = localFunctionStatement.ParameterList;

                    if (parameterList != null
                        && span.End <= parameterList.Span.Start)
                    {
                        RegisterRefactoring(context, localFunctionStatement);
                    }
                }
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, SyntaxNode node)
        {
            context.RegisterRefactoring("Add type parameter", c => RefactorAsync(context.Document, node, ConstraintKind.None, c));
            context.RegisterRefactoring("Add type parameter with type constraint", c => RefactorAsync(context.Document, node, ConstraintKind.Type, c));
            //context.RegisterRefactoring("Add type parameter with 'class' constraint", c => RefactorAsync(context.Document, node, ConstraintKind.Class, c));
            //context.RegisterRefactoring("Add type parameter with 'struct' constraint", c => RefactorAsync(context.Document, node, ConstraintKind.Struct, c));
            //context.RegisterRefactoring("Add type parameter with 'new()' constraint", c => RefactorAsync(context.Document, node, ConstraintKind.Constructor, c));
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            ConstraintKind constraintKind,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            TypeParameterConstraintSyntax constraint = CreateConstraint(constraintKind);

            SyntaxNode newNode = node;

            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        newNode = GetNewNode((MethodDeclarationSyntax)node, constraint, semanticModel, cancellationToken);
                        break;
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        newNode = GetNewNode((ClassDeclarationSyntax)node, constraint, semanticModel, cancellationToken);
                        break;
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        newNode = GetNewNode((StructDeclarationSyntax)node, constraint, semanticModel, cancellationToken);
                        break;
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        newNode = GetNewNode((InterfaceDeclarationSyntax)node, constraint, semanticModel, cancellationToken);
                        break;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        newNode = GetNewNode((DelegateDeclarationSyntax)node, constraint, semanticModel, cancellationToken);
                        break;
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        newNode = GetNewNode((LocalFunctionStatementSyntax)node, constraint, semanticModel, cancellationToken);
                        break;
                    }
            }

            return await document.ReplaceNodeAsync(node, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static SyntaxNode GetNewNode(
            MethodDeclarationSyntax methodDeclaration,
            TypeParameterConstraintSyntax constraint,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;

            IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration, cancellationToken);

            string name = GetMethodTypeParameterName(semanticModel, methodDeclaration.BodyOrExpressionBody().SpanStart, cancellationToken);

            MethodDeclarationSyntax newNode = methodDeclaration.AddTypeParameterListParameters(TypeParameter(Identifier(name).WithRenameAnnotation()));

            if (constraint != null)
                newNode = newNode.AddConstraintClauses(TypeParameterConstraintClause(name, constraint));

            return newNode;
        }

        private static SyntaxNode GetNewNode(
            ClassDeclarationSyntax classDeclaration,
            TypeParameterConstraintSyntax constraint,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

            INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            string name = GetTypeParameterName(classDeclaration.OpenBraceToken.SpanStart, semanticModel);

            ClassDeclarationSyntax newNode = classDeclaration.AddTypeParameterListParameters(TypeParameter(Identifier(name).WithRenameAnnotation()));

            if (constraint != null)
                newNode = newNode.AddConstraintClauses(TypeParameterConstraintClause(name, constraint));

            return newNode;
        }

        private static SyntaxNode GetNewNode(
            StructDeclarationSyntax structDeclaration,
            TypeParameterConstraintSyntax typeParameterConstraint,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

            INamedTypeSymbol structSymbol = semanticModel.GetDeclaredSymbol(structDeclaration, cancellationToken);

            string name = GetTypeParameterName(structDeclaration.OpenBraceToken.SpanStart, semanticModel);

            StructDeclarationSyntax newNode = structDeclaration.AddTypeParameterListParameters(TypeParameter(Identifier(name).WithRenameAnnotation()));

            if (typeParameterConstraint != null)
                newNode = newNode.AddConstraintClauses(TypeParameterConstraintClause(name, typeParameterConstraint));

            return newNode;
        }

        private static SyntaxNode GetNewNode(
            InterfaceDeclarationSyntax interfaceDeclaration,
            TypeParameterConstraintSyntax constraint,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeParameterListSyntax typeParameterList = interfaceDeclaration.TypeParameterList;

            INamedTypeSymbol interfaceSymbol = semanticModel.GetDeclaredSymbol(interfaceDeclaration, cancellationToken);

            string name = GetTypeParameterName(interfaceDeclaration.OpenBraceToken.SpanStart, semanticModel);

            InterfaceDeclarationSyntax newNode = interfaceDeclaration.AddTypeParameterListParameters(TypeParameter(Identifier(name).WithRenameAnnotation()));

            if (constraint != null)
                newNode = newNode.AddConstraintClauses(TypeParameterConstraintClause(name, constraint));

            return newNode;
        }

        private static SyntaxNode GetNewNode(
            DelegateDeclarationSyntax delegateDeclaration,
            TypeParameterConstraintSyntax constraint,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;

            INamedTypeSymbol delegateSymbol = semanticModel.GetDeclaredSymbol(delegateDeclaration, cancellationToken);

            int position = (typeParameterList != null)
                ? typeParameterList.SpanStart
                : delegateDeclaration.Identifier.SpanStart;

            string name = GetTypeParameterName(position, semanticModel);

            DelegateDeclarationSyntax newNode = delegateDeclaration.AddTypeParameterListParameters(TypeParameter(Identifier(name).WithRenameAnnotation()));

            if (constraint != null)
                newNode = newNode.AddConstraintClauses(TypeParameterConstraintClause(name, constraint));

            return newNode;
        }

        private static SyntaxNode GetNewNode(
            LocalFunctionStatementSyntax localFunctionStatement,
            TypeParameterConstraintSyntax constraint,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            TypeParameterListSyntax typeParameterList = localFunctionStatement.TypeParameterList;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(localFunctionStatement, cancellationToken);

            string name = GetMethodTypeParameterName(semanticModel, localFunctionStatement.BodyOrExpressionBody().SpanStart, cancellationToken);

            LocalFunctionStatementSyntax newNode = localFunctionStatement.AddTypeParameterListParameters(TypeParameter(Identifier(name).WithRenameAnnotation()));

            if (constraint != null)
                newNode = newNode.AddConstraintClauses(TypeParameterConstraintClause(name, constraint));

            return newNode;
        }

        private static string GetTypeParameterName(int position, SemanticModel semanticModel)
        {
            return NameGenerator.Default.EnsureUniqueName(
                DefaultNames.TypeParameter,
                semanticModel.LookupSymbols(position));
        }

        private static string GetMethodTypeParameterName(SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            return NameGenerator.Default.EnsureUniqueLocalName(
                DefaultNames.TypeParameter,
                semanticModel,
                position,
                cancellationToken: cancellationToken);
        }

        private static TypeParameterConstraintSyntax CreateConstraint(ConstraintKind constraintKind)
        {
            switch (constraintKind)
            {
                case ConstraintKind.Type:
                    return TypeConstraint(ObjectType());
                case ConstraintKind.Class:
                    return ClassConstraint();
                case ConstraintKind.Struct:
                    return StructConstraint();
                case ConstraintKind.Constructor:
                    return ConstructorConstraint();
                default:
                    return null;
            }
        }

        private enum ConstraintKind
        {
            None,
            Type,
            Class,
            Struct,
            Constructor
        }
    }
}
