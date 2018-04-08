// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Security;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Roslynator.CodeFixes;
using Roslynator.Configuration;

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
    public sealed partial class VSPackage : Package, IVsSolutionEvents
    {
        private uint _cookie;
        private FileSystemWatcher _watcher;

        public VSPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            InitializeSettings();

            var solution = GetService(typeof(SVsSolution)) as IVsSolution;

            solution?.AdviseSolutionEvents(this, out _cookie);
        }

        private void InitializeSettings()
        {
            var generalOptionsPage = (GeneralOptionsPage)GetDialogPage(typeof(GeneralOptionsPage));
            var refactoringsOptionsPage = (RefactoringsOptionsPage)GetDialogPage(typeof(RefactoringsOptionsPage));
            var codeFixesOptionsPage = (CodeFixesOptionsPage)GetDialogPage(typeof(CodeFixesOptionsPage));

            Version currentVersion = typeof(GeneralOptionsPage).Assembly.GetName().Version;

            if (!Version.TryParse(generalOptionsPage.ApplicationVersion, out Version version)
                || version < currentVersion)
            {
                generalOptionsPage.ApplicationVersion = currentVersion.ToString();
                generalOptionsPage.SaveSettingsToStorage();
            }

            codeFixesOptionsPage.CheckNewItemsDisabledByDefault();
            refactoringsOptionsPage.CheckNewItemsDisabledByDefault();

            SettingsManager.Instance.UpdateVisualStudioSettings(generalOptionsPage);
            SettingsManager.Instance.UpdateVisualStudioSettings(refactoringsOptionsPage);
            SettingsManager.Instance.UpdateVisualStudioSettings(codeFixesOptionsPage);
        }

        private void UpdateSettingsAfterConfigFileChanged()
        {
            SettingsManager.Instance.ConfigFileSettings = LoadConfigFileSettings();
            SettingsManager.Instance.ApplyTo(RefactoringSettings.Current);
            SettingsManager.Instance.ApplyTo(CodeFixSettings.Current);
        }

        private ConfigFileSettings LoadConfigFileSettings()
        {
            if (GetService(typeof(DTE)) is DTE dte)
            {
                string path = dte.Solution.FullName;

                if (!string.IsNullOrEmpty(path))
                {
                    string directoryPath = Path.GetDirectoryName(path);

                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        path = Path.Combine(directoryPath, ConfigFileSettings.FileName);

                        if (File.Exists(path))
                        {
                            try
                            {
                                return ConfigFileSettings.Load(path);
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
                        }
                    }
                }
            }

            return default(ConfigFileSettings);
        }

        private void WatchConfigFile()
        {
            if (GetService(typeof(DTE)) is DTE dte)
            {
                string path = dte.Solution.FullName;

                if (!string.IsNullOrEmpty(path))
                {
                    string directoryPath = Path.GetDirectoryName(path);

                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        _watcher = new FileSystemWatcher(directoryPath, ConfigFileSettings.FileName)
                        {
                            EnableRaisingEvents = true,
                            IncludeSubdirectories = false
                        };

                        _watcher.Changed += (object sender, FileSystemEventArgs e) => UpdateSettingsAfterConfigFileChanged();
                        _watcher.Created += (object sender, FileSystemEventArgs e) => UpdateSettingsAfterConfigFileChanged();
                        _watcher.Deleted += (object sender, FileSystemEventArgs e) => UpdateSettingsAfterConfigFileChanged();
                    }
                }
            }
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            UpdateSettingsAfterConfigFileChanged();

            WatchConfigFile();

            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }

            return VSConstants.S_OK;
        }
    }
}
