// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Documentation.Markdown
{
    public class MarkdownDocumentationGenerator : DocumentationGenerator
    {
        public MarkdownDocumentationGenerator(
            DocumentationModel documentationModel,
            DocumentationUrlProvider urlProvider,
            DocumentationOptions options = null,
            SourceReferenceProvider sourceReferenceProvider = null,
            DocumentationResources resources = null) : base(documentationModel, urlProvider: urlProvider, options: options, sourceReferenceProvider: sourceReferenceProvider, resources: resources)
        {
        }

        protected override DocumentationWriter CreateWriterCore()
        {
            return new MarkdownDocumentationWriter(
                DocumentationModel,
                urlProvider: UrlProvider,
                options: Options,
                resources: Resources);
        }
    }
}
