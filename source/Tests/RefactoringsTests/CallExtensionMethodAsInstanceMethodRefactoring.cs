// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class CallExtensionMethodAsInstanceMethodRefactoring
    {
        public static void ExtensionMethod(this string parameter1, string parameter2)
        {
            ExtensionMethod(parameter1, parameter2);
            CallExtensionMethodAsInstanceMethodRefactoring.ExtensionMethod(parameter1, parameter2);
            Roslynator.CSharp.Refactorings.Tests.CallExtensionMethodAsInstanceMethodRefactoring.ExtensionMethod(parameter1, parameter2);
            parameter1.ExtensionMethod(parameter2);
        }
    }
}
