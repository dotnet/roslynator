// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Documentation
{
    public class DocumentationContext
    {
        private readonly Func<DocumentationContext, DocumentationWriter> _createWriter;

        public DocumentationContext(
            DocumentationModel documentationModel,
            DocumentationUrlProvider urlProvider,
            Func<DocumentationContext, DocumentationWriter> createWriter,
            DocumentationOptions options = null,
            DocumentationResources resources = null,
            SourceReferenceProvider sourceReferenceProvider = null)
        {
            DocumentationModel = documentationModel;
            UrlProvider = urlProvider;
            Options = options ?? DocumentationOptions.Default;
            Resources = resources ?? DocumentationResources.Default;
            SourceReferenceProvider = sourceReferenceProvider;

            _createWriter = createWriter;
        }

        public DocumentationModel DocumentationModel { get; }

        public DocumentationUrlProvider UrlProvider { get; }

        public DocumentationOptions Options { get; }

        public DocumentationResources Resources { get; }

        public SourceReferenceProvider SourceReferenceProvider { get; }

        public DocumentationWriter CreateWriter()
        {
            return _createWriter(this);
        }
    }
}
