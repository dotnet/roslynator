// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    internal static class CodeRefactoringContextExtensions
    {
        public static void RegisterRefactoring(
            this CodeRefactoringContext context,
            string title,
            Func<CancellationToken, Task<Document>> createChangedDocument,
            string equivalenceKey = null)
        {
            CodeAction codeAction = CodeAction.Create(title, createChangedDocument, equivalenceKey);

            context.RegisterRefactoring(codeAction);
        }

        public static void RegisterRefactoring(
            this CodeRefactoringContext context,
            string title,
            Func<CancellationToken, Task<Solution>> createChangedSolution,
            string equivalenceKey = null)
        {
            CodeAction codeAction = CodeAction.Create(title, createChangedSolution, equivalenceKey);

            context.RegisterRefactoring(codeAction);
        }
    }
}
