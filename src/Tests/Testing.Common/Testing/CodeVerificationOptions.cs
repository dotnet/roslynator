// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using static Roslynator.RuntimeMetadataReference;

namespace Roslynator.Testing
{
    public abstract class CodeVerificationOptions
    {
        private ImmutableArray<MetadataReference> _metadataReferences;

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

        protected abstract ParseOptions CommonParseOptions { get; }

        protected abstract CompilationOptions CommonCompilationOptions { get; }

        public ParseOptions ParseOptions => CommonParseOptions;

        public CompilationOptions CompilationOptions => CommonCompilationOptions;

        public ImmutableArray<string> AssemblyNames { get; }

        public DiagnosticSeverity AllowedCompilerDiagnosticSeverity { get; }

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
