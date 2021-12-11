// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Roslynator.Configuration;

#pragma warning disable RCS1090

namespace Roslynator.VisualStudio
{
    [ComVisible(true)]
    public class AbstractPackage : AsyncPackage
    {
        internal static AbstractPackage Instance { get; private set; }

        public GeneralOptionsPage GeneralOptionsPage
        {
            get { return (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage)); }
        }

        public RefactoringsOptionsPage RefactoringsOptionsPage
        {
            get { return (RefactoringsOptionsPage)GetDialogPage(typeof(RefactoringsOptionsPage)); }
        }

        public CodeFixesOptionsPage CodeFixesOptionsPage
        {
            get { return (CodeFixesOptionsPage)GetDialogPage(typeof(CodeFixesOptionsPage)); }
        }

        public AnalyzersOptionsPage AnalyzersOptionsPage
        {
            get { return (AnalyzersOptionsPage)GetDialogPage(typeof(AnalyzersOptionsPage)); }
        }

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;

            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            InitializeConfig();

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        public void InitializeConfig()
        {
            GeneralOptionsPage generalOptionsPage = GeneralOptionsPage;
            RefactoringsOptionsPage refactoringsOptionsPage = RefactoringsOptionsPage;
            CodeFixesOptionsPage codeFixesOptionsPage = CodeFixesOptionsPage;

            Version currentVersion = typeof(GeneralOptionsPage).Assembly.GetName().Version;

            if (!Version.TryParse(generalOptionsPage.ApplicationVersion, out Version version)
                || version < currentVersion)
            {
                generalOptionsPage.ApplicationVersion = currentVersion.ToString();
                generalOptionsPage.SaveSettingsToStorage();
            }

            ConfigMigrator.MigrateToEditorConfig();

            refactoringsOptionsPage.CheckNewItemsDisabledByDefault(CodeAnalysisConfig.Instance.GetDisabledRefactorings());
            codeFixesOptionsPage.CheckNewItemsDisabledByDefault(CodeAnalysisConfig.Instance.GetDisabledCodeFixes());

            generalOptionsPage.UpdateConfig();
            refactoringsOptionsPage.UpdateConfig();
            codeFixesOptionsPage.UpdateConfig();
        }

        protected override void Dispose(bool disposing)
        {
            Instance = null;

            base.Dispose(disposing);
        }
    }
}
