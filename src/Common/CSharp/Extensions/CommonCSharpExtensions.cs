// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Roslynator.CSharp;

internal static class CommonCSharpExtensions
{
#pragma warning disable IDE0060, RCS1175
    public static bool SupportsCollectionExpression(this CSharpCompilation compilation)
#pragma warning restore IDE0060, RCS1175
    {
        Version version = typeof(SyntaxNode).Assembly.GetName().Version;

        return version.Major > 4
            || (version.Major == 4 && version.Minor >= 8);

        //TODO: bump Roslyn to 4.8.0
        //return compilation.LanguageVersion >= LanguageVersion.CSharp12;
    }
}
