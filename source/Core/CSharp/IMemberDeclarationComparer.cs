// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    public interface IMemberDeclarationComparer : IComparer<MemberDeclarationSyntax>
    {
        int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax member);

        int GetInsertIndex(SyntaxList<MemberDeclarationSyntax> members, SyntaxKind memberKind);

        int GetFieldInsertIndex(SyntaxList<MemberDeclarationSyntax> members, bool isConst);
    }
}
