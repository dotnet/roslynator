// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotMarkdown;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation.Markdown
{
    public class DocusaurusDocumentationWriter : MarkdownDocumentationWriter
    {
        public DocusaurusDocumentationWriter(DocumentationContext context, MarkdownWriter writer) : base(context, writer)
        {
        }

        internal override bool IncludeLinkInClassHierarchy => true;

        internal override bool IncludeLinkToRoot => false;

        public override void WriteStartDocument(ISymbol symbol, DocumentationFileKind fileKind)
        {
            string label = null;

            if (symbol != null)
                label = DocumentationUtility.GetSymbolLabel(symbol, Context);

            if (fileKind == DocumentationFileKind.Root)
                label = Context.Options.RootFileHeading;

            WriteRaw("---");
            WriteLine();

            if (fileKind == DocumentationFileKind.Root)
            {
                WriteRaw("sidebar_position: 0");
                WriteLine();
            }

            if (label != null)
            {
                WriteRaw("sidebar_label: ");
                WriteRaw(label);
                WriteLine();
            }

            WriteRaw("---");
            WriteLine();
            WriteLine();
        }
    }
}
