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
        public GeneralOptionsPageControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

            VSPackage package = VSPackage.Instance;

            IEnumerable<KeyValuePair<string, bool>> refactorings = null;

            if (package.RefactoringsOptionsPage.IsLoaded)
            {
                refactorings = package.RefactoringsOptionsPage
                    .Control
                    .Items
                    .Where(f => f.Enabled.HasValue)
                    .Select(f => new KeyValuePair<string, bool>(f.Id, f.Enabled.Value));
            }
            else
            {
                refactorings = package.RefactoringsOptionsPage.GetItems();
            }

            IEnumerable<KeyValuePair<string, bool>> codeFixes = null;

            if (package.CodeFixesOptionsPage.IsLoaded)
            {
                codeFixes = package.CodeFixesOptionsPage
                    .Control
                    .Items
                    .Where(f => f.Enabled.HasValue)
                    .Select(f => new KeyValuePair<string, bool>(f.Id, f.Enabled.Value));
            }
            else
            {
                codeFixes = package.CodeFixesOptionsPage.GetItems();
            }

            var options = new Dictionary<string, string>();

            try
            {
                EditorConfigCodeAnalysisConfig.Save(
                    dialog.FileName,
                    options: options,
                    refactorings: refactorings,
                    codeFixes: codeFixes);
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

            VSPackage package = VSPackage.Instance;

            package.RefactoringsOptionsPage.Load();
            package.CodeFixesOptionsPage.Load();

            Update(package.RefactoringsOptionsPage, config.GetRefactorings());
            Update(package.CodeFixesOptionsPage, config.GetCodeFixes());

            static void Update(BaseOptionsPage optionsPage, IReadOnlyDictionary<string, bool> values)
            {
                foreach (BaseModel model in optionsPage.Control.Items)
                {
                    if (values.TryGetValue(model.Id, out bool enabled))
                    {
                        model.Enabled = enabled;
                    }
                    else
                    {
                        model.Enabled = null;
                    }
                }
            }
        }

        private static void ShowErrorMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OpenLocation_Click(object sender, RoutedEventArgs e)
        {
            string filePath = EditorConfigCodeAnalysisConfig.CreateDefaultConfigFileIfNotExists();

            if (File.Exists(filePath))
            {
                try
                {
                    Process.Start("explorer.exe", $"/select, \"{filePath}\"");
                }
                catch (Exception ex) when (ex is InvalidOperationException
                    || ex is FileNotFoundException
                    || ex is Win32Exception)
                {
                    MessageBox.Show(ex.Message, null, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            e.Handled = true;
        }
    }
}
