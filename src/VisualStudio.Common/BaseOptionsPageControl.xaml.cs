// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Roslynator.VisualStudio
{
    public partial class BaseOptionsPageControl : UserControl
    {
        private GridViewColumnHeader _lastClickedHeader;
        private ListSortDirection _lastDirection;

        public BaseOptionsPageControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public string Comment { get; set; }

        public ObservableCollection<BaseModel> Items { get; } = new ObservableCollection<BaseModel>();

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader clickedHeader)
            {
                ListSortDirection direction;

                if (clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (clickedHeader != _lastClickedHeader)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else if (_lastDirection == ListSortDirection.Ascending)
                    {
                        direction = ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = ListSortDirection.Ascending;
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
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lsvItems.ItemsSource);

            var sortDescription = new SortDescription(propertyName, direction);

            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(sortDescription);
            dataView.Refresh();
        }

        private bool FilterItems(object item)
        {
            string s = tbxFilter.Text;

            if (!string.IsNullOrWhiteSpace(s))
            {
                s = s.Trim();

                var model = (BaseModel)item;

                return model.Id.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) != -1
                    || model.Title.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) != -1;
            }

            return true;
        }

        private void UncheckAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (BaseModel model in Items)
                model.Enabled = false;
        }

        private void CheckAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (BaseModel model in Items)
                model.Enabled = true;
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var view = (CollectionView)CollectionViewSource.GetDefaultView(lsvItems.ItemsSource);

            view.Filter = view.Filter ?? FilterItems;

            view.Refresh();
        }
    }
}
