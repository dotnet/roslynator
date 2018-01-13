// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp.Helpers;

namespace Roslynator.CSharp.Syntax
{
    public struct AccessibilityInfo
    {
        private static AccessibilityInfo Default { get; } = new AccessibilityInfo();

        private AccessibilityInfo(SyntaxNode node, SyntaxTokenList modifiers, int index, int secondIndex = -1)
        {
            Node = node;
            Modifiers = modifiers;
            TokenIndex = index;
            SecondTokenIndex = secondIndex;
        }

        public SyntaxNode Node { get; }

        public SyntaxToken Token
        {
            get { return GetTokenOrDefault(TokenIndex); }
        }

        public SyntaxToken SecondToken
        {
            get { return GetTokenOrDefault(SecondTokenIndex); }
        }

        public SyntaxTokenList Modifiers { get; }

        public int TokenIndex { get; }

        public int SecondTokenIndex { get; }

        public bool Success
        {
            get { return Node != null; }
        }

        public Accessibility Accessibility
        {
            get
            {
                if (TokenIndex == -1)
                    return Accessibility.NotApplicable;

                switch (Token.Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        {
                            return Accessibility.Public;
                        }
                    case SyntaxKind.PrivateKeyword:
                        {
                            return (SecondTokenIndex == -1)
                                ? Accessibility.Private
                                : Accessibility.ProtectedAndInternal;
                        }
                    case SyntaxKind.InternalKeyword:
                        {
                            return (SecondTokenIndex == -1)
                                ? Accessibility.Internal
                                : Accessibility.ProtectedOrInternal;
                        }
                    case SyntaxKind.ProtectedKeyword:
                        {
                            if (SecondTokenIndex == -1)
                                return Accessibility.Protected;

                            switch (SecondToken.Kind())
                            {
                                case SyntaxKind.PrivateKeyword:
                                    return Accessibility.ProtectedAndInternal;
                                case SyntaxKind.InternalKeyword:
                                    return Accessibility.ProtectedOrInternal;
                            }

                            break;
                        }
                }

                Debug.Fail(Modifiers.ToString());

                return Accessibility.NotApplicable;
            }
        }

        public bool CanHaveAccessibility
        {
            get { return Node?.Kind().CanHaveAccessibility() == true; }
        }

        internal static AccessibilityInfo Create(SyntaxNode node)
        {
            if (node == null)
                return Default;

            return Create(node, node.GetModifiers());
        }

        private static AccessibilityInfo Create(SyntaxNode node, SyntaxTokenList modifiers)
        {
            if (node == null)
                return Default;

            int count = modifiers.Count;

            for (int i = 0; i < count; i++)
            {
                switch (modifiers[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                        {
                            return new AccessibilityInfo(node, modifiers, i);
                        }
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.InternalKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (modifiers[j].IsKind(SyntaxKind.ProtectedKeyword))
                                    return new AccessibilityInfo(node, modifiers, i, j);
                            }

                            return new AccessibilityInfo(node, modifiers, i);
                        }
                    case SyntaxKind.ProtectedKeyword:
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (modifiers[j].IsKind(SyntaxKind.InternalKeyword, SyntaxKind.PrivateKeyword))
                                    return new AccessibilityInfo(node, modifiers, i, j);
                            }

                            return new AccessibilityInfo(node, modifiers, i);
                        }
                }
            }

            return new AccessibilityInfo(node, modifiers, -1);
        }

        public AccessibilityInfo WithModifiers(SyntaxTokenList newModifiers)
        {
            SyntaxNode newNode = Node.WithModifiers(newModifiers);

            return Create(newNode, newModifiers);
        }

        public AccessibilityInfo WithAccessibility(Accessibility newAccessibility, IModifierComparer comparer = null)
        {
            return ChangeAccessibilityHelper.ChangeAccessibility(this, newAccessibility, comparer);
        }

        private SyntaxToken GetTokenOrDefault(int index)
        {
            return (index == -1) ? default(SyntaxToken) : Modifiers[index];
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }
    }
}
