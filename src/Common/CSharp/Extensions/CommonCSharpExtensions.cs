// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp;

internal static class CommonCSharpExtensions
{
    public static bool SupportsCollectionExpression(this CSharpCompilation compilation)
    {
        return (int)compilation.LanguageVersion >= 1200;
    }
}
