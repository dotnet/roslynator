// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class DestructorDeclarationModifierHelper : AbstractModifierHelper<DestructorDeclarationSyntax>
    {
        private DestructorDeclarationModifierHelper()
        {
        }

        public static DestructorDeclarationModifierHelper Instance { get; } = new DestructorDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(DestructorDeclarationSyntax node)
        {
            return node.TildeToken;
        }

        public override SyntaxTokenList GetModifiers(DestructorDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override DestructorDeclarationSyntax WithModifiers(DestructorDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
