// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.Configuration;

namespace Roslynator.CodeGeneration
{
    internal static class CodeGenerationExtensions
    {
        public static void WriteCommentCharIf(this EditorConfigWriter writer, bool condition)
        {
            if (condition)
                writer.WriteCommentChar();
        }
    }
}
