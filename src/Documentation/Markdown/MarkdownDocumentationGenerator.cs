// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using DotMarkdown;

namespace Roslynator.Documentation.Markdown
{
    public class MarkdownDocumentationGenerator : DocumentationGenerator
    {
        private readonly MarkdownWriterSettings _writerSettings;

        public MarkdownDocumentationGenerator(
            DocumentationModel documentationModel,
            DocumentationUrlProvider urlProvider,
            MarkdownWriterSettings writerSettings,
            DocumentationOptions options = null,
            SourceReferenceProvider sourceReferenceProvider = null,
            DocumentationResources resources = null) : base(documentationModel, urlProvider: urlProvider, options: options, sourceReferenceProvider: sourceReferenceProvider, resources: resources)
        {
            _writerSettings = writerSettings;
        }

        protected override DocumentationWriter CreateWriterCore()
        {
            return new MarkdownDocumentationWriter(
                DocumentationModel,
                urlProvider: UrlProvider,
                writerSettings: _writerSettings,
                options: Options,
                resources: Resources);
        }
    }
}
