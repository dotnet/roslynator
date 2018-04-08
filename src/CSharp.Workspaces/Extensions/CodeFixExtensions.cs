// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator
{
    internal static class CodeFixExtensions
    {
        public static void RegisterCodeFix(
            this CodeFixContext context,
            string title,
            Func<CancellationToken, Task<Document>> createChangedDocument,
            string equivalenceKey,
            Diagnostic diagnostic)
        {
            context.RegisterCodeFix(CodeAction.Create(title, createChangedDocument, equivalenceKey), diagnostic);
        }

        public static void RegisterCodeFix(
            this CodeFixContext context,
            string title,
            Func<CancellationToken, Task<Solution>> createChangedSolution,
            string equivalenceKey,
            Diagnostic diagnostic)
        {
            context.RegisterCodeFix(CodeAction.Create(title, createChangedSolution, equivalenceKey), diagnostic);
        }
    }
}
