// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Markdown;

namespace Roslynator.CodeGeneration.Markdown
{
    internal static class MarkdownBuilderExtensions
    {
        public static void AppendCSharpCodeBlock(this MarkdownBuilder mb, string code)
        {
            mb.AppendCodeBlock(code, LanguageIdentifiers.CSharp);
        }
    }
}
