// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.IntroduceAndInitialize
{
    internal class IntroduceAndInitializePropertyRefactoring : IntroduceAndInitializeRefactoring
    {
        private string _name;

        public IntroduceAndInitializePropertyRefactoring(ParameterSyntax parameter)
            : base(parameter)
        {
        }

        public override string Name
        {
            get
            {
                if (_name == null)
                    _name = TextUtility.FirstCharToUpper(ParameterName);

                return _name;
            }
        }

        protected override MemberDeclarationSyntax CreateDeclaration()
        {
            return PropertyDeclaration(PropertyKind.AutoPropertyWithPrivateSet, Type, Name)
                .WithModifiers(SyntaxKind.PublicKeyword);
        }

        protected override int GetDeclarationIndex(SyntaxList<MemberDeclarationSyntax> members)
        {
            int fieldIndex = IndexOfLastProperty(members) + 1;

            if (fieldIndex == 0)
            {
                int constructorIndex = members.IndexOf(Constructor);
                fieldIndex = constructorIndex + 1;

                for (int i = fieldIndex; i < members.Count; i++)
                {
                    if (members[i].IsKind(SyntaxKind.ConstructorDeclaration))
                    {
                        fieldIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return fieldIndex;
        }

        private static int IndexOfLastProperty(SyntaxList<MemberDeclarationSyntax> members)
        {
            for (int i = members.Count - 1; i >= 0; i--)
            {
                if (members[i].IsKind(SyntaxKind.PropertyDeclaration))
                    return i;
            }

            return -1;
        }
    }
}
