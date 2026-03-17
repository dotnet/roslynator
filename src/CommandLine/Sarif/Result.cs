using System.Collections.Generic;

namespace Roslynator.CommandLine.Sarif;

internal class Result
{
    public string ruleId { get; set; }
    public Message message { get; set; }
    public IList<Location> locations { get; set; }
}
