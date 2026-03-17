using System;
using System.Collections.Generic;
using System.Text;

namespace Roslynator.CommandLine.Sarif;

internal class PhysicalLocation
{
    public ArtifactLocation artifactLocation { get; set; }
    public Region region { get; set; }
}
