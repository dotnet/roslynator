// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Roslynator.CodeFixes;
using Roslynator.Configuration;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;

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

            string disabledRefactorings = refactoringsOptionsPage.DisabledRefactorings;

            if (!string.IsNullOrEmpty(disabledRefactorings))
            {
                disabledRefactorings = string.Join(",", disabledRefactorings.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(f => Regex.IsMatch(f, @"\ARR\d{4}\z")).Select(f => f + "!"));

                disabledRefactorings = "RR0001!,RR0002!";

                if (!string.IsNullOrEmpty(disabledRefactorings))
                {
                    string refactorings = refactoringsOptionsPage.Refactorings;

                    if (!string.IsNullOrEmpty(refactorings))
                        disabledRefactorings = "," + disabledRefactorings;

                    refactoringsOptionsPage.Refactorings = disabledRefactorings;
                }

                refactoringsOptionsPage.DisabledRefactorings = "";
            }

            string disabledCodeFixes = codeFixesOptionsPage.DisabledCodeFixes;

            if (!string.IsNullOrEmpty(disabledCodeFixes))
            {
                disabledCodeFixes = string.Join(",", disabledCodeFixes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(f => f + "!"));

                if (!string.IsNullOrEmpty(disabledCodeFixes))
                {
                    string codeFixes = codeFixesOptionsPage.CodeFixes;

                    if (!string.IsNullOrEmpty(codeFixes))
                        disabledCodeFixes = "," + disabledCodeFixes;

                    codeFixesOptionsPage.CodeFixes = disabledCodeFixes;
                }

                codeFixesOptionsPage.DisabledCodeFixes = "";
            }

            ConfigMigrator.MigrateToEditorConfig();

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
