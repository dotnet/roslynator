// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#pragma warning disable RCS1223

namespace Roslynator.Testing.CSharp
{
    /// <summary>
    /// Represents a set of options for C# code verifications.
    /// </summary>
    public class CSharpCodeVerificationOptions : CodeVerificationOptions
    {
        private static CSharpCodeVerificationOptions _default_CSharp5;
        private static CSharpCodeVerificationOptions _default_CSharp6;
        private static CSharpCodeVerificationOptions _default_CSharp7;
        private static CSharpCodeVerificationOptions _default_CSharp7_3;
        private static CSharpCodeVerificationOptions _default_NullableReferenceTypes;

        /// <summary>
        /// Initializes a new instance of <see cref="CSharpCodeVerificationOptions"/>.
        /// </summary>
        /// <param name="parseOptions"></param>
        /// <param name="compilationOptions"></param>
        /// <param name="assemblyNames"></param>
        /// <param name="allowedCompilerDiagnosticSeverity"></param>
        /// <param name="allowedCompilerDiagnosticIds"></param>
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

        /// <summary>
        /// Gets a parse options that should be used to parse tested source code.
        /// </summary>
        new public CSharpParseOptions ParseOptions { get; }

        /// <summary>
        /// Gets a compilation options that should be used to compile test project.
        /// </summary>
        new public CSharpCompilationOptions CompilationOptions { get; }

        /// <summary>
        /// Gets a common parse options.
        /// </summary>
        protected override ParseOptions CommonParseOptions => ParseOptions;

        /// <summary>
        /// Gets a common compilation options.
        /// </summary>
        protected override CompilationOptions CommonCompilationOptions => CompilationOptions;

        /// <summary>
        /// Gets a default code verification options.
        /// </summary>
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
                    .WithLanguageVersion(LanguageVersion.CSharp9)
                    .WithPreprocessorSymbols(parseOptions.PreprocessorSymbolNames.Concat(new[] { "DEBUG" }));
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

        internal static CSharpCodeVerificationOptions Default_CSharp5
        {
            get
            {
                if (_default_CSharp5 == null)
                    Interlocked.CompareExchange(ref _default_CSharp5, Create(), null);

                return _default_CSharp5;

                static CSharpCodeVerificationOptions Create() => Default.WithParseOptions(Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp5));
            }
        }

        internal static CSharpCodeVerificationOptions Default_CSharp6
        {
            get
            {
                if (_default_CSharp6 == null)
                    Interlocked.CompareExchange(ref _default_CSharp6, Create(), null);

                return _default_CSharp6;

                static CSharpCodeVerificationOptions Create() => Default.WithParseOptions(Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp6));
            }
        }

        internal static CSharpCodeVerificationOptions Default_CSharp7
        {
            get
            {
                if (_default_CSharp7 == null)
                    Interlocked.CompareExchange(ref _default_CSharp7, Create(), null);

                return _default_CSharp7;

                static CSharpCodeVerificationOptions Create() => Default.WithParseOptions(Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp7));
            }
        }

        internal static CSharpCodeVerificationOptions Default_CSharp7_3
        {
            get
            {
                if (_default_CSharp7_3 == null)
                    Interlocked.CompareExchange(ref _default_CSharp7_3, Create(), null);

                return _default_CSharp7_3;

                static CSharpCodeVerificationOptions Create() => Default.WithParseOptions(Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp7_3));
            }
        }

        internal static CSharpCodeVerificationOptions Default_NullableReferenceTypes
        {
            get
            {
                if (_default_NullableReferenceTypes == null)
                    Interlocked.CompareExchange(ref _default_NullableReferenceTypes, Create(), null);

                return _default_NullableReferenceTypes;

                static CSharpCodeVerificationOptions Create() => Default.WithCompilationOptions(Default.CompilationOptions.WithNullableContextOptions(NullableContextOptions.Enable));
            }
        }

        /// <summary>
        /// Adds specified compiler diagnostic ID to the list of allowed compiler diagnostic IDs.
        /// </summary>
        /// <param name="diagnosticId"></param>
        public CSharpCodeVerificationOptions AddAllowedCompilerDiagnosticId(string diagnosticId)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.Add(diagnosticId));
        }

        /// <summary>
        /// Adds a list of specified compiler diagnostic IDs to the list of allowed compiler diagnostic IDs.
        /// </summary>
        /// <param name="diagnosticIds"></param>
        public CSharpCodeVerificationOptions AddAllowedCompilerDiagnosticIds(IEnumerable<string> diagnosticIds)
        {
            return WithAllowedCompilerDiagnosticIds(AllowedCompilerDiagnosticIds.AddRange(diagnosticIds));
        }

        /// <summary>
        /// Adds specified assembly name to the list of assembly names.
        /// </summary>
        /// <param name="assemblyName"></param>
        public CSharpCodeVerificationOptions AddAssemblyName(string assemblyName)
        {
            return WithAssemblyNames(AssemblyNames.Add(assemblyName));
        }

        internal CSharpCodeVerificationOptions WithEnabled(DiagnosticDescriptor descriptor)
        {
            var compilationOptions = (CSharpCompilationOptions)CompilationOptions.EnsureEnabled(descriptor);

            return WithCompilationOptions(compilationOptions);
        }

        internal CSharpCodeVerificationOptions WithEnabled(DiagnosticDescriptor descriptor1, DiagnosticDescriptor descriptor2)
        {
            ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions = CompilationOptions.SpecificDiagnosticOptions;

            diagnosticOptions = diagnosticOptions
                .SetItem(descriptor1.Id, descriptor1.DefaultSeverity.ToReportDiagnostic())
                .SetItem(descriptor2.Id, descriptor2.DefaultSeverity.ToReportDiagnostic());

            CSharpCompilationOptions compilationOptions = CompilationOptions.WithSpecificDiagnosticOptions(diagnosticOptions);

            return WithCompilationOptions(compilationOptions);
        }

        internal CSharpCodeVerificationOptions WithDisabled(DiagnosticDescriptor descriptor)
        {
            var compilationOptions = (CSharpCompilationOptions)CompilationOptions.EnsureSuppressed(descriptor);

            return WithCompilationOptions(compilationOptions);
        }
#pragma warning disable CS1591
        public CSharpCodeVerificationOptions WithAllowedCompilerDiagnosticIds(IEnumerable<string> allowedCompilerDiagnosticIds)
        {
            return new CSharpCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: allowedCompilerDiagnosticIds);
        }

        public CSharpCodeVerificationOptions WithAllowedCompilerDiagnosticSeverity(DiagnosticSeverity allowedCompilerDiagnosticSeverity)
        {
            return new CSharpCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: AssemblyNames,
                allowedCompilerDiagnosticSeverity: allowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
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

        public CSharpCodeVerificationOptions WithAssemblyNames(IEnumerable<string> assemblyNames)
        {
            return new CSharpCodeVerificationOptions(
                parseOptions: ParseOptions,
                compilationOptions: CompilationOptions,
                assemblyNames: assemblyNames,
                allowedCompilerDiagnosticSeverity: AllowedCompilerDiagnosticSeverity,
                allowedCompilerDiagnosticIds: AllowedCompilerDiagnosticIds);
        }
#pragma warning restore CS1591
    }
}
