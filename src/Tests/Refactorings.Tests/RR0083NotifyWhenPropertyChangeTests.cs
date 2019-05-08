// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0083NotifyWhenPropertyChangeTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.NotifyWhenPropertyChange;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_SetterWithBody()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get { return _value; }
        [||]set { _value = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", @"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get { return _value; }

        set
        {
            if (_value != value)
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_SetterWithExpressionBody()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get => _value;
        [||]set => _value = value;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", @"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get => _value;

        set
        {
            if (_value != value)
            {
                _value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_OnPropertyChanged()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get { return _value; }
        [||]set { _value = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", @"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get { return _value; }

        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_GenerateDefaultOnPropertyChangedMethod()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;

namespace N
{
    class C : INotifyPropertyChanged
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            [||]set { _value = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
", @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace N
{
    class C : INotifyPropertyChanged
    {
        private string _value;

        public string Value
        {
            get { return _value; }

            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_CantModifyWithoutAccessiblePropertyChangedOrNotifyMethod()
        {
            await VerifyNoRefactoringAsync(@"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
}

class D : C
{
    private string _value;

    public string Value
    {
        get { return _value; }
        [||]set { _value = value; }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_NamespacesAndOuterClassesShouldWork()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;

namespace L
{
    namespace N.M
    {
        class O
        {
            class C : INotifyPropertyChanged
            {
                private string _value;

                public string Value
                {
                    get { return _value; }
                    [||]set { _value = value; }
                }

                public event PropertyChangedEventHandler PropertyChanged;
            }
        }
    }
}
", @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace L
{
    namespace N.M
    {
        class O
        {
            class C : INotifyPropertyChanged
            {
                private string _value;

                public string Value
                {
                    get { return _value; }

                    set
                    {
                        if (_value != value)
                        {
                            _value = value;
                            OnPropertyChanged();
                        }
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;

                protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.NotifyWhenPropertyChange)]
        public async Task Test_UsesCallerMemberNameWhenAvailable()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;
using System.Runtime.CompilerServices;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get { return _value; }
        [||]set { _value = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", @"
using System.ComponentModel;
using System.Runtime.CompilerServices;

class C : INotifyPropertyChanged
{
    private string _value;

    public string Value
    {
        get { return _value; }

        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}
", equivalenceKey: RefactoringId);
        }
    }
}
