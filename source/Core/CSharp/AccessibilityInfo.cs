// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp
{
    internal struct AccessibilityInfo
    {
        private AccessibilityInfo(SyntaxTokenList modifiers, int index)
            : this(modifiers, index, -1)
        {
        }

        private AccessibilityInfo(SyntaxTokenList modifiers, int index, int additionalIndex)
        {
            Modifiers = modifiers;
            Index = index;
            AdditionalIndex = additionalIndex;
        }

        public static AccessibilityInfo Create(SyntaxTokenList modifiers)
        {
            int count = modifiers.Count;

            for (int i = 0; i < count; i++)
            {
                switch (modifiers[i].Kind())
                {
                    case SyntaxKind.PublicKeyword:
                    case SyntaxKind.PrivateKeyword:
                        return new AccessibilityInfo(modifiers, i);
                    case SyntaxKind.InternalKeyword:
                        return Create(modifiers, count, i, SyntaxKind.ProtectedKeyword);
                    case SyntaxKind.ProtectedKeyword:
                        return Create(modifiers, count, i, SyntaxKind.InternalKeyword);
                }
            }

            return new AccessibilityInfo(modifiers, -1);
        }

        private static AccessibilityInfo Create(SyntaxTokenList modifiers, int count, int i, SyntaxKind kind)
        {
            for (int j = i + 1; j < count; j++)
            {
                if (modifiers[j].Kind() == kind)
                    return new AccessibilityInfo(modifiers, i, j);
            }

            return new AccessibilityInfo(modifiers, i);
        }

        public SyntaxTokenList Modifiers { get; }

        public int Index { get; }

        public int AdditionalIndex { get; }

        public Accessibility Accessibility
        {
            get
            {
                if (Index == -1)
                    return Accessibility.NotApplicable;

                if (AdditionalIndex == -1)
                {
                    switch (Token.Kind())
                    {
                        case SyntaxKind.PublicKeyword:
                            return Accessibility.Public;
                        case SyntaxKind.PrivateKeyword:
                            return Accessibility.Private;
                        case SyntaxKind.InternalKeyword:
                            return Accessibility.Internal;
                        case SyntaxKind.ProtectedKeyword:
                            return Accessibility.Protected;
                    }
                }

                return Accessibility.ProtectedOrInternal;
            }
        }

        public SyntaxToken Token
        {
            get { return GetTokenOrDefault(Index); }
        }

        public SyntaxToken AdditionalToken
        {
            get { return GetTokenOrDefault(AdditionalIndex); }
        }

        private SyntaxToken GetTokenOrDefault(int index)
        {
            return (index == -1) ? default(SyntaxToken) : Modifiers[index];
        }
    }
}
