// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Roslynator.Metadata;
using Roslynator.Utilities;

namespace Roslynator.CodeGeneration
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            string rootPath = @"..\..\..\..\..";
#else
            string rootPath = System.Environment.CurrentDirectory;
#endif
            if (args?.Length > 0)
                rootPath = args[0];

            var metadata = new RoslynatorMetadata(rootPath);

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine("# Options");

            foreach (string value in metadata.GetAllAnalyzers()
                .SelectMany(f => f.Options)
                .Select(analyzerOption =>
                {
                    string optionKey = analyzerOption.OptionKey;

                    if (!optionKey.StartsWith("roslynator.", StringComparison.Ordinal))
                        optionKey = $"roslynator.{analyzerOption.ParentId}.{optionKey}";

                    return "#" + optionKey + " = " + (analyzerOption.OptionValue ?? "true");
                })
                .OrderBy(f => f))
            {
                sb.AppendLine(value);
            }

            GenerateRules(metadata.Analyzers, "Roslynator.Analyzers");
            GenerateRules(metadata.FormattingAnalyzers, "Roslynator.Formatting.Analyzers");
            GenerateRules(metadata.CodeAnalysisAnalyzers, "Roslynator.CodeAnalysis.Analyzers");

            string content = sb.ToString();

            FileHelper.WriteAllText("default.editorconfig", content, Encoding.UTF8, onlyIfChanges: false, fileMustExists: false);

            void GenerateRules(IEnumerable<AnalyzerMetadata> analyzers, string heading)
            {
                sb.AppendLine();
                sb.Append("# ");
                sb.AppendLine(heading);

                foreach (AnalyzerMetadata analyzer in analyzers
                    .Where(f => !f.IsObsolete)
                    .OrderBy(f => f.Id))
                {
                    sb.Append("dotnet_diagnostic.");
                    sb.Append(analyzer.Id);
                    sb.Append(".severity = ");
                    sb.AppendLine(GetSeverity(analyzer));
                }
            }

            string GetSeverity(AnalyzerMetadata analyzer)
            {
                if (!analyzer.IsEnabledByDefault)
                    return "none";

                switch (analyzer.DefaultSeverity)
                {
                    case "Hidden":
                        return "silent";
                    case "Info":
                        return "suggestion";
                    case "Warning":
                        return "warning";
                    case "Error":
                        return "error";
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
    }
}
