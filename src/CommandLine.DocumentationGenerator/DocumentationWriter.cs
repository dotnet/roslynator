// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotMarkdown;

namespace Roslynator.CommandLine.Documentation
{
    internal class DocumentationWriter : MarkdownDocumentationWriter
    {
        public DocumentationWriter(MarkdownWriter writer) : base(writer)
        {
        }

        public override void WriteOptionDescription(CommandOption option)
        {
            _writer.WriteString(option.FullDescription);
        }
    }
}
