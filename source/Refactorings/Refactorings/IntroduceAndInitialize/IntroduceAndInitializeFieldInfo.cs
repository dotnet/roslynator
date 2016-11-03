// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.IntroduceAndInitialize
{
    internal class IntroduceAndInitializeFieldInfo : IntroduceAndInitializeInfo
    {
        private string _name;

        public IntroduceAndInitializeFieldInfo(ParameterSyntax parameter, bool prefixFieldIndentifierWithUnderscore = false)
            : base(parameter)
        {
            PrefixFieldIdentifierWithUnderscore = prefixFieldIndentifierWithUnderscore;
        }

        public bool PrefixFieldIdentifierWithUnderscore { get; }

        public override string Name
        {
            get
            {
                if (_name == null)
                    _name = TextUtility.ToCamelCase(ParameterName, PrefixFieldIdentifierWithUnderscore);

                return _name;
            }
        }

        public override MemberDeclarationSyntax CreateDeclaration()
        {
            return FieldDeclaration(Modifiers.PrivateReadOnly(), Type, Name);
        }
    }
}
