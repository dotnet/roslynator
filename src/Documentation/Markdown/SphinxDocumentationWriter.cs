// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotMarkdown;

namespace Roslynator.Documentation.Markdown;

public class SphinxDocumentationWriter : MarkdownDocumentationWriter
{
    public SphinxDocumentationWriter(DocumentationContext context, MarkdownWriter writer) : base(context, writer)
    {
    }

    internal override bool IncludeLinkInClassHierarchy => false;

    internal override bool IncludeLinkToRoot => false;

    public override void WriteLinkTarget(string name)
    {
        WriteRaw($"({name})=");
    }
}
