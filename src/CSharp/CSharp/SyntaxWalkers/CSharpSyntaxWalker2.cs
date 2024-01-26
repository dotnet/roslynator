// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp.SyntaxWalkers;

internal class CSharpSyntaxWalker2 : CSharpSyntaxWalker
{
    public override void DefaultVisit(SyntaxNode node)
    {
        if (ShouldVisit)
            base.DefaultVisit(node);
    }

    protected virtual bool ShouldVisit => true;
}
