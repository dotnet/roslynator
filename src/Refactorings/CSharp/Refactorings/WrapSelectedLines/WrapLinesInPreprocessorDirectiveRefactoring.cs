// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.WrapSelectedLines
{
    internal sealed class WrapLinesInPreprocessorDirectiveRefactoring : WrapSelectedLinesRefactoring
    {
        private WrapLinesInPreprocessorDirectiveRefactoring()
        {
        }

        public static WrapLinesInPreprocessorDirectiveRefactoring Instance { get; } = new();

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
