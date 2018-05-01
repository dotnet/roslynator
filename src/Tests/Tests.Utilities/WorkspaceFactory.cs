// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    public static class WorkspaceFactory
    {
        public static Project EmptyCSharpProject { get; } = Project(LanguageNames.CSharp);

        public static Document Document(string language, string source, params string[] additionalSources)
        {
            Project project = Project(language, source);

            Document document = project.Documents.First();

            if (additionalSources.Length > 0)
                project = AddDocuments(project, language, additionalSources, fileNumberingBase: 1);

            return project.GetDocument(document.Id);
        }

        public static Project Project(string language, string source)
        {
            Project project = (language == LanguageNames.CSharp) ? EmptyCSharpProject : Project(language);

            ProjectId projectId = project.Id;

            string newFileName = FileUtility.CreateDefaultFileName(language: language);

            DocumentId documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

            return project
                .Solution
                .AddDocument(documentId, newFileName, SourceText.From(source))
                .GetProject(projectId);
        }

        public static Project Project(string language, IEnumerable<string> sources)
        {
            Project project = (language == LanguageNames.CSharp) ? EmptyCSharpProject : Project(language);

            return AddDocuments(project, language, sources);
        }

        private static Project AddDocuments(Project project, string language, IEnumerable<string> sources, int fileNumberingBase = FileUtility.FileNumberingBase)
        {
            Solution solution = project.Solution;

            int count = fileNumberingBase;
            foreach (string source in sources)
            {
                string newFileName = FileUtility.CreateFileName(language, suffix: count);
                DocumentId documentId = DocumentId.CreateNewId(project.Id, debugName: newFileName);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
            }

            return solution.GetProject(project.Id);
        }

        public static Project Project(string language)
        {
            ProjectId projectId = ProjectId.CreateNewId(debugName: FileUtility.TestProjectName);

            Project project = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, FileUtility.TestProjectName, FileUtility.TestProjectName, language)
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

            if (language == LanguageNames.CSharp)
            {
                var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

                var parseOptions = (CSharpParseOptions)project.ParseOptions;

                CSharpCompilationOptions newCompilationOptions = compilationOptions
                    .WithAllowUnsafe(true)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                project = project
                    .WithCompilationOptions(newCompilationOptions)
                    .WithParseOptions(parseOptions.WithLanguageVersion(LanguageVersion.Latest));
            }

            return project;
        }
    }
}
