// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.CodeAnalysis.CodeActions;
using Xunit;

namespace Roslynator.Testing.CSharp;

public static class CodeActionTests
{
    // https://github.com/dotnet/roslyn/issues/71050
    [Fact]
    public static void IsPropertyNestedCodeActionPublic()
    {
        PropertyInfo propertyInfo = typeof(CodeAction).GetProperty("NestedCodeActions", BindingFlags.Public | BindingFlags.Instance);

        Assert.Null(propertyInfo);
    }
}
