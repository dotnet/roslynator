// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    public class DocumentationCommentGeneratorSettings
    {
        public DocumentationCommentGeneratorSettings(
            string indent = null,
            bool generateReturns = true,
            bool skipNamespaceDeclaration = true)
        {
            Indent = indent ?? "";
            GenerateReturns = generateReturns;
            SkipNamespaceDeclaration = skipNamespaceDeclaration;
        }

        public static DocumentationCommentGeneratorSettings Default { get; } = new DocumentationCommentGeneratorSettings();

        public string Indent { get; }
        public bool GenerateReturns { get; }
        public bool SkipNamespaceDeclaration { get; }

        public DocumentationCommentGeneratorSettings WithIndent(string indent)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: indent,
                generateReturns: GenerateReturns,
                skipNamespaceDeclaration: SkipNamespaceDeclaration);

        }

        public DocumentationCommentGeneratorSettings WithGenerateReturns(bool generateReturns)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: Indent,
                generateReturns: generateReturns,
                skipNamespaceDeclaration: SkipNamespaceDeclaration);
        }

        public DocumentationCommentGeneratorSettings WithSkipNamespaceDeclaration(bool skipNamespaceDeclaration)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: Indent,
                generateReturns: GenerateReturns,
                skipNamespaceDeclaration: skipNamespaceDeclaration);
        }
    }
}
