// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal readonly struct ParameterListInfo
    {
        public ParameterListInfo(MemberDeclarationSyntax declaration, BaseParameterListSyntax parameterList)
        {
            Declaration = declaration;
            ParameterList = parameterList;
        }

        public MemberDeclarationSyntax Declaration { get; }

        public BaseParameterListSyntax ParameterList { get; }

        public SeparatedSyntaxList<ParameterSyntax> Parameters => ParameterList?.Parameters ?? default;

        public static ParameterListInfo Create(SyntaxNode node)
        {
            ParameterListInfo info = CreateImpl(node);

            if (info.Declaration == null)
                info = CreateImpl(node.Parent);

            return info;
        }

        private static ParameterListInfo CreateImpl(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)node;
                        return new ParameterListInfo(methodDeclaration, methodDeclaration.ParameterList);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var constructorDeclaration = (ConstructorDeclarationSyntax)node;
                        return new ParameterListInfo(constructorDeclaration, constructorDeclaration.ParameterList);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var operatorDeclaration = (OperatorDeclarationSyntax)node;
                        return new ParameterListInfo(operatorDeclaration, operatorDeclaration.ParameterList);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var conversionOperatorDeclaration = (ConversionOperatorDeclarationSyntax)node;
                        return new ParameterListInfo(conversionOperatorDeclaration, conversionOperatorDeclaration.ParameterList);
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        var delegateDeclaration = (DelegateDeclarationSyntax)node;
                        return new ParameterListInfo(delegateDeclaration, delegateDeclaration.ParameterList);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var indexerDeclaration = (IndexerDeclarationSyntax)node;
                        return new ParameterListInfo(indexerDeclaration, indexerDeclaration.ParameterList);
                    }
            }

            return default;
        }
    }
}
