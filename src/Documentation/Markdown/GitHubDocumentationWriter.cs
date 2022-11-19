// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotMarkdown;

namespace Roslynator.Documentation.Markdown;

public class GitHubDocumentationWriter : MarkdownDocumentationWriter
{
    public GitHubDocumentationWriter(DocumentationContext context, MarkdownWriter writer) : base(context, writer)
    {
    }

    internal override bool IncludeLinkInClassHierarchy => true;

    internal override bool IncludeLinkToRoot => true;
}
