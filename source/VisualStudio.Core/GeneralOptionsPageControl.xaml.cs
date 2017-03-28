// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Windows.Controls;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
