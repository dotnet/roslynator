// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Testing
{
    internal abstract class WorkspaceFactory
    {
        public abstract string Language { get; }

        public abstract string DefaultDocumentName { get; }

        public virtual string DefaultProjectName => "TestProject";

        public Document CreateDocument(Solution solution, string source, CodeVerificationOptions options)
        {
            return CreateDocument(solution, source, additionalSource: null, options);
        }

        public Document CreateDocument(Solution solution, string source, IEnumerable<string> additionalSource, CodeVerificationOptions options)
        {
            Project project = AddProject(solution, options);

            return AddDocument(project, source, additionalSource);
        }

        public virtual Project AddProject(Solution solution, CodeVerificationOptions options)
        {
            return solution
                .AddProject(DefaultProjectName, DefaultProjectName, Language)
                .WithMetadataReferences(options.MetadataReferences);
        }

        public Document AddDocument(Project project, string source, IEnumerable<string> additionalSources = null)
        {
            Document document = project.AddDocument(DefaultDocumentName, SourceText.From(source));

            if (additionalSources != null)
            {
                using (IEnumerator<string> en = additionalSources.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        int i = 2;
                        project = document.Project;

                        do
                        {
                            project = project
                                .AddDocument(AppendNumberToFileName(document.Name, i), SourceText.From(en.Current))
                                .Project;

                        } while (en.MoveNext());

                        document = project.GetDocument(document.Id);
                        i++;
                    }
                }
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
                Document document = project.AddDocument(AppendNumberToFileName(DefaultDocumentName, i), SourceText.From(source));
                expectedDocuments.Add(new ExpectedDocument(document.Id, expected));
                project = document.Project;

                i++;
            }

            return expectedDocuments.ToImmutableArray();
        }

        private static string AppendNumberToFileName(string fileName, int number)
        {
            int index = fileName.LastIndexOf(".");

            return fileName.Insert(index, (number).ToString(CultureInfo.InvariantCulture));
        }
    }
}
