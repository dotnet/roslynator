// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Helpers.ModifierHelpers
{
    internal class EventFieldDeclarationModifierHelper : AbstractModifierHelper<EventFieldDeclarationSyntax>
    {
        private EventFieldDeclarationModifierHelper()
        {
        }

        public static EventFieldDeclarationModifierHelper Instance { get; } = new EventFieldDeclarationModifierHelper();

        public override SyntaxNodeOrToken FindNodeOrTokenAfterModifiers(EventFieldDeclarationSyntax node)
        {
            return node.EventKeyword;
        }

        public override SyntaxTokenList GetModifiers(EventFieldDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        public override EventFieldDeclarationSyntax WithModifiers(EventFieldDeclarationSyntax node, SyntaxTokenList modifiers)
        {
            return node.WithModifiers(modifiers);
        }
    }
}
