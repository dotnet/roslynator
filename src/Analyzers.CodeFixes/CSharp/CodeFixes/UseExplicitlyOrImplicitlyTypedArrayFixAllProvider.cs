using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Roslynator.CSharp.CodeFixes;

namespace Roslynator.CSharp.CSharp.CodeFixes;

public class UseExplicitlyOrImplicitlyTypedArrayFixAllProvider : DocumentBasedFixAllProvider
{
    public static readonly UseExplicitlyOrImplicitlyTypedArrayFixAllProvider Instance = new();
    private UseExplicitlyOrImplicitlyTypedArrayFixAllProvider() { }

    protected override async Task<Document> FixAllAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
    {
        var codeFixProvider = (UseExplicitlyOrImplicitlyTypedArrayCodeFixProvider)fixAllContext.CodeFixProvider;
        foreach (var diag in diagnostics.OrderByDescending(d => d.Location.SourceSpan.Start))
        {
            document = await codeFixProvider.ApplyFixToDocument(document, diag, fixAllContext.CancellationToken);
        }

        return document;
    }
}
