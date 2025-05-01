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
            severity = diagnostic.Severity switch
            {
                DiagnosticSeverity.Warning => "major",
                DiagnosticSeverity.Error => "critical",
                _ => "minor",
            };

            string issueFingerPrint = $"{diagnostic.Descriptor.Id}-{diagnostic.Severity}-{location?.Path}-{location?.Lines.Begin}";
            byte[] source = Encoding.UTF8.GetBytes(issueFingerPrint);
            byte[] hashBytes;
#if NETFRAMEWORK
            using (var sha256 = SHA256.Create())
                hashBytes = sha256.ComputeHash(source);
#else
            hashBytes = SHA256.HashData(source);
#endif
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

        string report = JsonConvert.SerializeObject(reportItems, _jsonSerializerSettings);

        File.WriteAllText(filePath, report, Encoding.UTF8);
    }
}
