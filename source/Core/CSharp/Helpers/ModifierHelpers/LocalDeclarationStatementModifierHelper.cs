// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class LocalDeclarationStatementModifierHelper : AbstractModifierHelper<LocalDeclarationStatementSyntax>
    {
        private LocalDeclarationStatementModifierHelper()
        {
        }

        public static LocalDeclarationStatementModifierHelper Instance { get; } = new LocalDeclarationStatementModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(LocalDeclarationStatementSyntax node)
        {
            return node.Declaration?.Type;
        }

        public override SyntaxTokenList GetModifiers(LocalDeclarationStatementSyntax node)
        {
            return node.Modifiers;
        }

        public override LocalDeclarationStatementSyntax WithModifiers(LocalDeclarationStatementSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
