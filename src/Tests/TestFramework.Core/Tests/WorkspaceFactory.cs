// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.RuntimeMetadataReference;

namespace Roslynator.Tests
{
    public abstract class WorkspaceFactory
    {
        public abstract string Language { get; }

        public abstract string DefaultDocumentName { get; }

        public virtual string DefaultProjectName => "TestProject";

        public virtual Project AddProject(Solution solution, CodeVerificationOptions options = null)
        {
            return solution
                .AddProject(DefaultProjectName, DefaultProjectName, Language)
                .WithMetadataReferences(DefaultProjectReferences);
        }

        public Document AddDocument(Project project, string source, params string[] additionalSources)
        {
            Document document = project.AddDocument(DefaultDocumentName, SourceText.From(source));

            int length = additionalSources.Length;

            if (length > 0)
            {
                project = document.Project;

                for (int i = 0; i < length; i++)
                {
                    project = project
                        .AddDocument(PathHelpers.AppendNumberToFileName(document.Name, i + 2), SourceText.From(additionalSources[i]))
                        .Project;
                }

                document = project.GetDocument(document.Id);
            }

            return document;
        }

        internal ImmutableArray<ExpectedDocument> AddAdditionalDocuments(
            IEnumerable<(string source, string expected)> additionalData,
            ref Project project)
        {
            ImmutableArray<ExpectedDocument>.Builder expectedDocuments = ImmutableArray.CreateBuilder<ExpectedDocument>();

            int i = 2;
            foreach ((string source, string expected) in additionalData)
            {
                Document document = project.AddDocument(PathHelpers.AppendNumberToFileName(DefaultDocumentName, i), SourceText.From(source));
                expectedDocuments.Add(new ExpectedDocument(document.Id, expected));
                project = document.Project;

                i++;
            }

            return expectedDocuments.ToImmutableArray();
        }
    }
}
