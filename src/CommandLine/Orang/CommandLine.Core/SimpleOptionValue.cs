// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace Roslynator
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SimpleOptionValue : OptionValue
    {
        public SimpleOptionValue(
            string name,
            string value,
            string shortValue,
            string helpValue,
            string description = null,
            bool hidden = false,
            bool canContainExpression = false)
            : base(name, helpValue, description, hidden, canContainExpression)
        {
            Value = value;
            ShortValue = shortValue;
        }

        public override OptionValueKind Kind => OptionValueKind.Simple;

        public string Value { get; }

        public string ShortValue { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Kind}  {Name}  {HelpValue}";

        public bool IsValueOrShortValue(string value)
        {
            return value == Value || (ShortValue != null && value == ShortValue);
        }

        public static SimpleOptionValue Create<TEnum>(
            TEnum enumValue,
            string value = null,
            string shortValue = null,
            string helpValue = null,
            string description = null,
            bool hidden = false) where TEnum : struct
        {
            return Create(
                name: enumValue.ToString(),
                value: value,
                shortValue: shortValue,
                helpValue: helpValue,
                description: description,
                hidden: hidden);
        }

        public static SimpleOptionValue Create(
            string name,
            string value = null,
            string shortValue = null,
            string helpValue = null,
            string description = null,
            bool hidden = false,
            bool canContainExpression = false)
        {
            value ??= _lowerLetterUpperLetterRegex.Replace(name, e => e.Value.Insert(1, "-")).ToLowerInvariant();
            shortValue ??= value.Substring(0, 1);

            if (helpValue == null)
            {
                if (!string.IsNullOrEmpty(shortValue))
                {
                    if (string.CompareOrdinal(shortValue, 0, value, 0, shortValue.Length) == 0)
                    {
                        helpValue = $"{shortValue}[{value.Substring(shortValue.Length)}]";
                    }
                    else
                    {
                        helpValue = $"{shortValue} [{value}]";
                    }
                }
                else
                {
                    helpValue = value;
                }
            }

            return new SimpleOptionValue(
                name,
                value,
                shortValue,
                helpValue,
                description,
                hidden: hidden,
                canContainExpression: canContainExpression);
        }
    }
}
