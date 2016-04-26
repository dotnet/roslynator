// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AttributeListSyntaxExtensions
    {
        public static IEnumerable<SyntaxToken> CommaTokens(this AttributeListSyntax attributeList)
        {
            if (attributeList == null)
                throw new ArgumentNullException(nameof(attributeList));

            foreach (SyntaxToken syntaxToken in attributeList.ChildTokens())
            {
                if (syntaxToken.IsKind(SyntaxKind.CommaToken))
                    yield return syntaxToken;
            }
        }
    }
}
