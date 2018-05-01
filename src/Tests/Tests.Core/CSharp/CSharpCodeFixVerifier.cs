// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Tests.CSharp
{
    public abstract class CSharpCodeFixVerifier : CodeFixVerifier
    {
        public override string Language
        {
            get { return LanguageNames.CSharp; }
        }
    }
}
