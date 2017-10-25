// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    public struct GenericInfo
    {
        private static GenericInfo Default { get; } = new GenericInfo();

        private GenericInfo(ClassDeclarationSyntax classDeclaration)
            : this(classDeclaration, SyntaxKind.ClassDeclaration, classDeclaration.TypeParameterList, classDeclaration.ConstraintClauses)
        {
        }

        private GenericInfo(DelegateDeclarationSyntax delegateDeclaration)
            : this(delegateDeclaration, SyntaxKind.DelegateDeclaration, delegateDeclaration.TypeParameterList, delegateDeclaration.ConstraintClauses)
        {
        }

        private GenericInfo(InterfaceDeclarationSyntax interfaceDeclaration)
            : this(interfaceDeclaration, SyntaxKind.InterfaceDeclaration, interfaceDeclaration.TypeParameterList, interfaceDeclaration.ConstraintClauses)
        {
        }

        private GenericInfo(LocalFunctionStatementSyntax localFunctionStatement)
            : this(localFunctionStatement, SyntaxKind.LocalFunctionStatement, localFunctionStatement.TypeParameterList, localFunctionStatement.ConstraintClauses)
        {
        }

        private GenericInfo(MethodDeclarationSyntax methodDeclaration)
            : this(methodDeclaration, SyntaxKind.MethodDeclaration, methodDeclaration.TypeParameterList, methodDeclaration.ConstraintClauses)
        {
        }

        private GenericInfo(StructDeclarationSyntax structDeclaration)
            : this(structDeclaration, SyntaxKind.StructDeclaration, structDeclaration.TypeParameterList, structDeclaration.ConstraintClauses)
        {
        }

        private GenericInfo(
            SyntaxNode declaration,
            SyntaxKind kind,
            TypeParameterListSyntax typeParameterList,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            Declaration = declaration;
            Kind = kind;
            TypeParameterList = typeParameterList;
            ConstraintClauses = constraintClauses;
        }

        public SyntaxNode Declaration { get; }

        public SyntaxKind Kind { get; }

        public TypeParameterListSyntax TypeParameterList { get; }

        public SeparatedSyntaxList<TypeParameterSyntax> TypeParameters
        {
            get { return TypeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>); }
        }

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses { get; }

        public TypeParameterSyntax FindTypeParameter(string name)
        {
            foreach (TypeParameterSyntax typeParameter in TypeParameters)
            {
                if (string.Equals(name, typeParameter.Identifier.ValueText, StringComparison.Ordinal))
                    return typeParameter;
            }

            return null;
        }

        public TypeParameterConstraintClauseSyntax FindConstraintClause(string name)
        {
            foreach (TypeParameterConstraintClauseSyntax constraintClause in ConstraintClauses)
            {
                if (string.Equals(name, constraintClause.NameText(), StringComparison.Ordinal))
                    return constraintClause;
            }

            return null;
        }

        public bool Success
        {
            get { return Declaration != null; }
        }

        internal static GenericInfo Create(TypeParameterConstraintSyntax typeParameterConstraint)
        {
            return Create(typeParameterConstraint?.Parent as TypeParameterConstraintClauseSyntax);
        }

        internal static GenericInfo Create(TypeParameterConstraintClauseSyntax constraintClause)
        {
            return Create(constraintClause?.Parent);
        }

        internal static GenericInfo Create(SyntaxNode declaration)
        {
            switch (declaration?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)declaration;
                        return new GenericInfo(classDeclaration, SyntaxKind.ClassDeclaration, classDeclaration.TypeParameterList, classDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)declaration;
                        return new GenericInfo(delegateDeclaration, SyntaxKind.DelegateDeclaration, delegateDeclaration.TypeParameterList, delegateDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;
                        return new GenericInfo(interfaceDeclaration, SyntaxKind.InterfaceDeclaration, interfaceDeclaration.TypeParameterList, interfaceDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)declaration;
                        return new GenericInfo(localFunctionStatement, SyntaxKind.LocalFunctionStatement, localFunctionStatement.TypeParameterList, localFunctionStatement.ConstraintClauses);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)declaration;
                        return new GenericInfo(methodDeclaration, SyntaxKind.MethodDeclaration, methodDeclaration.TypeParameterList, methodDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)declaration;
                        return new GenericInfo(structDeclaration, SyntaxKind.StructDeclaration, structDeclaration.TypeParameterList, structDeclaration.ConstraintClauses);
                    }
            }

            return Default;
        }

        internal static GenericInfo Create(TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList == null)
                return Default;

            SyntaxNode parent = typeParameterList.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)parent;
                        return new GenericInfo(classDeclaration, SyntaxKind.ClassDeclaration, typeParameterList, classDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)parent;
                        return new GenericInfo(delegateDeclaration, SyntaxKind.DelegateDeclaration, typeParameterList, delegateDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)parent;
                        return new GenericInfo(interfaceDeclaration, SyntaxKind.InterfaceDeclaration, typeParameterList, interfaceDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)parent;
                        return new GenericInfo(localFunctionStatement, SyntaxKind.LocalFunctionStatement, typeParameterList, localFunctionStatement.ConstraintClauses);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)parent;
                        return new GenericInfo(methodDeclaration, SyntaxKind.MethodDeclaration, typeParameterList, methodDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)parent;
                        return new GenericInfo(structDeclaration, SyntaxKind.StructDeclaration, typeParameterList, structDeclaration.ConstraintClauses);
                    }
            }

            return Default;
        }

        internal static GenericInfo Create(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                return Default;

            return new GenericInfo(classDeclaration);
        }

        internal static GenericInfo Create(DelegateDeclarationSyntax delegateDeclaration)
        {
            if (delegateDeclaration == null)
                return Default;

            return new GenericInfo(delegateDeclaration);
        }

        internal static GenericInfo Create(InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                return Default;

            return new GenericInfo(interfaceDeclaration);
        }

        internal static GenericInfo Create(LocalFunctionStatementSyntax localFunctionStatement)
        {
            if (localFunctionStatement == null)
                return Default;

            return new GenericInfo(localFunctionStatement);
        }

        internal static GenericInfo Create(MethodDeclarationSyntax methodDeclaration)
        {
            if (methodDeclaration == null)
                return Default;

            return new GenericInfo(methodDeclaration);
        }

        internal static GenericInfo Create(StructDeclarationSyntax structDeclaration)
        {
            if (structDeclaration == null)
                return Default;

            return new GenericInfo(structDeclaration);
        }

        public GenericInfo WithTypeParameterList(TypeParameterListSyntax typeParameterList)
        {
            switch (Kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return new GenericInfo(((ClassDeclarationSyntax)Declaration).WithTypeParameterList(typeParameterList));
                case SyntaxKind.DelegateDeclaration:
                    return new GenericInfo(((DelegateDeclarationSyntax)Declaration).WithTypeParameterList(typeParameterList));
                case SyntaxKind.InterfaceDeclaration:
                    return new GenericInfo(((InterfaceDeclarationSyntax)Declaration).WithTypeParameterList(typeParameterList));
                case SyntaxKind.LocalFunctionStatement:
                    return new GenericInfo(((LocalFunctionStatementSyntax)Declaration).WithTypeParameterList(typeParameterList));
                case SyntaxKind.MethodDeclaration:
                    return new GenericInfo(((MethodDeclarationSyntax)Declaration).WithTypeParameterList(typeParameterList));
                case SyntaxKind.StructDeclaration:
                    return new GenericInfo(((StructDeclarationSyntax)Declaration).WithTypeParameterList(typeParameterList));
                case SyntaxKind.None:
                    return this;
            }

            Debug.Fail(Kind.ToString());

            return this;
        }

        public GenericInfo RemoveTypeParameter(TypeParameterSyntax typeParameter)
        {
            switch (Kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return new GenericInfo(((ClassDeclarationSyntax)Declaration).WithTypeParameterList(RemoveTypeParameterHelper(typeParameter)));
                case SyntaxKind.DelegateDeclaration:
                    return new GenericInfo(((DelegateDeclarationSyntax)Declaration).WithTypeParameterList(RemoveTypeParameterHelper(typeParameter)));
                case SyntaxKind.InterfaceDeclaration:
                    return new GenericInfo(((InterfaceDeclarationSyntax)Declaration).WithTypeParameterList(RemoveTypeParameterHelper(typeParameter)));
                case SyntaxKind.LocalFunctionStatement:
                    return new GenericInfo(((LocalFunctionStatementSyntax)Declaration).WithTypeParameterList(RemoveTypeParameterHelper(typeParameter)));
                case SyntaxKind.MethodDeclaration:
                    return new GenericInfo(((MethodDeclarationSyntax)Declaration).WithTypeParameterList(RemoveTypeParameterHelper(typeParameter)));
                case SyntaxKind.StructDeclaration:
                    return new GenericInfo(((StructDeclarationSyntax)Declaration).WithTypeParameterList(RemoveTypeParameterHelper(typeParameter)));
                case SyntaxKind.None:
                    return this;
            }

            Debug.Fail(Kind.ToString());
            return this;
        }

        private TypeParameterListSyntax RemoveTypeParameterHelper(TypeParameterSyntax typeParameter)
        {
            SeparatedSyntaxList<TypeParameterSyntax> parameters = TypeParameters;

            return (parameters.Count == 1)
                ? default(TypeParameterListSyntax)
                : TypeParameterList.WithParameters(parameters.Remove(typeParameter));
        }

        public GenericInfo WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            switch (Kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return new GenericInfo(((ClassDeclarationSyntax)Declaration).WithConstraintClauses(constraintClauses));
                case SyntaxKind.DelegateDeclaration:
                    return new GenericInfo(((DelegateDeclarationSyntax)Declaration).WithConstraintClauses(constraintClauses));
                case SyntaxKind.InterfaceDeclaration:
                    return new GenericInfo(((InterfaceDeclarationSyntax)Declaration).WithConstraintClauses(constraintClauses));
                case SyntaxKind.LocalFunctionStatement:
                    return new GenericInfo(((LocalFunctionStatementSyntax)Declaration).WithConstraintClauses(constraintClauses));
                case SyntaxKind.MethodDeclaration:
                    return new GenericInfo(((MethodDeclarationSyntax)Declaration).WithConstraintClauses(constraintClauses));
                case SyntaxKind.StructDeclaration:
                    return new GenericInfo(((StructDeclarationSyntax)Declaration).WithConstraintClauses(constraintClauses));
                case SyntaxKind.None:
                    return this;
            }

            Debug.Fail(Kind.ToString());
            return this;
        }

        public GenericInfo RemoveConstraintClause(TypeParameterConstraintClauseSyntax constraintClause)
        {
            switch (Kind)
            {
                case SyntaxKind.ClassDeclaration:
                    return new GenericInfo(((ClassDeclarationSyntax)Declaration).WithConstraintClauses(ConstraintClauses.Remove(constraintClause)));
                case SyntaxKind.DelegateDeclaration:
                    return new GenericInfo(((DelegateDeclarationSyntax)Declaration).WithConstraintClauses(ConstraintClauses.Remove(constraintClause)));
                case SyntaxKind.InterfaceDeclaration:
                    return new GenericInfo(((InterfaceDeclarationSyntax)Declaration).WithConstraintClauses(ConstraintClauses.Remove(constraintClause)));
                case SyntaxKind.LocalFunctionStatement:
                    return new GenericInfo(((LocalFunctionStatementSyntax)Declaration).WithConstraintClauses(ConstraintClauses.Remove(constraintClause)));
                case SyntaxKind.MethodDeclaration:
                    return new GenericInfo(((MethodDeclarationSyntax)Declaration).WithConstraintClauses(ConstraintClauses.Remove(constraintClause)));
                case SyntaxKind.StructDeclaration:
                    return new GenericInfo(((StructDeclarationSyntax)Declaration).WithConstraintClauses(ConstraintClauses.Remove(constraintClause)));
                case SyntaxKind.None:
                    return this;
            }

            Debug.Fail(Kind.ToString());
            return this;
        }

        public GenericInfo RemoveConstraintClauses()
        {
            if (!ConstraintClauses.Any())
                return this;

            TypeParameterConstraintClauseSyntax first = ConstraintClauses.First();

            SyntaxToken token = first.WhereKeyword.GetPreviousToken();

            SyntaxTriviaList trivia = token.TrailingTrivia.EmptyIfWhitespace()
                .AddRange(first.GetLeadingTrivia().EmptyIfWhitespace())
                .AddRange(ConstraintClauses.Last().GetTrailingTrivia());

            return Create(Declaration.ReplaceToken(token, token.WithTrailingTrivia(trivia)))
                .WithConstraintClauses(default(SyntaxList<TypeParameterConstraintClauseSyntax>));
        }

        public override string ToString()
        {
            return Declaration?.ToString() ?? base.ToString();
        }
    }
}