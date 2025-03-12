namespace Roslynator.CommandLine.GitLab;

internal sealed class GitLabIssueLocation
{
    public string Path { get; set; }
    public GitLabLocationLines Lines { get; set; }
}
