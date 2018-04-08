// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

namespace Roslynator.CodeFixes
{
    internal static class CodeFixContextExtensions
    {
        public static Task<SyntaxNode> GetSyntaxRootAsync(this CodeFixContext context)
        {
            return context.Document.GetSyntaxRootAsync(context.CancellationToken);
        }

        public static Task<SemanticModel> GetSemanticModelAsync(this CodeFixContext context)
        {
            return context.Document.GetSemanticModelAsync(context.CancellationToken);
        }

        public static Project Project(this CodeFixContext context)
        {
            return context.Document.Project;
        }

        public static Solution Solution(this CodeFixContext context)
        {
            return context.Document.Solution();
        }
    }
}
