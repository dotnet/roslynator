// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Microsoft.VisualStudio.Shell.Interop;
using Roslynator.CodeFixes;
using Roslynator.Configuration;

#pragma warning disable RCS1090

namespace Roslynator.VisualStudio
{
    [ComVisible(true)]
    public class AbstractPackage : AsyncPackage
    {
        private FileSystemWatcher _watcher;

        internal static AbstractPackage Instance { get; private set; }

        private string SolutionDirectoryPath { get; set; }

        private string ConfigFilePath { get; set; }

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

        public GlobalSuppressionsOptionsPage GlobalSuppressionsOptionsPage
        {
            get { return (GlobalSuppressionsOptionsPage)GetDialogPage(typeof(GlobalSuppressionsOptionsPage)); }
        }

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Instance = this;

            await base.InitializeAsync(cancellationToken, progress);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            InitializeSettings();

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var solution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            ErrorHandler.ThrowOnFailure(solution.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object isSolutionOpenValue));

            if (isSolutionOpenValue is bool isSolutionOpen
                && isSolutionOpen)
            {
                AfterOpenSolution();
            }

            SolutionEvents.OnAfterOpenSolution += AfterOpenSolution;
            SolutionEvents.OnAfterCloseSolution += AfterCloseSolution;
        }

        public void InitializeSettings()
        {
            GeneralOptionsPage generalOptionsPage = GeneralOptionsPage;
            RefactoringsOptionsPage refactoringsOptionsPage = RefactoringsOptionsPage;
            CodeFixesOptionsPage codeFixesOptionsPage = CodeFixesOptionsPage;
            GlobalSuppressionsOptionsPage globalSuppressionsOptionsPage = GlobalSuppressionsOptionsPage;

            Version currentVersion = typeof(GeneralOptionsPage).Assembly.GetName().Version;

            if (!Version.TryParse(generalOptionsPage.ApplicationVersion, out Version version)
                || version < currentVersion)
            {
                generalOptionsPage.ApplicationVersion = currentVersion.ToString();
                generalOptionsPage.SaveSettingsToStorage();
            }

            codeFixesOptionsPage.CheckNewItemsDisabledByDefault();
            refactoringsOptionsPage.CheckNewItemsDisabledByDefault();
            globalSuppressionsOptionsPage.CheckNewItemsDisabledByDefault();

            SettingsManager.Instance.UpdateVisualStudioSettings(generalOptionsPage);
            SettingsManager.Instance.UpdateVisualStudioSettings(refactoringsOptionsPage);
            SettingsManager.Instance.UpdateVisualStudioSettings(codeFixesOptionsPage);
            SettingsManager.Instance.UpdateVisualStudioSettings(globalSuppressionsOptionsPage);
        }

        private void AfterOpenSolution(object sender = null, OpenSolutionEventArgs e = null)
        {
            var solution = GetService(typeof(SVsSolution)) as IVsSolution;

            if (solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionFileName, out object solutionFileNameValue) == VSConstants.S_OK
                && solutionFileNameValue is string solutionFileName
                && !string.IsNullOrEmpty(solutionFileName))
            {
                SolutionDirectoryPath = Path.GetDirectoryName(solutionFileName);
                ConfigFilePath = Path.Combine(SolutionDirectoryPath, Settings.ConfigFileName);
            }

            UpdateSettings();

            WatchConfigFile();
        }

        private void AfterCloseSolution(object sender = null, EventArgs e = null)
        {
            SolutionDirectoryPath = null;
            ConfigFilePath = null;

            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }

        private void UpdateSettings()
        {
            SettingsManager.Instance.ConfigFileSettings = LoadConfigFileSettings();
            SettingsManager.Instance.ApplyTo(RefactoringSettings.Current);
            SettingsManager.Instance.ApplyTo(CodeFixSettings.Current);
            SettingsManager.Instance.ApplyTo(AnalyzerSettings.Current);

            Settings LoadConfigFileSettings()
            {
                if (!File.Exists(ConfigFilePath))
                    return null;

                try
                {
                    return Settings.Load(ConfigFilePath);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (SecurityException)
                {
                }

                return null;
            }
        }

        public void WatchConfigFile()
        {
            if (!Directory.Exists(SolutionDirectoryPath))
                return;

            _watcher = new FileSystemWatcher(SolutionDirectoryPath, Settings.ConfigFileName)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
            };

            _watcher.Changed += (object sender, FileSystemEventArgs e) => UpdateSettings();
            _watcher.Created += (object sender, FileSystemEventArgs e) => UpdateSettings();
            _watcher.Deleted += (object sender, FileSystemEventArgs e) => UpdateSettings();
        }

        protected override void Dispose(bool disposing)
        {
            Instance = null;

            base.Dispose(disposing);
        }
    }
}
