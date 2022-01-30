// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                        writer.WriteEntry(ConfigOptionKeys.MaxLineLength, config.MaxLineLength.ToString());

                    if (config.PrefixFieldIdentifierWithUnderscore != null)
                        writer.WriteEntry(ConfigOptionKeys.PrefixFieldIdentifierWithUnderscore, config.PrefixFieldIdentifierWithUnderscore.Value);

                    writer.WriteLineIf(config.MaxLineLength != null || config.PrefixFieldIdentifierWithUnderscore != null);

                    writer.WriteRefactorings(config.Refactorings.OrderBy(f => f.Key));
                    writer.WriteLineIf(config.Refactorings.Count > 0);

                    writer.WriteCompilerDiagnosticFixes(config.CodeFixes.OrderBy(f => f.Key));
                    writer.WriteLineIf(config.CodeFixes.Count > 0);
                    xmlConfigMigrated = true;
                }

                string ruleSetPath = Path.Combine(Path.GetDirectoryName(path), RuleSetLoader.DefaultRuleSetName);

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
                                (string key, string value) = MapRuleSetOptionToEditorConfigOption(kvp);

                                return (key != null)
                                    ? new KeyValuePair<string, string>(key, value)
                                    : default;
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

                const string suffix = ".migrated";
                string newPath = path + suffix;

                for (int i = 2; File.Exists(newPath); i++)
                {
                    newPath = path + suffix + i.ToString(CultureInfo.InvariantCulture);
                }

                if (!File.Exists(newPath))
                    File.Move(path, newPath);
            }

            static (string, string) MapRuleSetOptionToEditorConfigOption(KeyValuePair<string, ReportDiagnostic> kvp)
            {
                switch (kvp.Key)
                {
                    case "RCS0011i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.BlankLineBetweenSingleLineAccessors, "true")
                                : (ConfigOptionKeys.BlankLineBetweenSingleLineAccessors, "false");
                        }
                    case "RCS0015i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.BlankLineBetweenUsingDirectives, "never")
                                : (ConfigOptionKeys.BlankLineBetweenUsingDirectives, "separate_groups");
                        }
                    case "RCS0027i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.BinaryOperatorNewLine, "before")
                                : (ConfigOptionKeys.BinaryOperatorNewLine, "after");
                        }
                    case "RCS0028i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ConditionalOperatorNewLine, "before")
                                : (ConfigOptionKeys.ConditionalOperatorNewLine, "after");
                        }
                    case "RCS0032i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ArrowTokenNewLine, "before")
                                : (ConfigOptionKeys.ArrowTokenNewLine, "after");
                        }
                    case "RCS0051i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.NewLineBeforeWhileInDoStatement, "true")
                                : (ConfigOptionKeys.NewLineBeforeWhileInDoStatement, "false");
                        }
                    case "RCS0052i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.EqualsTokenNewLine, "before")
                                : (ConfigOptionKeys.EqualsTokenNewLine, "after");
                        }
                    case "RCS1014a":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit)
                                : (ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious);
                        }
                    case "RCS1014i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Explicit)
                                : (ConfigOptionKeys.ArrayCreationTypeStyle, ConfigOptionValues.ArrayCreationTypeStyle_Implicit);
                        }
                    case "RCS1016a":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.UseBlockBodyWhenExpressionSpansOverMultipleLines, "false")
                                : (ConfigOptionKeys.UseBlockBodyWhenExpressionSpansOverMultipleLines, "true");
                        }
                    case "RCS1016b":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.UseBlockBodyWhenDeclarationSpansOverMultipleLines, "false")
                                : (ConfigOptionKeys.UseBlockBodyWhenDeclarationSpansOverMultipleLines, "true");
                        }
                    case "RCS1016i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.BodyStyle, ConfigOptionValues.BodyStyle_Expression)
                                : (ConfigOptionKeys.BodyStyle, ConfigOptionValues.BodyStyle_Block);
                        }
                    case "RCS1018i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.AccessibilityModifiers, ConfigOptionValues.AccessibilityModifiers_Explicit)
                                : (ConfigOptionKeys.AccessibilityModifiers, ConfigOptionValues.AccessibilityModifiers_Implicit);
                        }
                    case "RCS1036a":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, "true")
                                : (ConfigOptionKeys.BlankLineBetweenClosingBraceAndSwitchSection, "false");
                        }
                    case "RCS1050i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Include)
                                : (ConfigOptionKeys.ObjectCreationParenthesesStyle, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit);
                        }
                    case "RCS1051a":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ConditionalOperatorConditionParenthesesStyle, ConfigOptionValues.ConditionalOperatorConditionParenthesesStyle_Include)
                                : (ConfigOptionKeys.ConditionalOperatorConditionParenthesesStyle, ConfigOptionValues.ConditionalOperatorConditionParenthesesStyle_OmitWhenConditionIsSingleToken);
                        }
                    case "RCS1078i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Literal)
                                : (ConfigOptionKeys.EmptyStringStyle, ConfigOptionValues.EmptyStringStyle_Field);
                        }
                    case "RCS1090i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.ConfigureAwait, "true")
                                : (ConfigOptionKeys.ConfigureAwait, "false");
                        }
                    case "RCS1096i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.EnumHasFlagStyle, ConfigOptionValues.EnumHasFlagStyle_Operator)
                                : (ConfigOptionKeys.EnumHasFlagStyle, ConfigOptionValues.EnumHasFlagStyle_Method);
                        }
                    case "RCS1207i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.UseAnonymousFunctionOrMethodGroup, ConfigOptionValues.UseAnonymousFunctionOrMethodGroup_MethodGroup)
                                : (ConfigOptionKeys.UseAnonymousFunctionOrMethodGroup, ConfigOptionValues.UseAnonymousFunctionOrMethodGroup_AnonymousFunction);
                        }
                    case "RCS1248i":
                        {
                            return (kvp.Value == ReportDiagnostic.Suppress)
                                ? (ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_PatternMatching)
                                : (ConfigOptionKeys.NullCheckStyle, ConfigOptionValues.NullCheckStyle_EqualityOperator);
                        }
                }

                Debug.Fail(kvp.Key);
                return default;
            }
        }
    }
}
