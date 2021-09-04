// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CommandLine.Help
{
    public class CommandItem : HelpItem
    {
        public CommandItem(Command command, string syntax, string description) : base(syntax, description)
        {
            Command = command;
        }

        public Command Command { get; }
    }
}
