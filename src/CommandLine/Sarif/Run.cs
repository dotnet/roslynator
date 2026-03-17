using System;
using System.Collections.Generic;
using System.Text;

namespace Roslynator.CommandLine.Sarif;

internal class Run
{
    public Tool tool { get; set; }
    public Artifact[] artifacts { get; set; }
    public IList<Result> results { get; set; }
}
