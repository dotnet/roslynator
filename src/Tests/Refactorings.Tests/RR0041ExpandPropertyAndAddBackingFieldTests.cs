// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Xunit;

namespace Roslynator.CSharp.Refactorings.Tests
{
    public class RR0041ExpandPropertyAndAddBackingFieldTests : AbstractCSharpRefactoringVerifier
    {
        public override string RefactoringId { get; } = RefactoringIdentifiers.ExpandPropertyAndAddBackingField;

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExpandPropertyAndAddBackingField)]
        public async Task Test_Property()
        {
            await VerifyRefactoringAsync(@"
class C
{
    private string _value;

    public string [||]Value { get; set; } = null;
}
", @"
class C
{
    private string _value;
    private string _value2 = null;

    public string Value
    {
        get { return _value2; }
        set { _value2 = value; }
    }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExpandPropertyAndAddBackingField)]
        public async Task Test_ReadOnlyProperty()
        {
            await VerifyRefactoringAsync(@"
class C : B
{
    private string _value;

    public override string [||]Value { get; } = null;

    void M()
    {
        var x = base.Value;
        var y = Value;
    }
}

class B
{
    public virtual string Value { get; }
}
", @"
class C : B
{
    private string _value;
    private string _value2 = null;

    public override string Value
    {
        get { return _value2; }
    }

    void M()
    {
        var x = base.Value;
        var y = _value2;
    }
}

class B
{
    public virtual string Value { get; }
}
", equivalenceKey: RefactoringId);
        }

        [Fact, Trait(Traits.Refactoring, RefactoringIdentifiers.ExpandPropertyAndAddBackingField)]
        public async Task Test_Property_INotifyPropertyChanged()
        {
            await VerifyRefactoringAsync(@"
using System.ComponentModel;

class C : INotifyPropertyChanged
{
    public string [||]Value { get; set; } = null;

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
    private string _value = null;

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
    }
}
