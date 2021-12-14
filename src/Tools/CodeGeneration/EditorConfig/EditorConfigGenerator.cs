// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.Configuration;
using Roslynator.Metadata;

namespace Roslynator.CodeGeneration.EditorConfig
{
    public static class EditorConfigGenerator
    {
        public static string GenerateEditorConfig(RoslynatorMetadata metadata)
        {
            var optionMap = new Dictionary<string, HashSet<AnalyzerMetadata>>();

            foreach (AnalyzerMetadata analyzer in metadata.Analyzers)
            {
                foreach (string option in analyzer.GlobalOptions)
                {
                    if (!optionMap.TryGetValue(option, out HashSet<AnalyzerMetadata> optionAnalyzers))
                        optionAnalyzers = new HashSet<AnalyzerMetadata>();

                    optionAnalyzers.Add(analyzer);
                    optionMap[option] = optionAnalyzers;
                }
            }

            using (var w = new EditorConfigWriter(new StringWriter()))
            {
                w.WriteLine("# Options");
                w.WriteLine();

                foreach (OptionDescriptor option in metadata.Options.OrderBy(f => f.Key))
                {
                    if (optionMap.TryGetValue(option.Key, out HashSet<AnalyzerMetadata> analyzers))
                    {
                        w.WriteLine("# Applicable to: " + string.Join(", ", analyzers.OrderBy(f => f.Id).Select(f => f.Id)));
                    }

                    w.WriteEntry(option.Key, option.DefaultValue);
                }

                w.WriteLine();
                w.WriteLine("# Analyzers");
                w.WriteLine();

                foreach (AnalyzerMetadata analyzer in metadata.GetAllAnalyzers()
                    .Where(f => !f.IsObsolete)
                    .OrderBy(f => f.Id))
                {
                    w.WriteLine($"# {analyzer.Title.TrimEnd('.')}");
                    w.WriteAnalyzer(
                        analyzer.Id,
                        (analyzer.IsEnabledByDefault)
                            ? ((DiagnosticSeverity)Enum.Parse(typeof(DiagnosticSeverity), analyzer.DefaultSeverity)).ToReportDiagnostic()
                            : ReportDiagnostic.Suppress);

                    foreach (AnalyzerOptionMetadata option in analyzer.Options.OrderBy(f => f.OptionKey))
                    {
                        w.WriteLine($"# {option.Title.TrimEnd('.')}");
                        w.WriteEntry($"roslynator.{analyzer.Id}.{option.OptionKey}", false);
                    }

                    w.WriteLine();
                }

                w.WriteLine();
                w.WriteLine("# Refactorings");
                w.WriteLine();

                foreach (RefactoringMetadata refactoring in metadata.Refactorings
                    .Where(f => !f.IsObsolete)
                    .OrderBy(f => f.OptionKey))
                {
                    w.WriteRefactoring(refactoring.OptionKey, refactoring.IsEnabledByDefault);
                }

                w.WriteLine();
                w.WriteLine("# Compiler diagnostic fixes");
                w.WriteLine();

                foreach (CompilerDiagnosticMetadata compilerDiagnostic in metadata.CompilerDiagnostics
                    .OrderBy(f => f.Id))
                {
                    w.WriteCompilerDiagnosticFix(compilerDiagnostic.Id, true);
                }

                const string content = @"# Roslynator Config File

## Set severity for all analyzers
#dotnet_analyzer_diagnostic.category-roslynator.severity = default|none|silent|suggestion|warning|error

## Set severity for a specific analyzer
#dotnet_diagnostic.<ANALYZER_ID>.severity = default|none|silent|suggestion|warning|error

## Enable/disable all refactorings
#roslynator.refactorings.enabled = true|false

## Enable/disable specific refactoring
#roslynator.refactoring.<REFACTORING_NAME>.enabled = true|false

## Enable/disable all fixes for compiler diagnostics
#roslynator.compiler_diagnostic_fixes.enabled = true|false

## Enable/disable fix for a specific compiler diagnostics
#roslynator.compiler_diagnostic_fix.<COMPILER_DIAGNOSTIC_ID>.enabled = true|false
";

                return content + w.ToString();
            }
        }
    }
}
