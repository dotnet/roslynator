// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine
{
    internal struct AnalyzerAssemblyInfo
    {
        public AnalyzerAssemblyInfo(AnalyzerAssembly analyzerAssembly, string filePath)
        {
            AnalyzerAssembly = analyzerAssembly;
            FilePath = filePath;
        }

        public AnalyzerAssembly AnalyzerAssembly { get; }

        public string FilePath { get; }
    }
}
