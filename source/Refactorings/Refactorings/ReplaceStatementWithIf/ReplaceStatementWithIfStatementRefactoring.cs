// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.ReplaceStatementWithIf
{
    internal static class ReplaceStatementWithIfStatementRefactoring
    {
        public static ReplaceReturnStatementWithIfStatementRefactoring ReplaceReturnWithIfElse { get; } = new ReplaceReturnStatementWithIfStatementRefactoring();

        public static ReplaceYieldStatementWithIfStatementRefactoring ReplaceYieldReturnWithIfElse { get; } = new ReplaceYieldStatementWithIfStatementRefactoring();
    }
}
