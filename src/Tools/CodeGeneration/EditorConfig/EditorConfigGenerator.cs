﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.Configuration;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration.EditorConfig;

public static class EditorConfigGenerator
{
    public static string GenerateEditorConfig(RoslynatorMetadata metadata, bool commentOut)
    {
        var optionMap = new Dictionary<string, HashSet<AnalyzerMetadata>>();

        foreach (AnalyzerMetadata analyzer in metadata.Analyzers)
        {
            foreach (AnalyzerConfigOption option in analyzer.ConfigOptions)
            {
                if (!optionMap.TryGetValue(option.Key, out HashSet<AnalyzerMetadata> optionAnalyzers))
                    optionAnalyzers = new HashSet<AnalyzerMetadata>();

                optionAnalyzers.Add(analyzer);
                optionMap[option.Key] = optionAnalyzers;
            }
        }

        using (var w = new EditorConfigWriter(new StringWriter()))
        {
            w.WriteLine();
            w.WriteLine("# Options");
            w.WriteLine();

            var isSeparatedWithNewLine = true;

            foreach (AnalyzerOptionMetadata option in metadata.ConfigOptions
                .Where(f => !f.IsObsolete)
                .OrderBy(f => f.Key, StringComparer.InvariantCulture))
            {
                if (optionMap.TryGetValue(option.Key, out HashSet<AnalyzerMetadata> analyzers)
                    && !isSeparatedWithNewLine)
                {
                    w.WriteLine();
                }

                w.WriteCommentCharIf(commentOut);
                w.WriteEntry($"{option.Key}", option.DefaultValuePlaceholder);

                string defaultValue = option.DefaultValue;

                if (defaultValue is not null)
                    w.WriteLine($"# Default: {defaultValue}");

                if (analyzers?.Count > 0)
                {
                    w.WriteLine("# Applicable to: " + string.Join(", ", analyzers.OrderBy(f => f.Id, StringComparer.InvariantCulture).Select(f => f.Id.ToLowerInvariant())));
                    w.WriteLine();
                    isSeparatedWithNewLine = true;
                }
                else
                {
                    isSeparatedWithNewLine = false;
                }
            }

            w.WriteLine();
            w.WriteLine("# Analyzers");
            w.WriteLine();

            foreach (AnalyzerMetadata analyzer in metadata.Analyzers
                .Where(f => f.Status == AnalyzerStatus.Enabled && !f.Tags.Contains("HideFromConfiguration"))
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                w.WriteLine($"# {analyzer.Title.TrimEnd('.')}");
                w.WriteCommentCharIf(commentOut);
                w.WriteAnalyzer(
                    analyzer.Id.ToLowerInvariant(),
                    (analyzer.IsEnabledByDefault)
                        ? Enum.Parse<DiagnosticSeverity>(analyzer.DefaultSeverity).ToReportDiagnostic()
                        : ReportDiagnostic.Suppress);

                if (analyzer.ConfigOptions.Count > 0)
                {
                    w.WriteLine("# Options: "
                        + string.Join(
                            ", ",
                            analyzer.ConfigOptions
                                .Where(f => metadata.ConfigOptions.FirstOrDefault(ff => ff.Key == f.Key)?.IsObsolete != true)
                                .OrderBy(f => f.Key, StringComparer.InvariantCulture)
                                .Select(f2 => metadata.ConfigOptions.First(f => f.Key == f2.Key).Key)));
                }

                w.WriteLine();
            }

            w.WriteLine();
            w.WriteLine("# Refactorings");
            w.WriteLine();

            foreach (RefactoringMetadata refactoring in metadata.Refactorings
                .Where(f => !f.IsObsolete)
                .OrderBy(f => f.OptionKey, StringComparer.InvariantCulture))
            {
                w.WriteCommentCharIf(commentOut);
                w.WriteRefactoring(refactoring.OptionKey, refactoring.IsEnabledByDefault);
            }

            w.WriteLine();
            w.WriteLine("# Compiler diagnostic fixes");
            w.WriteLine();

            foreach (CompilerDiagnosticMetadata compilerDiagnostic in metadata.CompilerDiagnostics
                .OrderBy(f => f.Id, StringComparer.InvariantCulture))
            {
                w.WriteCommentCharIf(commentOut);
                w.WriteCompilerDiagnosticFix(compilerDiagnostic.Id.ToLowerInvariant(), true);
            }

            return w.ToString();
        }
    }
}
