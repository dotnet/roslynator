namespace Roslynator.CommandLine.Sarif;

internal class Location
{
    public PhysicalLocation physicalLocation { get; set; }
    public LogicalLocation[] logicalLocations { get; set; }
}
