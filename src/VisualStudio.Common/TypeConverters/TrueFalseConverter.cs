// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Globalization;

namespace Roslynator.VisualStudio.TypeConverters
{
    public abstract class TrueFalseConverter : BooleanConverter
    {
        public abstract string TrueText { get; }

        public abstract string FalseText { get; }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is bool
                && destinationType == typeof(string))
            {
                return ((bool)value) ? TrueText : FalseText;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                if (string.Equals(s, TrueText, StringComparison.Ordinal))
                    return true;

                if (string.Equals(s, FalseText, StringComparison.Ordinal))
                    return false;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
