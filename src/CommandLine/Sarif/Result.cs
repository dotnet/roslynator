using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Roslynator.CommandLine.Sarif;

internal class Result
{
    public string RuleId { get; set; }
    public Message Message { get; set; }
    public string Level { get; set; }
    public IList<Location> Locations { get; set; }
}
