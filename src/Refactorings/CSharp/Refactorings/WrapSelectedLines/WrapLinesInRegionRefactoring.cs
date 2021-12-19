// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.WrapSelectedLines
{
    internal sealed class WrapLinesInRegionRefactoring : WrapSelectedLinesRefactoring
    {
        private WrapLinesInRegionRefactoring()
        {
        }

        public static WrapLinesInRegionRefactoring Instance { get; } = new();

        public override bool Indent
        {
            get { return true; }
        }

        public override string GetFirstLineText()
        {
            return "#region";
        }

        public override string GetLastLineText()
        {
            return "#endregion";
        }
    }
}
