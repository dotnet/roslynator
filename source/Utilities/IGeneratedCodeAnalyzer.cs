// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator
{
    public interface IGeneratedCodeAnalyzer
    {
        bool IsGeneratedCode(SyntaxTreeAnalysisContext context);
        bool IsGeneratedCode(SyntaxNodeAnalysisContext context);
        bool IsGeneratedCode(SymbolAnalysisContext context);
        bool IsGeneratedCode(CodeBlockAnalysisContext context);
    }
}
