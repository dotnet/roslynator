// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    public abstract class BaseDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private static readonly CSharpGeneratedCodeAnalyzer _generatedCodeAnalyzerInstance = new CSharpGeneratedCodeAnalyzer();

        protected BaseDiagnosticAnalyzer()
        {
        }

        public virtual CSharpGeneratedCodeAnalyzer GeneratedCodeAnalyzer
        {
            get { return _generatedCodeAnalyzerInstance; }
        }
    }
}
