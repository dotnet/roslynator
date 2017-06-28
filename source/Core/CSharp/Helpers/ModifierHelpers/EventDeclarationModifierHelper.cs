// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class EventDeclarationModifierHelper : AbstractModifierHelper<EventDeclarationSyntax>
    {
        private EventDeclarationModifierHelper()
        {
        }

        public static EventDeclarationModifierHelper Instance { get; } = new EventDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(EventDeclarationSyntax node)
        {
            return node.EventKeyword;
        }

        public override SyntaxTokenList GetModifiers(EventDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override EventDeclarationSyntax WithModifiers(EventDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
