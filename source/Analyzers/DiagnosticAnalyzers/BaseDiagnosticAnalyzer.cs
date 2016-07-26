// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    public abstract class BaseDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private static readonly IGeneratedCodeAnalyzer _generatedCodeAnalyzerInstance = new GeneratedCodeAnalyzer();

        protected BaseDiagnosticAnalyzer()
        {
        }

        public virtual IGeneratedCodeAnalyzer GeneratedCodeAnalyzer
            => _generatedCodeAnalyzerInstance;
    }
}
