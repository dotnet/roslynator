using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Roslynator.CommandLine.Sarif;
using Roslynator.Diagnostics;

namespace Roslynator.CommandLine.Json;

internal class DiagnosticSarifJsonSerializer
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Newtonsoft.Json.Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
    };

    public static void Serialize(
        IEnumerable<ProjectAnalysisResult> results,
        string filePath)
    {
        var rootObject = new RootObject();
        var run = new Run();
        run.results = new List<Result>();
        foreach (var result in results)
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                run.results.Add(new Result()
                {
                    ruleId = diagnostic.Descriptor.Id,
                    message = new Message() { text = diagnostic.Descriptor.Title.ToString() },
                    locations = new List<Location>()
                    {
                        new Location(){
                            physicalLocation = new PhysicalLocation()
                            {
                                artifactLocation = new ArtifactLocation()
                                {
                                    uri = $"file://{diagnostic.LineSpan.Path}"
                                },
                                region = new Region()
                                {
                                    startLine = diagnostic.LineSpan.StartLinePosition.Line,
                                    startColumn = diagnostic.LineSpan.StartLinePosition.Character,
                                    endLine = diagnostic.LineSpan.EndLinePosition.Line,
                                    endColumn = diagnostic.LineSpan.EndLinePosition.Character
                                },
                            },
                        },
                    },
                });
            }
        }
        run.tool = new Tool()
        {
            driver = new ToolComponent()
        };
        rootObject.runs = new Run[] { run };
        string report = JsonConvert.SerializeObject(rootObject, _jsonSerializerSettings);

        File.WriteAllText(filePath, report, Encoding.UTF8);
    }
}
