// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.Configuration;

namespace Roslynator
{
    internal class DiagnosticDescriptorFactory
    {
        private const string IdSuffix = "FadeOut";
        private const string HelpLinkUriRoot = "http://pihrt.net/roslynator/analyzer?id=";

        private readonly string _path;
        private RuleSet _ruleSet;

        private RuleSet RuleSet
        {
            get
            {
                if (_ruleSet == null)
                    Interlocked.CompareExchange(ref _ruleSet, LoadRuleSet(), null);

                return _ruleSet;

                RuleSet LoadRuleSet()
                {
                    return RuleSetUtility.Load(_path, CodeAnalysisConfiguration.Default.RuleSets) ?? RuleSetUtility.EmptyRuleSet;
                }
            }
        }

        public DiagnosticDescriptorFactory(string path)
        {
            _path = path;
        }

        internal static DiagnosticDescriptorFactory CreateFromAssemblyLocation(string assemblyLocation)
        {
            string path = null;

            if (!string.IsNullOrEmpty(assemblyLocation))
                path = Path.Combine(Path.GetDirectoryName(assemblyLocation), RuleSetUtility.DefaultRuleSetName);

            return new DiagnosticDescriptorFactory(path);
        }

        public DiagnosticDescriptor Create(
            string id,
            string title,
            string messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault,
            string description = null,
            string helpLinkUri = null,
            params string[] customTags)
        {
            return new DiagnosticDescriptor(
                id: id,
                title: title,
                messageFormat: messageFormat,
                category: category,
                defaultSeverity: RuleSet.GetDiagnosticSeverityOrDefault(id, defaultSeverity),
                isEnabledByDefault: RuleSet.IsDiagnosticEnabledOrDefault(id, isEnabledByDefault),
                description: description,
                helpLinkUri: HelpLinkUriRoot + helpLinkUri,
                customTags: customTags);
        }

        public static DiagnosticDescriptor CreateFadeOut(DiagnosticDescriptor descriptor)
        {
            return new DiagnosticDescriptor(
                descriptor.Id + IdSuffix,
                descriptor.Title,
                descriptor.MessageFormat,
                DiagnosticCategories.FadeOut,
                DiagnosticSeverity.Hidden,
                isEnabledByDefault: true,
                customTags: new string[] { WellKnownDiagnosticTags.Unnecessary, WellKnownDiagnosticTags.NotConfigurable });
        }
    }
}
