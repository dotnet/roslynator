using System.Collections.Generic;

namespace Roslynator.CommandLine.Sarif;

internal class Run
{
    public Tool Tool { get; set; }
    public Artifact[] Artifacts { get; set; }
    public IList<Result> Results { get; set; }
}
