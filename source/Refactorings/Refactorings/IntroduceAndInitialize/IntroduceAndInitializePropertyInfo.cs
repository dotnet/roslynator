using System;
using Microsoft.CodeAnalysis.CSharp;
// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.IntroduceAndInitialize
{
    internal class IntroduceAndInitializePropertyInfo : IntroduceAndInitializeInfo
    {
        private string _name;

        public IntroduceAndInitializePropertyInfo(ParameterSyntax parameter, bool supportsCSharp6)
            : base(parameter)
        {
            SupportsCSharp6 = supportsCSharp6;
        }

        public bool SupportsCSharp6 { get; }

        public override string Name
        {
            get
            {
                if (_name == null)
                    _name = StringUtility.FirstCharToUpper(ParameterName);

                return _name;
            }
        }

        public override SyntaxKind Kind
        {
            get { return SyntaxKind.PropertyDeclaration; }
        }

        public override MemberDeclarationSyntax CreateDeclaration()
        {
            PropertyKind propertyKind = (SupportsCSharp6)
                ? PropertyKind.ReadOnlyAutoProperty
                : PropertyKind.AutoPropertyWithPrivateSet;

            return PropertyDeclaration(propertyKind, ModifierFactory.Public(), Type, Name);
        }
    }
}
