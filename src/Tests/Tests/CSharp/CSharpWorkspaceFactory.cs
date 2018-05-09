// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp
{
    internal class CSharpWorkspaceFactory : IWorkspaceFactory
    {
        private Project _emptyProject;

        public static CSharpWorkspaceFactory Instance { get; } = new CSharpWorkspaceFactory();

        public string Language
        {
            get { return LanguageNames.CSharp; }
        }

        public Project EmptyProject
        {
            get { return _emptyProject ?? (_emptyProject = Project()); }
        }

        public Document Document(string source, params string[] additionalSources)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (additionalSources == null)
                throw new ArgumentNullException(nameof(additionalSources));

            Project project = Project(source);

            Document document = project.Documents.First();

            if (additionalSources.Length > 0)
                project = AddDocuments(project, additionalSources, fileNumberingBase: 1);

            return project.GetDocument(document.Id);
        }

        public Project Project(string source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Project project = EmptyProject;

            ProjectId projectId = project.Id;

            string newFileName = FileUtility.CreateDefaultFileName(language: Language);

            DocumentId documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

            return project
                .Solution
                .AddDocument(documentId, newFileName, SourceText.From(source))
                .GetProject(projectId);
        }

        public Project Project(IEnumerable<string> sources)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));

            Project project = EmptyProject;

            return AddDocuments(project, sources);
        }

        public Project Project()
        {
            ProjectId projectId = ProjectId.CreateNewId(debugName: FileUtility.TestProjectName);

            Project project = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, FileUtility.TestProjectName, FileUtility.TestProjectName, Language)
                .AddMetadataReferences(
                    projectId,
                    new MetadataReference[]
                    {
                        RuntimeMetadataReference.CorLibReference,
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Core.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Linq.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Linq.Expressions.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Runtime.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("System.Collections.Immutable.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("Microsoft.CodeAnalysis.dll"),
                        RuntimeMetadataReference.CreateFromAssemblyName("Microsoft.CodeAnalysis.CSharp.dll"),
                    })
                .GetProject(projectId);

            var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

            CSharpCompilationOptions newCompilationOptions = compilationOptions
                .WithAllowUnsafe(true)
                .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

            var parseOptions = (CSharpParseOptions)project.ParseOptions;

            CSharpParseOptions newParseOptions = parseOptions
                .WithLanguageVersion(LanguageVersion.Latest);

            return project
                .WithCompilationOptions(newCompilationOptions)
                .WithParseOptions(newParseOptions);
        }

        private Project AddDocuments(
            Project project,
            IEnumerable<string> sources,
            int fileNumberingBase = FileUtility.FileNumberingBase)
        {
            Solution solution = project.Solution;

            int count = fileNumberingBase;
            foreach (string source in sources)
            {
                string newFileName = FileUtility.CreateFileName(Language, suffix: count);
                DocumentId documentId = DocumentId.CreateNewId(project.Id, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(project.Id);
        }
    }
}
