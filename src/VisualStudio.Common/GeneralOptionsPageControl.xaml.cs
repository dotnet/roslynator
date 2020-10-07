// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;
using Roslynator.Configuration;

namespace Roslynator.VisualStudio
{
    public partial class GeneralOptionsPageControl : UserControl, INotifyPropertyChanged
    {
        private bool _prefixFieldIdentifierWithUnderscore;
        private bool _useConfigFile;

        public GeneralOptionsPageControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool PrefixFieldIdentifierWithUnderscore
        {
            get { return _prefixFieldIdentifierWithUnderscore; }
            set
            {
                if (_prefixFieldIdentifierWithUnderscore != value)
                {
                    _prefixFieldIdentifierWithUnderscore = value;
                    OnPropertyChanged(nameof(PrefixFieldIdentifierWithUnderscore));
                }
            }
        }

        public bool UseConfigFile
        {
            get { return _useConfigFile; }
            set
            {
                if (_useConfigFile != value)
                {
                    _useConfigFile = value;
                    OnPropertyChanged(nameof(UseConfigFile));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void ExportOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "All Files  (*.*)|*.*|Config Files (*.config)|*.config",
                FileName = Path.GetFileName(CodeAnalysisConfiguration.ConfigFileName),
                DefaultExt = "config",
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = true,
            };

            if (dialog.ShowDialog() != true)
                return;

            AbstractPackage package = AbstractPackage.Instance;

            IEnumerable<string> disabledRefactorings = null;

            if (package.RefactoringsOptionsPage.IsLoaded)
            {
                disabledRefactorings = package.RefactoringsOptionsPage
                    .Control
                    .Items
                    .Where(f => !f.Enabled)
                    .Select(f => f.Id);
            }
            else
            {
                disabledRefactorings = package.RefactoringsOptionsPage.GetDisabledItems();
            }

            IEnumerable<string> disabledCodeFixes = null;

            if (package.CodeFixesOptionsPage.IsLoaded)
            {
                disabledCodeFixes = package.CodeFixesOptionsPage
                    .Control
                    .Items
                    .Where(f => !f.Enabled)
                    .Select(f => f.Id);
            }
            else
            {
                disabledCodeFixes = package.CodeFixesOptionsPage.GetDisabledItems();
            }

            var configuration = new CodeAnalysisConfiguration(
                refactorings: disabledRefactorings.Select(f => new KeyValuePair<string, bool>(f, false)),
                codeFixes: disabledCodeFixes.Select(f => new KeyValuePair<string, bool>(f, false)),
                prefixFieldIdentifierWithUnderscore: PrefixFieldIdentifierWithUnderscore);

            try
            {
                configuration.Save(dialog.FileName);
            }
            catch (Exception ex) when (ex is IOException
                || ex is UnauthorizedAccessException)
            {
                ShowErrorMessage(ex);
            }
        }

        private void ImportOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "All Files  (*.*)|*.*|Config Files (*.config)|*.config",
                CheckPathExists = true,
                Multiselect = false,
            };

            if (dialog.ShowDialog() != true)
                return;

            CodeAnalysisConfiguration configuration = CodeAnalysisConfiguration.LoadAndCatchIfThrows(dialog.FileName, f => ShowErrorMessage(f));

            AbstractPackage package = AbstractPackage.Instance;

            package.RefactoringsOptionsPage.Load();
            package.CodeFixesOptionsPage.Load();

            PrefixFieldIdentifierWithUnderscore = configuration.PrefixFieldIdentifierWithUnderscore;

            Update(package.RefactoringsOptionsPage, configuration.GetDisabledRefactorings().ToHashSet());
            Update(package.CodeFixesOptionsPage, configuration.GetDisabledCodeFixes().ToHashSet());

            static void Update(BaseOptionsPage optionsPage, HashSet<string> disabledIds)
            {
                foreach (BaseModel model in optionsPage.Control.Items)
                {
                    model.Enabled = !disabledIds.Contains(model.Id);
                }
            }
        }

        private static void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
