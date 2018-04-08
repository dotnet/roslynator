// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.WrapSelectedLines
{
    internal class WrapInIfDirectiveRefactoring : WrapSelectedLinesRefactoring
    {
        private WrapInIfDirectiveRefactoring()
        {
        }

        public static WrapInIfDirectiveRefactoring Instance { get; } = new WrapInIfDirectiveRefactoring();

        public override string GetFirstLineText()
        {
            return "#if DEBUG";
        }

        public override string GetLastLineText()
        {
            return "#endif";
        }
    }
}
