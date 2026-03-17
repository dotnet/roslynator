using System;
using System.Collections.Generic;
using System.Text;

namespace Roslynator.CommandLine.Sarif;

internal class Region
{
    public int startLine { get; set; }
    public int endLine { get; set; }
    public int startColumn { get; set; }
    public int endColumn { get; set; }
}
