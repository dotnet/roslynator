// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.IntroduceAndInitialize
{
    internal class IntroduceAndInitializeFieldRefactoring : IntroduceAndInitializeRefactoring
    {
        private string _name;

        public IntroduceAndInitializeFieldRefactoring(ParameterSyntax parameter, bool prefixFieldIndentifierWithUnderscore = false)
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

        protected override MemberDeclarationSyntax CreateDeclaration()
        {
            return FieldDeclaration(Type, Name)
                .WithModifiers(new SyntaxKind[] { SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword });
        }

        protected override int GetDeclarationIndex(SyntaxList<MemberDeclarationSyntax> members)
        {
            int index = IndexOfLastField(members) + 1;

            if (index == 0)
                index = members.IndexOf(Constructor);

            return index;
        }

        private static int IndexOfLastField(SyntaxList<MemberDeclarationSyntax> members)
        {
            for (int i = members.Count - 1; i >= 0; i--)
            {
                if (members[i].IsKind(SyntaxKind.FieldDeclaration))
                    return i;
            }

            return -1;
        }
    }
}
