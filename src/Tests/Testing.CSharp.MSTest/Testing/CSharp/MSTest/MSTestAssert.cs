// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Testing.CSharp.MSTest;

internal class MSTestAssert : IAssert
{
    public static MSTestAssert Instance { get; } = new();

    public void Equal(string expected, string actual)
    {
        global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
    }

    public void True(bool condition, string userMessage)
    {
        global::Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(condition, userMessage);
    }
}
