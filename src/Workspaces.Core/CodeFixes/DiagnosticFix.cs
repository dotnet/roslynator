// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CodeFixes
{
    internal struct DiagnosticFix
    {
        public DiagnosticFix(CodeAction codeAction, Document document, CodeFixProvider fixProvider, CodeFixProvider fixProvider2)
        {
            CodeAction = codeAction;
            Document = document;
            FixProvider = fixProvider;
            FixProvider2 = fixProvider2;
        }

        public CodeAction CodeAction { get; }

        public Document Document { get; }

        public CodeFixProvider FixProvider { get; }

        public CodeFixProvider FixProvider2 { get; }
    }
}
