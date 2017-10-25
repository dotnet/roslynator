// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    public struct TypeParameterInfo
    {
        private static TypeParameterInfo Default { get; } = new TypeParameterInfo();

        private TypeParameterInfo(
            TypeParameterSyntax typeParameter,
            SyntaxNode declaration,
            TypeParameterListSyntax typeParameterList,
            SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            TypeParameter = typeParameter;
            Declaration = declaration;
            TypeParameterList = typeParameterList;
            ConstraintClauses = constraintClauses;
        }

        public TypeParameterSyntax TypeParameter { get; }

        public string Name
        {
            get { return TypeParameter?.Identifier.ValueText; }
        }

        public SyntaxNode Declaration { get; }

        public TypeParameterListSyntax TypeParameterList { get; }

        public SeparatedSyntaxList<TypeParameterSyntax> TypeParameters
        {
            get { return TypeParameterList?.Parameters ?? default(SeparatedSyntaxList<TypeParameterSyntax>); }
        }

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses { get; }

        public TypeParameterConstraintClauseSyntax ConstraintClause
        {
            get
            {
                string name = Name;

                foreach (TypeParameterConstraintClauseSyntax constraintClause in ConstraintClauses)
                {
                    if (string.Equals(name, constraintClause.NameText(), StringComparison.Ordinal))
                        return constraintClause;
                }

                return null;
            }
        }

        public bool Success
        {
            get { return TypeParameter != null; }
        }

        internal static TypeParameterInfo Create(TypeParameterSyntax typeParameter)
        {
            if (!(typeParameter.Parent is TypeParameterListSyntax typeParameterList))
                return Default;

            SyntaxNode parent = typeParameterList.Parent;

            switch (parent?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)parent;
                        return new TypeParameterInfo(typeParameter, classDeclaration, typeParameterList, classDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)parent;
                        return new TypeParameterInfo(typeParameter, delegateDeclaration, typeParameterList, delegateDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)parent;
                        return new TypeParameterInfo(typeParameter, interfaceDeclaration, typeParameterList, interfaceDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)parent;
                        return new TypeParameterInfo(typeParameter, localFunctionStatement, typeParameterList, localFunctionStatement.ConstraintClauses);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)parent;
                        return new TypeParameterInfo(typeParameter, methodDeclaration, typeParameterList, methodDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)parent;
                        return new TypeParameterInfo(typeParameter, structDeclaration, typeParameterList, structDeclaration.ConstraintClauses);
                    }
            }

            return Default;
        }

        internal static TypeParameterInfo Create(SyntaxNode declaration, string name)
        {
            switch (declaration?.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)declaration;

                        TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

                        if (typeParameterList == null)
                            return Default;

                        TypeParameterSyntax typeParameter = typeParameterList.GetTypeParameterByName(name);

                        if (typeParameter == null)
                            return Default;

                        return new TypeParameterInfo(typeParameter, classDeclaration, typeParameterList, classDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)declaration;

                        TypeParameterListSyntax typeParameterList = delegateDeclaration.TypeParameterList;

                        if (typeParameterList == null)
                            return Default;

                        TypeParameterSyntax typeParameter = typeParameterList.GetTypeParameterByName(name);

                        if (typeParameter == null)
                            return Default;

                        return new TypeParameterInfo(typeParameter, delegateDeclaration, typeParameterList, delegateDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)declaration;

                        TypeParameterListSyntax typeParameterList = interfaceDeclaration.TypeParameterList;

                        if (typeParameterList == null)
                            return Default;

                        TypeParameterSyntax typeParameter = typeParameterList.GetTypeParameterByName(name);

                        if (typeParameter == null)
                            return Default;

                        return new TypeParameterInfo(typeParameter, interfaceDeclaration, typeParameterList, interfaceDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.LocalFunctionStatement:
                    {
                        var localFunctionStatement = (LocalFunctionStatementSyntax)declaration;

                        TypeParameterListSyntax typeParameterList = localFunctionStatement.TypeParameterList;

                        if (typeParameterList == null)
                            return Default;

                        TypeParameterSyntax typeParameter = typeParameterList.GetTypeParameterByName(name);

                        if (typeParameter == null)
                            return Default;

                        return new TypeParameterInfo(typeParameter, localFunctionStatement, typeParameterList, localFunctionStatement.ConstraintClauses);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)declaration;

                        TypeParameterListSyntax typeParameterList = methodDeclaration.TypeParameterList;

                        if (typeParameterList == null)
                            return Default;

                        TypeParameterSyntax typeParameter = typeParameterList.GetTypeParameterByName(name);

                        if (typeParameter == null)
                            return Default;

                        return new TypeParameterInfo(typeParameter, methodDeclaration, typeParameterList, methodDeclaration.ConstraintClauses);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)declaration;

                        TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

                        if (typeParameterList == null)
                            return Default;

                        TypeParameterSyntax typeParameter = typeParameterList.GetTypeParameterByName(name);

                        if (typeParameter == null)
                            return Default;

                        return new TypeParameterInfo(typeParameter, structDeclaration, typeParameterList, structDeclaration.ConstraintClauses);
                    }
            }

            return Default;
        }

        public override string ToString()
        {
            return TypeParameter?.ToString() ?? base.ToString();
        }
    }
}