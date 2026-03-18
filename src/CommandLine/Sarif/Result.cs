using System.Collections.Generic;

namespace Roslynator.CommandLine.Sarif;

internal class Result
{
    public string RuleId { get; set; }
    public Message Message { get; set; }
    public string Level { get; set; }
    public IList<Location> Locations { get; set; }
}
