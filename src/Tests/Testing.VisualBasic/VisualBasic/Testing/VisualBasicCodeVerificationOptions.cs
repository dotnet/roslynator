// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Roslynator.Testing;

namespace Roslynator.VisualBasic.Testing
{
    public class VisualBasicCodeVerificationOptions : CodeVerificationOptions
    {
        public VisualBasicCodeVerificationOptions(
            VisualBasicParseOptions parseOptions,
            VisualBasicCompilationOptions compilationOptions,
            IEnumerable<string> assemblyNames,
            DiagnosticSeverity allowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
            : base(assemblyNames, allowedCompilerDiagnosticSeverity, allowedCompilerDiagnosticIds)
        {
            ParseOptions = parseOptions ?? throw new ArgumentNullException(nameof(parseOptions));
            CompilationOptions = compilationOptions ?? throw new ArgumentNullException(nameof(compilationOptions));
        }

        new public VisualBasicParseOptions ParseOptions { get; }

        new public VisualBasicCompilationOptions CompilationOptions { get; }

        protected override ParseOptions CommonParseOptions => ParseOptions;

        protected override CompilationOptions CommonCompilationOptions => CompilationOptions;

        internal static VisualBasicCodeVerificationOptions Default { get; } = CreateDefault();

        private static VisualBasicCodeVerificationOptions CreateDefault()
        {
            VisualBasicParseOptions parseOptions = null;
            VisualBasicCompilationOptions compilationOptions = null;

            using (var workspace = new AdhocWorkspace())
            {
                Project project = workspace
                    .CurrentSolution
                    .AddProject("TestProject", "TestProject", LanguageNames.VisualBasic);

                compilationOptions = ((VisualBasicCompilationOptions)project.CompilationOptions)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                parseOptions = ((VisualBasicParseOptions)project.ParseOptions)
                    .WithLanguageVersion(LanguageVersion.Latest);
            }

            return new VisualBasicCodeVerificationOptions(
                parseOptions: parseOptions,
                compilationOptions: compilationOptions,
                assemblyNames: RuntimeMetadataReference.DefaultAssemblyNames);
        }

        public VisualBasicCodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.Add(diagnosticId));
        }

        public VisualBasicCodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.AddRange(diagnosticIds));
        }

        public VisualBasicCodeVerificationOptions WithAllowedCompilerDiagnosticIds(IEnumerable<string> allowedCompilerDiagnosticIds)
        {
            if (allowedCompilerDiagnosticIds == null)
                throw new ArgumentNullException(nameof(allowedCompilerDiagnosticIds));

            return new VisualBasicCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: allowedCompilerDiagnosticIds);
        }

        public VisualBasicCodeVerificationOptions WithParseOptions(VisualBasicParseOptions parseOptions)
        {
            return new VisualBasicCodeVerificationOptions(
                parseOptions: parseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }

        public VisualBasicCodeVerificationOptions WithCompilationOptions(VisualBasicCompilationOptions compilationOptions)
        {
            return new VisualBasicCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: compilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }
    }
}
