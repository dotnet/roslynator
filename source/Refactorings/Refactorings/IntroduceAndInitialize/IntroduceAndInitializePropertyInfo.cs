// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.IntroduceAndInitialize
{
    internal class IntroduceAndInitializePropertyInfo : IntroduceAndInitializeInfo
    {
        private string _name;

        public IntroduceAndInitializePropertyInfo(ParameterSyntax parameter)
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

        public override MemberDeclarationSyntax CreateDeclaration()
        {
            return PropertyDeclaration(PropertyKind.AutoPropertyWithPrivateSet, Type, Name)
                .WithModifiers(Modifiers.Public());
        }
    }
}
