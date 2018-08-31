// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Tests.Text;

namespace Roslynator.Tests
{
    public abstract class CodeVerifier
    {
        public abstract CodeVerificationOptions Options { get; }

        public abstract string Language { get; }

        protected virtual TextSpanParser SpanParser { get; } = TextSpanParser.Default;

        protected abstract Project CreateProject();

        protected abstract string CreateFileName(int index = 0);

        protected virtual Document CreateDocument(string source, params string[] additionalSources)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (additionalSources == null)
                throw new ArgumentNullException(nameof(additionalSources));

            Document document = CreateProject().AddDocument(CreateFileName(), SourceText.From(source));

            Project project = document.Project;

            int length = additionalSources.Length;

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                    project = project.AddDocument(CreateFileName(i + 1), SourceText.From(additionalSources[i])).Project;
            }

            return project.GetDocument(document.Id);
        }
    }
}
