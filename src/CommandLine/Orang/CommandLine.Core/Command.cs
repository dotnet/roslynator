// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Roslynator;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record Command
{
    public string Name { get; init; }

    public string Description { get; init; }

    public CommandGroup Group { get; init; }

    public ImmutableArray<CommandArgument> Arguments { get; init; }

    public ImmutableArray<CommandOption> Options { get; init; }

    public string ObsoleteMessage { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => Name + "  " + Description;
}
