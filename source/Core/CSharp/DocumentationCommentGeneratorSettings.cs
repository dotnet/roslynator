// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    public class DocumentationCommentGeneratorSettings
    {
        public DocumentationCommentGeneratorSettings()
        {
        }

        public DocumentationCommentGeneratorSettings(string indent = null, bool generateReturns = true)
        {
            Indent = indent ?? "";
            GenerateReturns = generateReturns;
        }

        public static DocumentationCommentGeneratorSettings Default { get; } = new DocumentationCommentGeneratorSettings();

        public string Indent { get; }
        public bool GenerateReturns { get; }
    }
}
