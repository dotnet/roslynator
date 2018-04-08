// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;

namespace Roslynator.VisualStudio
{
    public class BaseModel : INotifyPropertyChanged
    {
        private bool _enabled;

        public BaseModel(string id, string title, bool enabled)
        {
            Id = id;
            Title = title;
            Enabled = enabled;
        }

        public string Id { get; }

        public string Title { get; }

        public bool Enabled
        {
            get { return _enabled; }

            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
