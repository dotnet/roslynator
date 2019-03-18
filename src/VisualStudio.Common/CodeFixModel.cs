// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.VisualStudio
{
    public class CodeFixModel : BaseModel
    {
        private string _id;

        public CodeFixModel(string compilerDiagnosticId, string compilerDiagnosticTitle, string codeFixId, string codeFixTitle, bool enabled = false)
            : base(compilerDiagnosticId, codeFixTitle, enabled)
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

        public override string Id => _id ?? (_id = $"{CompilerDiagnosticId}.{CodeFixId}");

        public override string NameToolTip => CompilerDiagnosticTitle;
    }
}
