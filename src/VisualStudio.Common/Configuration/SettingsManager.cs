// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CodeFixes;
using Roslynator.VisualStudio;

namespace Roslynator.Configuration
{
    public class SettingsManager
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
            VisualStudioSettings.Refactorings.Clear();

            foreach (string id in refactoringsOptionsPage.GetDisabledItems())
                VisualStudioSettings.Refactorings[id] = false;
        }

        public void UpdateVisualStudioSettings(CodeFixesOptionsPage codeFixOptionsPage)
        {
            VisualStudioSettings.CodeFixes.Clear();

            foreach (string id in codeFixOptionsPage.GetDisabledItems())
                VisualStudioSettings.CodeFixes[id] = false;
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
    }
}
