// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.CSharp.Documentation
{
    public class DocumentationCommentGeneratorSettings
    {
        public DocumentationCommentGeneratorSettings(
            IEnumerable<string> comments = null,
            string indent = null,
            bool singleLineSummary = false,
            bool generateReturns = true
            )
        {
            Comments = (comments != null) ? ImmutableArray.CreateRange(comments) : ImmutableArray<string>.Empty;
            Indent = indent ?? "";
            SingleLineSummary = singleLineSummary;
            GenerateReturns = generateReturns;
        }

        public static DocumentationCommentGeneratorSettings Default { get; } = new DocumentationCommentGeneratorSettings();

        public ImmutableArray<string> Comments { get; }
        public string Indent { get; }
        public bool SingleLineSummary { get; }
        public bool GenerateReturns { get; }

        public DocumentationCommentGeneratorSettings WithComments(IEnumerable<string> comments)
        {
            return new DocumentationCommentGeneratorSettings(
                comments: comments,
                indent: Indent,
                singleLineSummary: SingleLineSummary,
                generateReturns: GenerateReturns);
        }

        public DocumentationCommentGeneratorSettings WithIndent(string indent)
        {
            return new DocumentationCommentGeneratorSettings(
                comments: Comments,
                indent: indent,
                singleLineSummary: SingleLineSummary,
                generateReturns: GenerateReturns);
        }

        public DocumentationCommentGeneratorSettings WithSingleLineSummary(bool singleLineSummary)
        {
            return new DocumentationCommentGeneratorSettings(
                comments: Comments,
                indent: Indent,
                singleLineSummary: singleLineSummary,
                generateReturns: GenerateReturns);
        }

        public DocumentationCommentGeneratorSettings WithGenerateReturns(bool generateReturns)
        {
            return new DocumentationCommentGeneratorSettings(
                comments: Comments,
                indent: Indent,
                singleLineSummary: SingleLineSummary,
                generateReturns: generateReturns);
        }
    }
}
