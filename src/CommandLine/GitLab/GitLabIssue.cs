using Newtonsoft.Json;

namespace Roslynator.CommandLine.GitLab
{
    internal sealed class GitLabIssue
    {
        public string Type { get; set; }
        public string Fingerprint { get; set; }
        [JsonProperty("check_name")]
        public string CheckName { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public GitLabIssueLocation Location { get; set; }
        public string[] Categories { get; set; }
    }
}