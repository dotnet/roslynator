// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotMarkdown;

namespace Roslynator.Documentation.Markdown
{
    public class MarkdownDocumentationGenerator : DocumentationGenerator
    {
        private readonly MarkdownWriter _writer;

        public MarkdownDocumentationGenerator(
            DocumentationModel documentationModel,
            DocumentationUrlProvider urlProvider,
            MarkdownWriter writer,
            DocumentationOptions options = null,
            SourceReferenceProvider sourceReferenceProvider = null,
            DocumentationResources resources = null) : base(documentationModel, urlProvider: urlProvider, options: options, sourceReferenceProvider: sourceReferenceProvider, resources: resources)
        {
            _writer = writer;
        }

        protected override DocumentationWriter CreateWriterCore()
        {
            return new MarkdownDocumentationWriter(
                DocumentationModel,
                urlProvider: UrlProvider,
                writer: _writer,
                options: Options,
                resources: Resources);
        }
    }
}
