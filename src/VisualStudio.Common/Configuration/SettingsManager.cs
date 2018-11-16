// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Roslynator.CodeFixes;
using Roslynator.VisualStudio;

namespace Roslynator.Configuration
{
    public sealed class SettingsManager
    {
        private SettingsManager()
        {
        }

        public bool UseConfigFile { get; set; }

        public static SettingsManager Instance { get; } = new SettingsManager();

        public Settings VisualStudioSettings { get; } = new Settings();

        public ConfigFileSettings ConfigFileSettings { get; set; }

        public void UpdateVisualStudioSettings(GeneralOptionsPage generalOptionsPage)
        {
            UseConfigFile = generalOptionsPage.UseConfigFile;

            VisualStudioSettings.PrefixFieldIdentifierWithUnderscore = generalOptionsPage.PrefixFieldIdentifierWithUnderscore;
        }

        public void UpdateVisualStudioSettings(RefactoringsOptionsPage refactoringsOptionsPage)
        {
            UpdateVisualStudioSettings(VisualStudioSettings.Refactorings, refactoringsOptionsPage.GetDisabledItems());
        }

        public void UpdateVisualStudioSettings(CodeFixesOptionsPage codeFixOptionsPage)
        {
            UpdateVisualStudioSettings(VisualStudioSettings.CodeFixes, codeFixOptionsPage.GetDisabledItems());
        }

        public void UpdateVisualStudioSettings(AnalyzersOptionsPage analyzersOptionsPage)
        {
            UpdateVisualStudioSettings(VisualStudioSettings.Analyzers, analyzersOptionsPage.GetDisabledItems());
        }

        public static void UpdateVisualStudioSettings(Dictionary<string, bool> values, IEnumerable<string> disabledItems)
        {
            values.Clear();

            foreach (string id in disabledItems)
                values[id] = false;
        }

        internal void ApplyTo(RefactoringSettings settings)
        {
            settings.Reset();

            VisualStudioSettings.ApplyTo(settings);

            if (UseConfigFile)
                ConfigFileSettings?.ApplyTo(settings);
        }

        public void ApplyTo(CodeFixSettings settings)
        {
            settings.Reset();

            VisualStudioSettings.ApplyTo(settings);

            if (UseConfigFile)
                ConfigFileSettings?.ApplyTo(settings);
        }

        public void ApplyTo(AnalyzerSettings settings)
        {
            settings.Reset();

            VisualStudioSettings.ApplyTo(settings);

            if (UseConfigFile)
                ConfigFileSettings?.ApplyTo(settings);
        }
    }
}
