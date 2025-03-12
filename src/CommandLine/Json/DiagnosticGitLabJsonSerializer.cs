using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Roslynator.CommandLine.GitLab;
using Roslynator.Diagnostics;

namespace Roslynator.CommandLine.Json;

internal static class DiagnosticGitLabJsonSerializer
{
    public static void Serialize(
        IEnumerable<ProjectAnalysisResult> results,
        string filePath,
        IFormatProvider formatProvider = null)
    {
        IEnumerable<DiagnosticInfo> diagnostics = results.SelectMany(f => f.CompilerDiagnostics.Concat(f.Diagnostics));

        var reportItems = new List<GitLabIssue>();
        foreach (DiagnosticInfo diagnostic in diagnostics)
        {
            GitLabIssueLocation location = null;
            if (diagnostic.LineSpan.IsValid)
            {
                location = new GitLabIssueLocation()
                {
                    Path = diagnostic.LineSpan.Path,
                    Lines = new GitLabLocationLines()
                    {
                        Begin = diagnostic.LineSpan.StartLinePosition.Line
                    },
                };
            }

            var severity = "minor";
            switch (diagnostic.Severity)
            {
                case DiagnosticSeverity.Warning:
                    severity = "major";
                    break;
                case DiagnosticSeverity.Error:
                    severity = "critical";
                    break;
                default:
                    severity = "minor";
                    break;
            }

            string issueFingerPrint = $"{diagnostic.Descriptor.Id}-{diagnostic.Severity}-{location?.Path}-{location?.Lines.Begin}";
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(issueFingerPrint));
            issueFingerPrint = BitConverter.ToString(hashBytes)
                .Replace("-", "")
                .ToLowerInvariant();

            reportItems.Add(new GitLabIssue()
            {
                Type = "issue",
                Fingerprint = issueFingerPrint,
                CheckName = diagnostic.Descriptor.Id,
                Description = diagnostic.Descriptor.Title.ToString(formatProvider),
                Severity = severity,
                Location = location,
                Categories = new string[] { diagnostic.Descriptor.Category },
            });
        }

        string gitlabreport = JsonConvert.SerializeObject(
            reportItems,
            new JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
            });

        File.WriteAllText(filePath, gitlabreport, Encoding.UTF8);
    }
}
