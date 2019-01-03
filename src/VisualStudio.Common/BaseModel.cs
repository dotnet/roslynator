// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Diagnostics;

namespace Roslynator.VisualStudio
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Id} {Title} Enabled = {Enabled}";

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
