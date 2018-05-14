// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Roslynator.Tests
{
    public abstract class CodeVerifier
    {
        public virtual CodeVerificationOptions Options
        {
            get { return CodeVerificationOptions.Default; }
        }

        public abstract string Language { get; }

        protected virtual Document CreateDocument(string source, params string[] additionalSources)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (additionalSources == null)
                throw new ArgumentNullException(nameof(additionalSources));

            Project project = CreateProject();

            ProjectId projectId = project.Id;

            string newFileName = FileUtility.CreateDefaultFileName(language: Language);

            DocumentId documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);

            project = project
                .Solution
                .AddDocument(documentId, newFileName, SourceText.From(source))
                .GetProject(projectId);

            Document document = project.Documents.First();

            if (additionalSources.Length > 0)
            {
                Solution solution = project.Solution;

                int count = 1;
                foreach (string additionalSource in additionalSources)
                {
                    newFileName = FileUtility.CreateFileName(Language, suffix: count);
                    documentId = DocumentId.CreateNewId(project.Id, debugName: newFileName);
                    solution = solution.AddDocument(documentId, newFileName, SourceText.From(additionalSource));
                    count++;
                }

                project = solution.GetProject(project.Id);
            }

            return project.GetDocument(document.Id);
        }

        protected virtual Project CreateProject()
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

            if (Language == LanguageNames.CSharp)
            {
                var compilationOptions = (CSharpCompilationOptions)project.CompilationOptions;

                CSharpCompilationOptions newCompilationOptions = compilationOptions
                    .WithAllowUnsafe(true)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                var parseOptions = (CSharpParseOptions)project.ParseOptions;

                CSharpParseOptions newParseOptions = parseOptions
                    .WithLanguageVersion(LanguageVersion.Latest);

                project = project
                    .WithCompilationOptions(newCompilationOptions)
                    .WithParseOptions(newParseOptions);
            }

            return project;
        }

        internal void VerifyCompilerDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            DiagnosticSeverity maxAllowedSeverity = Options.MaxAllowedCompilerDiagnosticSeverity;

            ImmutableArray<string> allowedDiagnosticIds = Options.AllowedCompilerDiagnosticIds;

            if (IsAny())
            {
                IEnumerable<Diagnostic> notAllowed = diagnostics
                    .Where(f => f.Severity > maxAllowedSeverity && !allowedDiagnosticIds.Any(id => id == f.Id));

                Assert.True(false, $"No compiler diagnostics with severity higher than '{maxAllowedSeverity}' expected{notAllowed.ToDebugString()}");
            }

            bool IsAny()
            {
                foreach (Diagnostic diagnostic in diagnostics)
                {
                    if (diagnostic.Severity <= maxAllowedSeverity)
                        continue;

                    bool isAllowed = false;
                    foreach (string diagnosticId in allowedDiagnosticIds)
                    {
                        if (diagnostic.Id == diagnosticId)
                        {
                            isAllowed = true;
                            break;
                        }
                    }

                    if (!isAllowed)
                        return true;
                }

                return false;
            }
        }

        internal void VerifyNoNewCompilerDiagnostics(
            ImmutableArray<Diagnostic> diagnostics,
            ImmutableArray<Diagnostic> newDiagnostics)
        {
            ImmutableArray<string> allowedDiagnosticIds = Options.AllowedCompilerDiagnosticIds;

            if (allowedDiagnosticIds.IsDefault)
                allowedDiagnosticIds = ImmutableArray<string>.Empty;

            if (IsAnyNewCompilerDiagnostic())
            {
                IEnumerable<Diagnostic> diff = newDiagnostics
                    .Where(diagnostic => !allowedDiagnosticIds.Any(id => id == diagnostic.Id))
                    .Except(diagnostics, DiagnosticDeepEqualityComparer.Instance);

                Assert.True(false, $"Code fix introduced new compiler diagnostics.{diff.ToDebugString()}");
            }

            bool IsAnyNewCompilerDiagnostic()
            {
                foreach (Diagnostic newDiagnostic in newDiagnostics)
                {
                    bool isAllowed = false;
                    foreach (string diagnosticId in allowedDiagnosticIds)
                    {
                        if (newDiagnostic.Id == diagnosticId)
                        {
                            isAllowed = true;
                            break;
                        }
                    }

                    if (isAllowed)
                        continue;

                    bool isNew = true;
                    foreach (Diagnostic diagnostic in diagnostics)
                    {
                        if (DiagnosticDeepEqualityComparer.Instance.Equals(newDiagnostic, diagnostic))
                        {
                            isNew = false;
                            break;
                        }
                    }

                    if (isNew)
                        return true;
                }

                return false;
            }
        }
    }
}
