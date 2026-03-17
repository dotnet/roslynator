using Newtonsoft.Json;

namespace Roslynator.CommandLine.Sarif;

internal class RootObject
{
    public string version => "2.1.0";
    [JsonProperty("$schema")]
    public string schema => "https://docs.oasis-open.org/sarif/sarif/v2.1.0/errata01/os/schemas/sarif-schema-2.1.0.json";
    public Run[] runs { get; set; }
}
