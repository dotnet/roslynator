// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    internal readonly struct CompilerDiagnosticFix
    {
        public CompilerDiagnosticFix(string compilerDiagnosticId, string compilerDiagnosticTitle, string codeFixId, string codeFixTitle)
        {
            CompilerDiagnosticId = compilerDiagnosticId;
            CompilerDiagnosticTitle = compilerDiagnosticTitle;
            CodeFixId = codeFixId;
            CodeFixTitle = codeFixTitle;
        }

        public string CompilerDiagnosticId { get; }

        public string CompilerDiagnosticTitle { get; }

        public string CodeFixId { get; }

        public string CodeFixTitle { get; }
    }
}
