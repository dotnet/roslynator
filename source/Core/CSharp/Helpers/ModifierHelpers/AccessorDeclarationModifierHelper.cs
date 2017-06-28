// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class AccessorDeclarationModifierHelper : AbstractModifierHelper<AccessorDeclarationSyntax>
    {
        private AccessorDeclarationModifierHelper()
        {
        }

        public static AccessorDeclarationModifierHelper Instance { get; } = new AccessorDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(AccessorDeclarationSyntax node)
        {
            return node.Keyword;
        }

        public override SyntaxTokenList GetModifiers(AccessorDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override AccessorDeclarationSyntax WithModifiers(AccessorDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
