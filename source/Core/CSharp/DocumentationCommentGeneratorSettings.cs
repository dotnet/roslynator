// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    public class DocumentationCommentGeneratorSettings
    {
        public DocumentationCommentGeneratorSettings(
            string indent = null,
            bool singleLineSummary = false,
            bool skipNamespaceDeclaration = true,
            bool generateReturns = true
            )
        {
            Indent = indent ?? "";
            SingleLineSummary = singleLineSummary;
            SkipNamespaceDeclaration = skipNamespaceDeclaration;
            GenerateReturns = generateReturns;
        }

        public static DocumentationCommentGeneratorSettings Default { get; } = new DocumentationCommentGeneratorSettings();

        public string Indent { get; }
        public bool SingleLineSummary { get; }
        public bool SkipNamespaceDeclaration { get; }
        public bool GenerateReturns { get; }

        public DocumentationCommentGeneratorSettings WithIndent(string indent)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: indent,
                singleLineSummary: SingleLineSummary,
                skipNamespaceDeclaration: SkipNamespaceDeclaration,
                generateReturns: GenerateReturns);
        }

        public DocumentationCommentGeneratorSettings WithSingleLineSummary(bool singleLineSummary)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: Indent,
                singleLineSummary: singleLineSummary,
                skipNamespaceDeclaration: SkipNamespaceDeclaration,
                generateReturns: GenerateReturns);
        }

        public DocumentationCommentGeneratorSettings WithSkipNamespaceDeclaration(bool skipNamespaceDeclaration)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: Indent,
                singleLineSummary: SingleLineSummary,
                skipNamespaceDeclaration: skipNamespaceDeclaration,
                generateReturns: GenerateReturns);
        }

        public DocumentationCommentGeneratorSettings WithGenerateReturns(bool generateReturns)
        {
            return new DocumentationCommentGeneratorSettings(
                indent: Indent,
                singleLineSummary: SingleLineSummary,
                skipNamespaceDeclaration: SkipNamespaceDeclaration,
                generateReturns: generateReturns);
        }
    }
}
