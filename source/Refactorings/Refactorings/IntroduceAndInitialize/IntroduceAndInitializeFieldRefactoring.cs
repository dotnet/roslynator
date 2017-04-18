// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.IntroduceAndInitialize
{
    internal class IntroduceAndInitializeFieldRefactoring : IntroduceAndInitializeRefactoring
    {
        public IntroduceAndInitializeFieldRefactoring(IntroduceAndInitializeInfo info)
            : base(info)
        {
        }

        public IntroduceAndInitializeFieldRefactoring(IEnumerable<IntroduceAndInitializeInfo> infos)
            : base(infos)
        {
        }

        protected override int GetDeclarationIndex(SyntaxList<MemberDeclarationSyntax> members)
        {
            int index = members.LastIndexOf(SyntaxKind.FieldDeclaration);

            if (index != -1)
                return index + 1;

            index = members.IndexOf(SyntaxKind.ConstructorDeclaration);

            if (index != -1)
                return index;

            return 0;
        }

        protected override string GetTitle()
        {
            if (Infos.Length > 1)
            {
                return $"Introduce and initialize fields {GetNames()}";
            }
            else
            {
                return $"Introduce and initialize field '{FirstInfo.Name}'";
            }
        }
    }
}
