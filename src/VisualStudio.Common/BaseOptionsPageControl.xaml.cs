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

        public string NameColumnHeaderText { get; set; } = "Id";

        public string TitleColumnHeaderText { get; set; } = "Title";

        public string CheckBoxColumnHeaderText { get; set; } = "Enabled";

        public string Comment { get; set; }

        public ListSortDirection DefaultSortDirection { get; set; }

        public ObservableCollection<BaseModel> Items { get; } = new ObservableCollection<BaseModel>();

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader clickedHeader))
                return;

            if (clickedHeader.Role == GridViewColumnHeaderRole.Padding)
                return;

            ListSortDirection direction;

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

            Sort(clickedHeader, direction);
        }

        private void Sort(GridViewColumnHeader columnHeader, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lsvItems.ItemsSource);
            SortDescriptionCollection sortDescriptions = dataView.SortDescriptions;

            sortDescriptions.Clear();

            if (columnHeader == checkBoxGridViewColumnHeader)
            {
                sortDescriptions.Add(new SortDescription("Enabled", direction));
                sortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
            }
            else
            {
                string propertyName = columnHeader.Content.ToString();

                if (propertyName != "Id")
                    propertyName = "Title";

                sortDescriptions.Add(new SortDescription(propertyName, direction));
            }

            dataView.Refresh();

            if (direction == ListSortDirection.Ascending)
            {
                columnHeader.Column.HeaderTemplate = Resources["gridViewHeaderArrowUpTemplate"] as DataTemplate;
            }
            else
            {
                columnHeader.Column.HeaderTemplate = Resources["gridViewHeaderArrowDownTemplate"] as DataTemplate;
            }

            if (_lastClickedHeader != null
                && _lastClickedHeader != columnHeader)
            {
                _lastClickedHeader.Column.HeaderTemplate = null;
            }

            _lastClickedHeader = columnHeader;
            _lastDirection = direction;
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

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_lastClickedHeader == null)
                Sort(checkBoxGridViewColumnHeader, DefaultSortDirection);
        }
    }
}
