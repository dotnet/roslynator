// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class ClassDeclarationModifierHelper : AbstractModifierHelper<ClassDeclarationSyntax>
    {
        private ClassDeclarationModifierHelper()
        {
        }

        public static ClassDeclarationModifierHelper Instance { get; } = new ClassDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(ClassDeclarationSyntax node)
        {
            return node.Keyword;
        }

        public override SyntaxTokenList GetModifiers(ClassDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override ClassDeclarationSyntax WithModifiers(ClassDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
