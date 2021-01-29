// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using static Roslynator.RuntimeMetadataReference;

#pragma warning disable RCS1223

namespace Roslynator.Testing
{
    /// <summary>
    /// Represents a set of options for code verifications.
    /// </summary>
    public abstract class CodeVerificationOptions
    {
        private ImmutableArray<MetadataReference> _metadataReferences;

        /// <summary>
        /// Initializes a new instance of <see cref="CodeVerificationOptions"/>.
        /// </summary>
        /// <param name="assemblyNames"></param>
        /// <param name="allowedCompilerDiagnosticSeverity"></param>
        /// <param name="allowedCompilerDiagnosticIds"></param>
        protected CodeVerificationOptions(
            IEnumerable<string> assemblyNames = null,
            DiagnosticSeverity allowedCompilerDiagnosticSeverity = DiagnosticSeverity.Info,
            IEnumerable<string> allowedCompilerDiagnosticIds = null)
        {
            if (assemblyNames == null)
                throw new ArgumentNullException(nameof(assemblyNames));

            AssemblyNames = assemblyNames.ToImmutableArray();
            AllowedCompilerDiagnosticSeverity = allowedCompilerDiagnosticSeverity;
            AllowedCompilerDiagnosticIds = allowedCompilerDiagnosticIds?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
        }

        /// <summary>
        /// Gets a common parse options.
        /// </summary>
        protected abstract ParseOptions CommonParseOptions { get; }

        /// <summary>
        /// Gets a common compilation options.
        /// </summary>
        protected abstract CompilationOptions CommonCompilationOptions { get; }

        /// <summary>
        /// Gets a parse options that should be used to parse tested source code.
        /// </summary>
        public ParseOptions ParseOptions => CommonParseOptions;

        /// <summary>
        /// Gets a compilation options that should be used to compile test project.
        /// </summary>
        public CompilationOptions CompilationOptions => CommonCompilationOptions;

        /// <summary>
        /// Gets a list of assemblies that should be used to compile test project. Assembly name should be in a format "MyAseembly.dll".
        /// </summary>
        public ImmutableArray<string> AssemblyNames { get; }

        /// <summary>
        /// Gets a diagnostic severity that is allowed. Default value is <see cref="DiagnosticSeverity.Info"/>
        /// which means that compiler diagnostics with severity <see cref="DiagnosticSeverity.Hidden"/>
        /// and <see cref="DiagnosticSeverity.Info"/> are allowed.
        /// </summary>
        public DiagnosticSeverity AllowedCompilerDiagnosticSeverity { get; }

        /// <summary>
        /// Gets a list of compiler diagnostic IDs that are allowed.
        /// </summary>
        public ImmutableArray<string> AllowedCompilerDiagnosticIds { get; }

        internal ImmutableArray<MetadataReference> MetadataReferences
        {
            get
            {
                if (_metadataReferences.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _metadataReferences, CreateMetadataReferences());

                return _metadataReferences;

                ImmutableArray<MetadataReference> CreateMetadataReferences()
                {
                    ImmutableArray<MetadataReference>.Builder builder = ImmutableArray.CreateBuilder<MetadataReference>();

                    builder.Add(CorLibReference);

                    IEnumerable<MetadataReference> metadataReferences;
                    if (!AssemblyNames.IsEmpty)
                    {
                        metadataReferences = AssemblyNames.Select(f => MetadataReference.CreateFromFile(TrustedPlatformAssemblyMap[f]));
                    }
                    else
                    {
                        metadataReferences = TrustedPlatformAssemblyMap.Select(f => MetadataReference.CreateFromFile(f.Value));
                    }

                    builder.AddRange(metadataReferences);

                    return builder.ToImmutableArray();
                }
            }
        }
    }
}
