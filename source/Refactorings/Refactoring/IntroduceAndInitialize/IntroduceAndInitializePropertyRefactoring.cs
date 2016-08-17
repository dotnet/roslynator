// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring.IntroduceAndInitialize
{
    internal class IntroduceAndInitializePropertyRefactoring : IntroduceAndInitializeRefactoring
    {
        public IntroduceAndInitializePropertyRefactoring(IEnumerable<ParameterSyntax> parameters)
            : base(parameters)
        {
        }

        protected override int GetDeclarationIndex(SyntaxList<MemberDeclarationSyntax> members)
        {
            int index = members.LastIndexOf(SyntaxKind.PropertyDeclaration);

            if (index == -1)
                index = members.LastIndexOf(SyntaxKind.ConstructorDeclaration);

            return index + 1;
        }

        protected override string GetTitle()
        {
            if (Infos.Length > 1)
            {
                return $"Introduce and initialize properties {GetNames()}";
            }
            else
            {
                return $"Introduce and initialize property '{FirstInfo.Name}'";
            }
        }

        protected override IntroduceAndInitializeInfo CreateInfo(ParameterSyntax parameter)
        {
            return new IntroduceAndInitializePropertyInfo(parameter);
        }
    }
}
