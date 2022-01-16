// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Roslynator.Configuration;

#pragma warning disable RCS1090

namespace Roslynator.VisualStudio
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("7AD86013-9E55-4BBE-98CC-DE72FADAB1E6")]
    [ProvideOptionPage(typeof(GeneralOptionsPage), "Roslynator", "General", 0, 0, true, Sort = 0)]
    [ProvideOptionPage(typeof(RefactoringsOptionsPage), "Roslynator", "Refactorings", 0, 0, true, Sort = 2)]
    [ProvideOptionPage(typeof(CodeFixesOptionsPage), "Roslynator", "Code Fixes", 0, 0, true, Sort = 3)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ComVisible(true)]
    public sealed class VSPackage : AsyncPackage
    {
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;

            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            InitializeConfig();

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        internal static VSPackage Instance { get; private set; }

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
                disabledRefactorings = string.Join(",", disabledRefactorings.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Where(f => System.Text.RegularExpressions.Regex.IsMatch(f, @"\ARR\d{4}\z")).Select(f => f + "!"));

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
