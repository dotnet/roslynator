// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
                Filter = "All Files  (*.*)|*.*",
                FileName = Path.GetFileName(EditorConfigCodeAnalysisConfig.FileName),
                DefaultExt = "editorconfig",
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

            var options = new Dictionary<string, string>()
            {
                [OptionKeys.PrefixFieldIdentifierWithUnderscore] = (PrefixFieldIdentifierWithUnderscore) ? "true" : "false"
            };

            try
            {
                EditorConfigCodeAnalysisConfig.Save(
                    dialog.FileName,
                    options: options,
                    refactorings: disabledRefactorings.Select(f => new KeyValuePair<string, bool>(f, false)),
                    codeFixes: disabledCodeFixes.Select(f => new KeyValuePair<string, bool>(f, false)));
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
                Filter = "All Files  (*.*)|*.*",
                CheckPathExists = true,
                Multiselect = false,
            };

            if (dialog.ShowDialog() != true)
                return;

            EditorConfigCodeAnalysisConfig config = EditorConfigCodeAnalysisConfigLoader.LoadAndCatchIfThrows(new string[] { dialog.FileName }, ex => ShowErrorMessage(ex));

            AbstractPackage package = AbstractPackage.Instance;

            package.RefactoringsOptionsPage.Load();
            package.CodeFixesOptionsPage.Load();

            PrefixFieldIdentifierWithUnderscore = config.PrefixFieldIdentifierWithUnderscore ?? OptionDefaultValues.PrefixFieldIdentifierWithUnderscore;

            Update(package.RefactoringsOptionsPage, config.GetDisabledRefactorings().ToHashSet());
            Update(package.CodeFixesOptionsPage, config.GetDisabledCodeFixes().ToHashSet());

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
