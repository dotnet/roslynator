// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine.Help
{
    public class ArgumentItem : HelpItem
    {
        public ArgumentItem(CommandArgument argument, string syntax, string description) : base(syntax, description)
        {
            Argument = argument;
        }

        public CommandArgument Argument { get; }

        public bool IsRequired => Argument.IsRequired;

        public string Name => Argument.Name;
    }
}
