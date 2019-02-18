// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslynator.Documentation
{
    internal class DocumentationCommentGeneratorSettings
    {
        public DocumentationCommentGeneratorSettings(
            IEnumerable<string> summary = null,
            string indentation = null,
            bool singleLineSummary = false,
            bool returns = true)
        {
            Summary = (summary != null) ? ImmutableArray.CreateRange(summary) : ImmutableArray<string>.Empty;
            Indentation = indentation ?? "";
            SingleLineSummary = singleLineSummary;
            Returns = returns;
        }

        public static DocumentationCommentGeneratorSettings Default { get; } = new DocumentationCommentGeneratorSettings();

        public ImmutableArray<string> Summary { get; }

        public string Indentation { get; }

        public bool SingleLineSummary { get; }

        public bool Returns { get; }

        public DocumentationCommentGeneratorSettings WithIndentation(string indentation)
        {
            return new DocumentationCommentGeneratorSettings(
                summary: Summary,
                indentation: indentation,
                singleLineSummary: SingleLineSummary,
                returns: Returns);
        }
    }
}
