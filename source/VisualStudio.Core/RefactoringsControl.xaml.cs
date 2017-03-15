// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Roslynator.VisualStudio
{
    public partial class RefactoringsControl : UserControl
    {
        private GridViewColumnHeader _lastClickedHeader;
        private ListSortDirection _lastDirection;

        public RefactoringsControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public ObservableCollection<RefactoringModel> Refactorings { get; } = new ObservableCollection<RefactoringModel>();

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var clickedHeader = e.OriginalSource as GridViewColumnHeader;

            if (clickedHeader != null)
            {
                ListSortDirection direction;

                if (clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (clickedHeader != _lastClickedHeader)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var propertyName = clickedHeader.Column.Header as string;

                    Sort(propertyName, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        clickedHeader.Column.HeaderTemplate = Resources["gridViewHeaderArrowUpTemplate"] as DataTemplate;
                    }
                    else
                    {
                        clickedHeader.Column.HeaderTemplate = Resources["gridViewHeaderArrowDownTemplate"] as DataTemplate;
                    }

                    if (_lastClickedHeader != null
                        && _lastClickedHeader != clickedHeader)
                    {
                        _lastClickedHeader.Column.HeaderTemplate = null;
                    }

                    _lastClickedHeader = clickedHeader;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string propertyName, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lsvRefactorings.ItemsSource);

            var sortDescription = new SortDescription(propertyName, direction);

            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(sortDescription);
            dataView.Refresh();
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RefactoringModel refactoring in Refactorings)
                refactoring.Enabled = true;
        }

        private void DisableAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (RefactoringModel refactoring in Refactorings)
                refactoring.Enabled = false;
        }
    }
}
