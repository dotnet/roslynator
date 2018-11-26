// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;

namespace Roslynator.CodeFixes
{
    public class CodeFixerOptions : CodeAnalysisOptions
    {
        private ImmutableArray<string> _fileBannerLines;

        public static CodeFixerOptions Default { get; } = new CodeFixerOptions();

        public CodeFixerOptions(
            DiagnosticSeverity severityLevel = DiagnosticSeverity.Info,
            bool ignoreCompilerErrors = false,
            bool ignoreAnalyzerReferences = false,
            IEnumerable<string> supportedDiagnosticIds = null,
            IEnumerable<string> ignoredDiagnosticIds = null,
            IEnumerable<string> ignoredCompilerDiagnosticIds = null,
            IEnumerable<string> projectNames = null,
            IEnumerable<string> ignoredProjectNames = null,
            IEnumerable<string> diagnosticIdsFixableOneByOne = null,
            IEnumerable<KeyValuePair<string, string>> diagnosticFixMap = null,
            IEnumerable<KeyValuePair<string, string>> diagnosticFixerMap = null,
            string fileBanner = null,
            string language = null,
            int maxIterations = -1,
            int batchSize = -1,
            bool format = false) : base(severityLevel, ignoreAnalyzerReferences, supportedDiagnosticIds, ignoredDiagnosticIds, projectNames, ignoredProjectNames, language)
        {
            IgnoreCompilerErrors = ignoreCompilerErrors;
            IgnoredCompilerDiagnosticIds = ignoredCompilerDiagnosticIds?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            DiagnosticIdsFixableOneByOne = diagnosticIdsFixableOneByOne?.ToImmutableHashSet() ?? ImmutableHashSet<string>.Empty;
            DiagnosticFixMap = diagnosticFixMap?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
            DiagnosticFixerMap = diagnosticFixerMap?.ToImmutableDictionary() ?? ImmutableDictionary<string, string>.Empty;
            FileBanner = fileBanner;
            MaxIterations = maxIterations;
            BatchSize = batchSize;
            Format = format;
        }

        public bool IgnoreCompilerErrors { get; }

        public ImmutableHashSet<string> IgnoredCompilerDiagnosticIds { get; }

        public string FileBanner { get; }

        public ImmutableArray<string> FileBannerLines
        {
            get
            {
                if (_fileBannerLines.IsDefault)
                {
                    if (!string.IsNullOrEmpty(FileBanner))
                    {
                        ImmutableArray<string>.Builder lines = ImmutableArray.CreateBuilder<string>();

                        using (var sr = new StringReader(FileBanner))
                        {
                            string line = null;
                            while ((line = sr.ReadLine()) != null)
                            {
                                lines.Add(line);
                            }
                        }

                        _fileBannerLines = lines.ToImmutableArray();
                    }
                    else
                    {
                        _fileBannerLines = ImmutableArray<string>.Empty;
                    }
                }

                return _fileBannerLines;
            }
        }

        public int MaxIterations { get; }

        public int BatchSize { get; }

        public bool Format { get; }

        public ImmutableHashSet<string> DiagnosticIdsFixableOneByOne { get; }

        public ImmutableDictionary<string, string> DiagnosticFixMap { get; }

        public ImmutableDictionary<string, string> DiagnosticFixerMap { get; }
    }
}
