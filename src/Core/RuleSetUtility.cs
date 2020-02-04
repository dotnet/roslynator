// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;

namespace Roslynator
{
    internal static class RuleSetUtility
    {
        private static RuleSet _emptyRuleSet;

        internal const string DefaultRuleSetName = "roslynator.ruleset";

        public static RuleSet EmptyRuleSet
        {
            get
            {
                if (_emptyRuleSet == null)
                    Interlocked.CompareExchange(ref _emptyRuleSet, CreateEmptyRuleSet(), null);

                return _emptyRuleSet;

                static RuleSet CreateEmptyRuleSet()
                {
                    return new RuleSet(
                        filePath: "",
                        generalOption: ReportDiagnostic.Default,
                        specificOptions: ImmutableDictionary<string, ReportDiagnostic>.Empty,
                        includes: ImmutableArray<RuleSetInclude>.Empty);
                }
            }
        }

        public static RuleSet Load(string path, ImmutableArray<string> additionalPaths)
        {
            RuleSet ruleSet = Load(path);

            foreach (string ruleSetPath in additionalPaths)
            {
                RuleSet ruleSet2 = Load(ruleSetPath);

                ruleSet = Combine(ruleSet, ruleSet2);
            }

            return ruleSet;
        }

        private static RuleSet Load(string path)
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
                    catch (Exception ex) when (ex is IOException
                        || ex is UnauthorizedAccessException
                        || ex.GetType().FullName == "Microsoft.CodeAnalysis.InvalidRuleSetException")
                    {
#if DEBUG
                        sw.WriteLine($"{DateTime.Now.ToString()} {ex}");
                        throw;
#endif
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

        public static RuleSet Combine(RuleSet ruleSet, RuleSet parent)
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
    }
}