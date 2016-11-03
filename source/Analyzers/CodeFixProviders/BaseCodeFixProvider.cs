// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CSharp.CodeFixProviders
{
    public abstract class BaseCodeFixProvider : CodeFixProvider
    {
        public const string EquivalenceKeySuffix = "CodeFixProvider";

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;
    }
}
