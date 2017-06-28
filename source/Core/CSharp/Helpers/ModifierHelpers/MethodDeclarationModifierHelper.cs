// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class MethodDeclarationModifierHelper : AbstractModifierHelper<MethodDeclarationSyntax>
    {
        private MethodDeclarationModifierHelper()
        {
        }

        public static MethodDeclarationModifierHelper Instance { get; } = new MethodDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(MethodDeclarationSyntax node)
        {
            return node.ReturnType;
        }

        public override SyntaxTokenList GetModifiers(MethodDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override MethodDeclarationSyntax WithModifiers(MethodDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
