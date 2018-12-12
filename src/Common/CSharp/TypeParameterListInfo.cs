// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal readonly struct TypeParameterListInfo
    {
        public TypeParameterListInfo(MemberDeclarationSyntax declaration, TypeParameterListSyntax typeParameterList)
        {
            Declaration = declaration;
            TypeParameterList = typeParameterList;
        }

        public MemberDeclarationSyntax Declaration { get; }

        public TypeParameterListSyntax TypeParameterList { get; }

        public SeparatedSyntaxList<TypeParameterSyntax> Parameters => TypeParameterList?.Parameters ?? default;

        public static TypeParameterListInfo Create(SyntaxNode node)
        {
            TypeParameterListInfo info = CreateImpl(node);

            if (info.Declaration == null)
                info = CreateImpl(node.Parent);

            return info;
        }

        private static TypeParameterListInfo CreateImpl(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var classDeclaration = (ClassDeclarationSyntax)node;
                        return new TypeParameterListInfo(classDeclaration, classDeclaration.TypeParameterList);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var interfaceDeclaration = (InterfaceDeclarationSyntax)node;
                        return new TypeParameterListInfo(interfaceDeclaration, interfaceDeclaration.TypeParameterList);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var structDeclaration = (StructDeclarationSyntax)node;
                        return new TypeParameterListInfo(structDeclaration, structDeclaration.TypeParameterList);
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;
                        return new TypeParameterListInfo(methodDeclaration, methodDeclaration.TypeParameterList);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)node;
                        return new TypeParameterListInfo(delegateDeclaration, delegateDeclaration.TypeParameterList);
                    }
            }

            return default;
        }
    }
}
