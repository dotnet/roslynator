// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Roslynator.Tests;

namespace Roslynator.VisualBasic.Tests
{
    public class VisualBasicCodeVerificationOptions : CodeVerificationOptions
    {
        public VisualBasicCodeVerificationOptions(
            VisualBasicParseOptions parseOptions = null,
            VisualBasicCompilationOptions compilationOptions = null,
            bool allowNewCompilerDiagnostics = false,
            bool enableDiagnosticsDisabledByDefault = true,
            DiagnosticSeverity maxAllowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
            : base(parseOptions, compilationOptions, allowNewCompilerDiagnostics, enableDiagnosticsDisabledByDefault, maxAllowedCompilerDiagnosticSeverity, allowedCompilerDiagnosticIds)
        {
            ParseOptions = parseOptions;
            CompilationOptions = compilationOptions;
        }

        new public VisualBasicParseOptions ParseOptions { get; }

        new public VisualBasicCompilationOptions CompilationOptions { get; }

        //TODO: Allowed compiler diagnostic IDs for Visual Basic
        public static VisualBasicCodeVerificationOptions Default { get; } = CreateDefault();

        private static VisualBasicCodeVerificationOptions CreateDefault()
        {
            VisualBasicParseOptions parseOptions = null;
            VisualBasicCompilationOptions compilationOptions = null;

            using (var workspace = new AdhocWorkspace())
            {
                Project project = workspace
                    .CurrentSolution
                    .AddProject("Temp", "Temp", LanguageNames.VisualBasic);

                compilationOptions = ((VisualBasicCompilationOptions)project.CompilationOptions)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                parseOptions = ((VisualBasicParseOptions)project.ParseOptions)
                    .WithLanguageVersion(LanguageVersion.Latest);
            }

            return new VisualBasicCodeVerificationOptions(
                parseOptions: parseOptions,
                compilationOptions: compilationOptions);
        }

        public override CodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.Add(diagnosticId));
        }

        public override CodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds)
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
                allowNewCompilerDiagnostics: AllowNewCompilerDiagnostics,
                enableDiagnosticsDisabledByDefault: EnableDiagnosticsDisabledByDefault,
                maxAllowedCompilerDiagnosticSeverity: MaxAllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: allowedCompilerDiagnosticIds);
        }

        public VisualBasicCodeVerificationOptions WithParseOptions(VisualBasicParseOptions parseOptions)
        {
            return new VisualBasicCodeVerificationOptions(
                parseOptions: parseOptions,
                compilationOptions: CompilationOptions,
                allowNewCompilerDiagnostics: AllowNewCompilerDiagnostics,
                enableDiagnosticsDisabledByDefault: EnableDiagnosticsDisabledByDefault,
                maxAllowedCompilerDiagnosticSeverity: MaxAllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }

        public VisualBasicCodeVerificationOptions WithCompilationOptions(VisualBasicCompilationOptions compilationOptions)
        {
            return new VisualBasicCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: compilationOptions,
                allowNewCompilerDiagnostics: AllowNewCompilerDiagnostics,
                enableDiagnosticsDisabledByDefault: EnableDiagnosticsDisabledByDefault,
                maxAllowedCompilerDiagnosticSeverity: MaxAllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }
    }
}
