// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Roslynator.Diagnostics;

namespace Roslynator.CommandLine;

internal class AnalyzeCommandResult : CommandResult
{
    public AnalyzeCommandResult(CommandStatus status, ImmutableArray<ProjectAnalysisResult> analysisResults, string rootDirectoryPath = null)
        : base(status)
    {
        AnalysisResults = analysisResults;
        RootDirectoryPath = rootDirectoryPath;
    }

    public ImmutableArray<ProjectAnalysisResult> AnalysisResults { get; }

    public string RootDirectoryPath { get; }
}
