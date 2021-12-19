// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.ConvertReturnToIf
{
    internal static class ConvertReturnStatementToIfRefactoring
    {
        public static ConvertReturnToIfElseRefactoring ConvertReturnToIfElse { get; } = new();

        public static ConvertYieldReturnToIfElseRefactoring ConvertYieldReturnToIfElse { get; } = new();
    }
}
