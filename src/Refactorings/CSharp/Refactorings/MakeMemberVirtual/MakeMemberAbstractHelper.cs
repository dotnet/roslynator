// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.MakeMemberVirtual
{
    internal static class MakeMemberAbstractHelper
    {
        public static AccessorListSyntax ExpandAccessorList(AccessorListSyntax accessorList)
        {
            IEnumerable<AccessorDeclarationSyntax> accessors = accessorList
                .Accessors
                .Select(f => f.WithBody(Block()).WithSemicolonToken(default(SyntaxToken)));

            AccessorListSyntax newAccessorList = AccessorList(List(accessors));

            return newAccessorList
                .RemoveWhitespace()
                .WithCloseBraceToken(newAccessorList.CloseBraceToken.WithLeadingTrivia(NewLine()));
        }
    }
}