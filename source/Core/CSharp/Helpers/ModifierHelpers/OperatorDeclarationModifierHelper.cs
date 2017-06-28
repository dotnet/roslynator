// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class OperatorDeclarationModifierHelper : AbstractModifierHelper<OperatorDeclarationSyntax>
    {
        private OperatorDeclarationModifierHelper()
        {
        }

        public static OperatorDeclarationModifierHelper Instance { get; } = new OperatorDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(OperatorDeclarationSyntax node)
        {
            return node.ReturnType;
        }

        public override SyntaxTokenList GetModifiers(OperatorDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override OperatorDeclarationSyntax WithModifiers(OperatorDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
