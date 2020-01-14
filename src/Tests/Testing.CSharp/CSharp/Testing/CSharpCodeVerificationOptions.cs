// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing;

namespace Roslynator.CSharp.Testing
{
    public class CSharpCodeVerificationOptions : CodeVerificationOptions
    {
        private static CSharpCodeVerificationOptions _defaultWithCSharp5;
        private static CSharpCodeVerificationOptions _defaultWithCSharp6;

        public CSharpCodeVerificationOptions(
            CSharpParseOptions parseOptions,
            CSharpCompilationOptions compilationOptions,
            IEnumerable<string> assemblyNames,
            DiagnosticSeverity allowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
            : base(assemblyNames, allowedCompilerDiagnosticSeverity, allowedCompilerDiagnosticIds)
        {
            ParseOptions = parseOptions ?? throw new ArgumentNullException(nameof(parseOptions));
            CompilationOptions = compilationOptions ?? throw new ArgumentNullException(nameof(compilationOptions));
        }

        new public CSharpParseOptions ParseOptions { get; }

        new public CSharpCompilationOptions CompilationOptions { get; }

        protected override ParseOptions CommonParseOptions => ParseOptions;

        protected override CompilationOptions CommonCompilationOptions => CompilationOptions;

        public static CSharpCodeVerificationOptions Default { get; } = CreateDefault();

        private static CSharpCodeVerificationOptions CreateDefault()
        {
            CSharpParseOptions parseOptions = null;
            CSharpCompilationOptions compilationOptions = null;

            using (var workspace = new AdhocWorkspace())
            {
                Project project = workspace
                    .CurrentSolution
                    .AddProject("TestProject", "TestProject", LanguageNames.CSharp);

                compilationOptions = ((CSharpCompilationOptions)project.CompilationOptions)
                    .WithAllowUnsafe(true)
                    .WithOutputKind(OutputKind.DynamicallyLinkedLibrary);

                parseOptions = ((CSharpParseOptions)project.ParseOptions);

                parseOptions = parseOptions
                    .WithLanguageVersion(LanguageVersion.Latest)
                    .WithPreprocessorSymbols(parseOptions.PreprocessorSymbolNames.Concat(new string[] { "DEBUG" }));
            }

            return new CSharpCodeVerificationOptions(
                parseOptions: parseOptions,
                compilationOptions: compilationOptions,
                assemblyNames: RuntimeMetadataReference.DefaultAssemblyNames,
                allowedCompilerDiagnosticIds: ImmutableArray.Create(
                    "CS0067", // Event is never used
                    "CS0168", // Variable is declared but never used
                    "CS0169", // Field is never used
                    "CS0219", // Variable is assigned but its value is never used
                    "CS0414", // Field is assigned but its value is never used
                    "CS0649", // Field is never assigned to, and will always have its default value null
                    "CS0660", // Type defines operator == or operator != but does not override Object.Equals(object o)
                    "CS0661", // Type defines operator == or operator != but does not override Object.GetHashCode()
                    "CS8019", // Unnecessary using directive
                    "CS8321" // The local function is declared but never used
                ));
        }

        internal static CSharpCodeVerificationOptions DefaultWithCSharp5
        {
            get
            {
                if (_defaultWithCSharp5 == null)
                    Interlocked.CompareExchange(ref _defaultWithCSharp5, LoadDefaultOptionsWithCSharp5(), null);

                return _defaultWithCSharp5;

                static CSharpCodeVerificationOptions LoadDefaultOptionsWithCSharp5() => Default.WithParseOptions(Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp5));
            }
        }

        internal static CSharpCodeVerificationOptions DefaultWithCSharp6
        {
            get
            {
                if (_defaultWithCSharp6 == null)
                    Interlocked.CompareExchange(ref _defaultWithCSharp6, LoadDefaultOptionsWithCSharp6(), null);

                return _defaultWithCSharp6;

                static CSharpCodeVerificationOptions LoadDefaultOptionsWithCSharp6() => Default.WithParseOptions(Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp6));
            }
        }

        public CSharpCodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.Add(diagnosticId));
        }

        public CSharpCodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.AddRange(diagnosticIds));
        }

        public CSharpCodeVerificationOptions WithAllowedCompilerDiagnosticIds(IEnumerable<string> allowedCompilerDiagnosticIds)
        {
            return new CSharpCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: allowedCompilerDiagnosticIds);
        }

        public CSharpCodeVerificationOptions WithParseOptions(CSharpParseOptions parseOptions)
        {
            return new CSharpCodeVerificationOptions(
                parseOptions: parseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }

        public CSharpCodeVerificationOptions WithCompilationOptions(CSharpCompilationOptions compilationOptions)
        {
            return new CSharpCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: compilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }
    }
}
