// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Roslynator.CodeFixes;
using Roslynator.Configuration;

namespace Roslynator
{
    internal sealed class Settings
    {
        private Settings()
        {
        }

        public bool UseConfigFile { get; set; }

        public static Settings Instance { get; } = new Settings();

        public CodeAnalysisConfiguration VisualStudio { get; set; } = new CodeAnalysisConfiguration();

        public CodeAnalysisConfiguration ConfigFile { get; set; }

        internal void ApplyTo(AnalyzerSettings analyzerSettings)
        {
            analyzerSettings.Reset(VisualStudio, (UseConfigFile) ? ConfigFile : null);
        }

        internal void ApplyTo(RefactoringSettings refactoringSettings)
        {
            refactoringSettings.Reset(VisualStudio, (UseConfigFile) ? ConfigFile : null);
        }

        public void ApplyTo(CodeFixSettings codeFixSettings)
        {
            codeFixSettings.Reset(VisualStudio, (UseConfigFile) ? ConfigFile : null);
        }
    }
}
