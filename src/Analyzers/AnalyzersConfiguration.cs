// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.Configuration;

namespace Roslynator.CSharp
{
    internal static class AnalyzersConfiguration
    {
        private static RuleSet _ruleSet;

        private static RuleSet RuleSet
        {
            get
            {
                if (_ruleSet == null)
                    Interlocked.CompareExchange(ref _ruleSet, LoadRuleSet() ?? CreateEmptyRuleSet(), null);

                return _ruleSet;

                RuleSet CreateEmptyRuleSet()
                {
                    return new RuleSet(
                        filePath: "",
                        generalOption: ReportDiagnostic.Default,
                        specificOptions: ImmutableDictionary<string, ReportDiagnostic>.Empty,
                        includes: ImmutableArray<RuleSetInclude>.Empty);
                }
            }
        }

        private static RuleSet LoadRuleSet()
        {
            RuleSet ruleSet = null;

            string assemblyPath = typeof(AnalyzersConfiguration).Assembly.Location;

            if (!string.IsNullOrEmpty(assemblyPath))
            {
                string ruleSetPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "roslynator.ruleset");

                ruleSet = LoadRuleSet(ruleSetPath);
            }

            foreach (string ruleSetPath in CodeAnalysisConfiguration.Current.RuleSets)
            {
                RuleSet ruleSet2 = LoadRuleSet(ruleSetPath);

                ruleSet = Combine(ruleSet, ruleSet2);
            }

            return ruleSet;
        }

        private static RuleSet LoadRuleSet(string path)
        {
            if (!File.Exists(path))
                return null;
#if DEBUG
            using (StreamWriter sw = File.CreateText(path + ".log"))
            {
                sw.WriteLine($"{DateTime.Now.ToString()} loading rule set from {path}");
                sw.WriteLine($"{DateTime.Now.ToString()} base directory {AppContext.BaseDirectory}");
                sw.WriteLine($"{DateTime.Now.ToString()} current directory {Environment.CurrentDirectory}");
#endif
                if (File.Exists(path))
                {
                    RuleSet ruleSet = null;

                    try
                    {
                        ruleSet = RuleSet.LoadEffectiveRuleSetFromFile(path);
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException
                            || ex is UnauthorizedAccessException
                            || ex.GetType().FullName == "Microsoft.CodeAnalysis.InvalidRuleSetException")
                        {
#if DEBUG
                            sw.WriteLine($"{DateTime.Now.ToString()} {ex}");
                            throw;
#endif
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (ruleSet != null)
                    {
#if DEBUG
                        sw.WriteLine($"{DateTime.Now.ToString()} rule set loaded");
                        sw.WriteLine($"{DateTime.Now.ToString()} default action is {ruleSet.GeneralDiagnosticOption}");

                        foreach (KeyValuePair<string, ReportDiagnostic> kvp in ruleSet.SpecificDiagnosticOptions)
                            sw.WriteLine($"{DateTime.Now.ToString()} {kvp.Key} {kvp.Value}");
#endif
                        return ruleSet;
                    }
                }
#if DEBUG
                else
                {
                    sw.WriteLine($"{DateTime.Now.ToString()} rule set not found");
                }
            }
#endif
            return null;
        }

        private static RuleSet Combine(RuleSet ruleSet, RuleSet parent)
        {
            if (ruleSet == null)
                return parent;

            if (parent == null)
                return ruleSet;

            ReportDiagnostic newGeneralOption = (IsStricterThan(ruleSet.GeneralDiagnosticOption, parent.GeneralDiagnosticOption))
                ? ruleSet.GeneralDiagnosticOption
                : parent.GeneralDiagnosticOption;

            ImmutableDictionary<string, ReportDiagnostic>.Builder newSpecificOptions = parent.SpecificDiagnosticOptions.ToBuilder();

            foreach (KeyValuePair<string, ReportDiagnostic> kvp in ruleSet.SpecificDiagnosticOptions)
            {
                if (!parent.SpecificDiagnosticOptions.ContainsKey(kvp.Key))
                    newSpecificOptions[kvp.Key] = kvp.Value;
            }

            return new RuleSet(
                filePath: parent.FilePath,
                generalOption: newGeneralOption,
                specificOptions: newSpecificOptions.ToImmutable(),
                includes: parent.Includes);
        }

        private static bool IsStricterThan(ReportDiagnostic action1, ReportDiagnostic action2)
        {
            switch (action2)
            {
                case ReportDiagnostic.Suppress:
                    {
                        return true;
                    }
                case ReportDiagnostic.Default:
                    {
                        return action1 == ReportDiagnostic.Warn
                            || action1 == ReportDiagnostic.Error
                            || action1 == ReportDiagnostic.Info
                            || action1 == ReportDiagnostic.Hidden;
                    }
                case ReportDiagnostic.Hidden:
                    {
                        return action1 == ReportDiagnostic.Warn
                            || action1 == ReportDiagnostic.Error
                            || action1 == ReportDiagnostic.Info;
                    }
                case ReportDiagnostic.Info:
                    {
                        return action1 == ReportDiagnostic.Warn
                            || action1 == ReportDiagnostic.Error;
                    }
                case ReportDiagnostic.Warn:
                    {
                        return action1 == ReportDiagnostic.Error;
                    }
                case ReportDiagnostic.Error:
                    {
                        return false;
                    }
            }

            return false;
        }

        public static DiagnosticSeverity GetDiagnosticSeverityOrDefault(string id, DiagnosticSeverity defaultValue)
        {
            if (!RuleSet.SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
                reportDiagnostic = RuleSet.GeneralDiagnosticOption;

            if (reportDiagnostic != ReportDiagnostic.Default
                && reportDiagnostic != ReportDiagnostic.Suppress)
            {
                return reportDiagnostic.ToDiagnosticSeverity();
            }

            return defaultValue;
        }

        public static bool IsDiagnosticEnabledOrDefault(string id, bool defaultValue)
        {
            if (RuleSet.SpecificDiagnosticOptions.TryGetValue(id, out ReportDiagnostic reportDiagnostic))
            {
                if (reportDiagnostic != ReportDiagnostic.Default)
                    return reportDiagnostic != ReportDiagnostic.Suppress;
            }
            else if (RuleSet.GeneralDiagnosticOption == ReportDiagnostic.Suppress)
            {
                return false;
            }

            return defaultValue;
        }
    }
}