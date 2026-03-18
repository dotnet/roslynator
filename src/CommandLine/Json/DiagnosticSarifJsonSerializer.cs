// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Roslynator.CommandLine.Sarif;
using Roslynator.Diagnostics;

namespace Roslynator.CommandLine.Json;

internal static class DiagnosticSarifJsonSerializer
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Newtonsoft.Json.Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
    };

    public static void Serialize(
        IEnumerable<ProjectAnalysisResult> results,
        string filePath,
        IFormatProvider formatProvider = null)
    {
        IEnumerable<DiagnosticInfo> diagnostics = results.SelectMany(f => f.CompilerDiagnostics.Concat(f.Diagnostics));
        var rootObject = new RootObject();
        var run = new Run()
        {
            Results = new List<Result>()
        };
        foreach (DiagnosticInfo diagnostic in diagnostics)
        {
            Location location = null;
            if(diagnostic.LineSpan.IsValid)
            {
                location = new Location()
                {
                    PhysicalLocation = new PhysicalLocation()
                    {
                        ArtifactLocation = new ArtifactLocation()
                        {
                            Uri = $"file:///{diagnostic.LineSpan.Path}"
                        },
                        Region = new Region()
                        {
                            StartLine = diagnostic.LineSpan.StartLinePosition.Line,
                            StartColumn = diagnostic.LineSpan.StartLinePosition.Character,
                            EndLine = diagnostic.LineSpan.EndLinePosition.Line,
                            EndColumn = diagnostic.LineSpan.EndLinePosition.Character,
                        },
                    },
                };
            }
            run.Results.Add(new Result()
            {
                RuleId = diagnostic.Descriptor.Id,
                Message = new Message() { Text = diagnostic.Descriptor.Title.ToString(formatProvider) },
                Level = GetLevel(diagnostic.Severity),
                Locations = new List<Location>() { location },
            });
        }
        run.Artifacts = diagnostics
            .Where(d => d.LineSpan.IsValid)
            .Select(d => d.LineSpan.Path)
            .Distinct()
            .Select(path => new Artifact()
            {
                Location = new ArtifactLocation() { Uri = $"file:///{path}" }
            })
            .ToArray();
        Assembly assembly = typeof(DiagnosticSarifJsonSerializer).Assembly;
        string projectUrl = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(a => a.Key == "PackageProjectUrl")?.Value;
        AssemblyName assemblyName = assembly.GetName();
        run.Tool = new Tool()
        {
            Driver = new ToolComponent()
            {
                Name = assemblyName.Name,
                Version = assemblyName.Version.ToString(),
                InformationUri = projectUrl
            },
        };
        rootObject.Runs = new Run[] { run };
        string report = JsonConvert.SerializeObject(rootObject, _jsonSerializerSettings);

        File.WriteAllText(filePath, report, Encoding.UTF8);
    }

    private static string GetLevel(Microsoft.CodeAnalysis.DiagnosticSeverity severity)
    {
        return severity switch
            {
                Microsoft.CodeAnalysis.DiagnosticSeverity.Info => "note",
                Microsoft.CodeAnalysis.DiagnosticSeverity.Warning => "warning",
                Microsoft.CodeAnalysis.DiagnosticSeverity.Error => "error",
                _ => "none",
            };
    }
}
