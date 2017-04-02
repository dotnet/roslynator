// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CSharp.Refactorings;
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

        public ConfigFileSettings ConfigFileSettings { get; } = new ConfigFileSettings();

        public void UpdateVisualStudioSettings(GeneralOptionsPage generalOptionsPage)
        {
            UseConfigFile = generalOptionsPage.UseConfigFile;

            VisualStudioSettings.PrefixFieldIdentifierWithUnderscore = generalOptionsPage.PrefixFieldIdentifierWithUnderscore;
        }

        public void UpdateVisualStudioSettings(RefactoringsOptionsPage refactoringsOptionsPage)
        {
            VisualStudioSettings.Refactorings.Clear();

            foreach (string id in refactoringsOptionsPage.GetDisabledRefactorings())
                VisualStudioSettings.Refactorings[id] = false;
        }

        public void ApplyTo(RefactoringSettings settings)
        {
            settings.Reset();

            VisualStudioSettings.ApplyTo(settings);

            if (UseConfigFile)
                ConfigFileSettings.ApplyTo(settings);
        }
    }
}
