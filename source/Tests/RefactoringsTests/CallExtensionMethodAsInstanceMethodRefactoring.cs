// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class CallExtensionMethodAsInstanceMethodRefactoring
    {
        public static void ExtensionMethod(this string value, string value2)
        {
            CallExtensionMethodAsInstanceMethodRefactoring.ExtensionMethod(value, value2);
            Roslynator.CSharp.Refactorings.Tests.CallExtensionMethodAsInstanceMethodRefactoring.ExtensionMethod(value, value2);
            value.ExtensionMethod(value2);
        }
    }
}
