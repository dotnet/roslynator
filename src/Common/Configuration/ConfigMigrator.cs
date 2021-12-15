// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Configuration
{
    internal static class ConfigMigrator
    {
        public static void MigrateToEditorConfig()
        {
            foreach (string path in CodeAnalysisConfig.Instance
                .XmlConfig
                .Includes
                .Where(path => Path.GetFileName(path) == XmlCodeAnalysisConfig.FileName))
            {
                Migrate(path);
            }
        }

        private static void Migrate(string path)
        {
            string editorConfigPath = Path.Combine(Path.GetDirectoryName(path), EditorConfigCodeAnalysisConfig.FileName);

            if (File.Exists(editorConfigPath))
                return;

            var xmlConfigMigrated = false;
            var ruleSetMigrated = false;

            EditorConfigWriter writer = null;
            try
            {
                if (File.Exists(path))
                {
                    XmlCodeAnalysisConfig config = XmlCodeAnalysisConfigLoader.Load(path, XmlConfigLoadOptions.SkipIncludes);

                    writer = new EditorConfigWriter(new StringWriter());

                    writer.WriteGlobalDirective();
                    writer.WriteLine();

                    if (config.MaxLineLength != null)
                        writer.WriteEntry(OptionKeys.MaxLineLength, config.MaxLineLength.ToString());

                    if (config.PrefixFieldIdentifierWithUnderscore != null)
                        writer.WriteEntry(OptionKeys.PrefixFieldIdentifierWithUnderscore, config.PrefixFieldIdentifierWithUnderscore.Value);

                    writer.WriteLineIf(config.MaxLineLength != null || config.PrefixFieldIdentifierWithUnderscore != null);

                    writer.WriteRefactorings(config.Refactorings.OrderBy(f => f.Key));
                    writer.WriteLineIf(config.Refactorings.Count > 0);

                    writer.WriteCompilerDiagnosticFixes(config.CodeFixes.OrderBy(f => f.Key));
                    writer.WriteLineIf(config.CodeFixes.Count > 0);
                    xmlConfigMigrated = true;
                }

                string ruleSetPath = Path.Combine(Path.GetDirectoryName(path), RuleSetUtility.DefaultRuleSetName);

                if (File.Exists(ruleSetPath))
                {
                    RuleSet ruleSet = null;

                    try
                    {
                        ruleSet = RuleSet.LoadEffectiveRuleSetFromFile(ruleSetPath);
                    }
                    catch (Exception ex) when (ex is IOException
                        || ex is UnauthorizedAccessException
                        || ex.GetType().FullName == "Microsoft.CodeAnalysis.InvalidRuleSetException")
                    {
                    }

                    if (ruleSet != null)
                    {
                        if (writer == null)
                        {
                            writer = new EditorConfigWriter(new StringWriter());

                            writer.WriteGlobalDirective();
                            writer.WriteLine();
                        }

                        ReportDiagnostic generalOption = ruleSet.GeneralDiagnosticOption;

                        if (generalOption != ReportDiagnostic.Default)
                        {
                            writer.WriteAnalyzerCategory(DiagnosticCategories.Roslynator.ToLowerInvariant(), generalOption);
                            writer.WriteLine();
                        }

                        var hasOption = false;

                        foreach (KeyValuePair<string, string> kvp in ruleSet.SpecificDiagnosticOptions
                            .Where(kvp => Regex.IsMatch(kvp.Key, @"\ARCS\d{4}[a-z]\z"))
                            .Select(kvp =>
                            {
                                string key = MapRuleSetOptionToEditorConfigOption(kvp.Key);

                                if (key != null)
                                {
                                    string value = (kvp.Value == ReportDiagnostic.Suppress) ? "false" : "true";

                                    return new KeyValuePair<string, string>(key, value);
                                }

                                return default;
                            })
                            .Where(f => f.Key != null)
                            .OrderBy(f => f.Key))
                        {
                            writer.WriteEntry(kvp);
                            hasOption = true;
                        }

                        writer.WriteLineIf(hasOption);

                        writer.WriteAnalyzers(ruleSet.SpecificDiagnosticOptions
                            .Where(f => Regex.IsMatch(f.Key, @"\ARCS\d{4}\z"))
                            .OrderBy(f => f.Key));

                        ruleSetMigrated = true;
                    }
                }

                if (writer != null)
                {
                    File.WriteAllText(editorConfigPath, writer.ToString());

                    if (xmlConfigMigrated)
                        MarkFileAsMigrated(path);

                    if (ruleSetMigrated)
                        MarkFileAsMigrated(ruleSetPath);
                }
            }
            finally
            {
                writer?.Dispose();
            }

            static void MarkFileAsMigrated(string path)
            {
                string content = File.ReadAllText(path, Encoding.UTF8);

                Match match = Regex.Match(content, @"\A<\?xml version=""1\.0"" encoding=""utf-8""\?>(\r?\n)?");

                content = content.Insert(
                    (match.Success) ? match.Length : 0,
                    "<!-- IMPORTANT: THIS CONFIGURATION HAS BEEN MIGRATED TO .roslynatorconfig file -->" + Environment.NewLine);

                File.WriteAllText(path, content);
                File.Move(path, path + ".migrated");
            }

            static string MapRuleSetOptionToEditorConfigOption(string id)
            {
                switch (id)
                {
                    case "RCS0011i":
                        return "roslynator.RCS0011.invert";
                    case "RCS0015i":
                        return "roslynator.RCS0015.invert";
                    case "RCS0027i":
                        return "roslynator.RCS0027.invert";
                    case "RCS0028i":
                        return "roslynator.RCS0028.invert";
                    case "RCS0032i":
                        return "roslynator.RCS0032.invert";
                    case "RCS0051i":
                        return "roslynator.RCS0051.invert";
                    case "RCS0052i":
                        return "roslynator.RCS0052.invert";
                    case "RCS1014a":
                        return "roslynator.RCS1014.use_implicit_type_when_obvious";
                    case "RCS1014i":
                        return "roslynator.RCS1014.invert";
                    case "RCS1016a":
                        return "roslynator.RCS1016.use_block_body_when_expression_is_multiline";
                    case "RCS1016b":
                        return "roslynator.RCS1016.use_block_body_when_declaration_is_multiline";
                    case "RCS1016i":
                        return "roslynator.RCS1016.invert";
                    case "RCS1018i":
                        return "roslynator.RCS1018.invert";
                    case "RCS1036a":
                        return "roslynator.RCS1036.remove_empty_line_between_closing_brace_and_switch_section";
                    case "RCS1045a":
                        return "roslynator.RCS1045.suppress_when_field_is_static";
                    case "RCS1050i":
                        return "roslynator.RCS1050.invert";
                    case "RCS1051a":
                        return "roslynator.RCS1051.do_not_parenthesize_single_token";
                    case "RCS1078i":
                        return "roslynator.RCS1078.invert";
                    case "RCS1090i":
                        return "roslynator.RCS1090.invert";
                    case "RCS1096i":
                        return "roslynator.RCS1096.invert";
                    case "RCS1104a":
                        return "roslynator.RCS1104.suppress_when_condition_is_inverted";
                    case "RCS1207i":
                        return "roslynator.RCS1207.invert";
                        case "RCS1246a":
                        return "roslynator.RCS1246.suppress_when_expression_is_invocation";
                    case "RCS1248i":
                        return "roslynator.RCS1248.invert";
                }

                Debug.Fail(id);
                return null;
            }
        }
    }
}
