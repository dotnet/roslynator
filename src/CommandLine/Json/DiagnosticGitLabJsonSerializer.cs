using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        IList<AnalyzeCommandResult> results,
        string filePath,
        IFormatProvider formatProvider = null)
    {
        var reportItems = new List<GitLabIssue>();

        foreach (AnalyzeCommandResult commandResult in results)
        {
            string baseDirectoryPath = commandResult.RootDirectoryPath;

            foreach (ProjectAnalysisResult result in commandResult.AnalysisResults)
            {
                foreach (DiagnosticInfo diagnostic in result.CompilerDiagnostics.Concat(result.Diagnostics))
                {
                    GitLabIssueLocation location = null;
                    if (diagnostic.LineSpan.IsValid)
                    {
                        location = new GitLabIssueLocation()
                        {
                            Path = FormatPath(diagnostic.LineSpan.Path, baseDirectoryPath),
                            Lines = new GitLabLocationLines()
                            {
                                Begin = diagnostic.LineSpan.StartLinePosition.Line + 1
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
#pragma warning disable CA1872 // Use Convert.ToHexString instead of BitConverter.ToString
                    issueFingerPrint = BitConverter.ToString(hashBytes)
                        .Replace("-", "")
                        .ToLowerInvariant();
#pragma warning restore CA1872

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
            }
        }

        string report = JsonConvert.SerializeObject(reportItems, _jsonSerializerSettings);

        File.WriteAllText(filePath, report, Encoding.UTF8);
    }

    private static string FormatPath(string path, string baseDirectoryPath)
    {
        if (string.IsNullOrEmpty(path))
            return path;

        if (string.IsNullOrEmpty(baseDirectoryPath))
            return ToForwardSlashPath(path);

        string normalizedPath = NormalizePath(path);
        string normalizedBase = NormalizePath(baseDirectoryPath);

        StringComparison comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        if (string.Equals(normalizedPath, normalizedBase, comparison))
            return ToForwardSlashPath(Path.GetFileName(normalizedPath));

        if (normalizedPath.StartsWith(normalizedBase, comparison))
        {
            int length = normalizedBase.Length;

            while (length < normalizedPath.Length && IsDirectorySeparator(normalizedPath[length]))
                length++;

            return ToForwardSlashPath(normalizedPath.Substring(length));
        }

        return ToForwardSlashPath(path);
    }

    private static string NormalizePath(string path)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception)
        {
            return path;
        }
    }

    private static bool IsDirectorySeparator(char c)
        => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar;

    private static string ToForwardSlashPath(string path)
        => path.Replace('\\', '/');
}
