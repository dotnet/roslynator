using Newtonsoft.Json;

namespace Roslynator.CommandLine.Sarif;

internal class RootObject
{
    [JsonProperty("version")]
    public static string Version => "2.1.0";
    [JsonProperty("$schema")]
    public static string Schema => "https://docs.oasis-open.org/sarif/sarif/v2.1.0/errata01/os/schemas/sarif-schema-2.1.0.json";
    public Run[] Runs { get; set; }
}
