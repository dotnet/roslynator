// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class DelegateDeclarationModifierHelper : AbstractModifierHelper<DelegateDeclarationSyntax>
    {
        private DelegateDeclarationModifierHelper()
        {
        }

        public static DelegateDeclarationModifierHelper Instance { get; } = new DelegateDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(DelegateDeclarationSyntax node)
        {
            return node.DelegateKeyword;
        }

        public override SyntaxTokenList GetModifiers(DelegateDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override DelegateDeclarationSyntax WithModifiers(DelegateDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
