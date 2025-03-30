using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.Formatting.CodeFixes.CSharp;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FixBracketFormattingOfListCodeFixProvider))]
[Shared]
public sealed class FixBracketFormattingOfListCodeFixProvider : BaseCodeFixProvider
{
    private const string Title = "Fix formatting";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIdentifiers.FixFormattingOfList);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

        if (!TryFindFirstAncestorOrSelf(
                root,
                context.Span,
                out SyntaxNode node,
                predicate: f =>
                {
                    switch (f.Kind())
                    {
                        case SyntaxKind.ParameterList:
                        case SyntaxKind.BracketedParameterList:
                        case SyntaxKind.TypeParameterList:
                        case SyntaxKind.ArgumentList:
                        case SyntaxKind.BracketedArgumentList:
                        case SyntaxKind.AttributeArgumentList:
                        case SyntaxKind.TypeArgumentList:
                        case SyntaxKind.AttributeList:
                        case SyntaxKind.BaseList:
                        case SyntaxKind.TupleType:
                        case SyntaxKind.TupleExpression:
                        case SyntaxKind.ArrayInitializerExpression:
                        case SyntaxKind.CollectionInitializerExpression:
                        case SyntaxKind.ComplexElementInitializerExpression:
                        case SyntaxKind.ObjectInitializerExpression:
                            return true;
                        default:
                            return false;
                    }
                }
            )
        )
        {
            return;
        }

        Document document = context.Document;
        Diagnostic diagnostic = context.Diagnostics[0];
        CodeAction codeAction = CreateCodeAction();

        context.RegisterCodeFix(codeAction, diagnostic);

        CodeAction CreateCodeAction()
        {
            switch (node)
            {
                case ParameterListSyntax parameterList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, parameterList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case BracketedParameterListSyntax bracketedParameterList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, bracketedParameterList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TypeParameterListSyntax typeParameterList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, typeParameterList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case ArgumentListSyntax argumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, argumentList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case BracketedArgumentListSyntax bracketedArgumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, bracketedArgumentList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case AttributeArgumentListSyntax attributeArgumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, attributeArgumentList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TypeArgumentListSyntax typeArgumentList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, typeArgumentList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case AttributeListSyntax attributeList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, attributeList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case BaseListSyntax baseList:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, baseList, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TupleTypeSyntax tupleType:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, tupleType, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case TupleExpressionSyntax tupleExpression:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, tupleExpression, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                case InitializerExpressionSyntax initializerExpression:
                {
                    return CodeAction.Create(
                        Title,
                        ct => CodeFixHelpers.FixListAsync(document, initializerExpression, ListFixMode.Fix, ct),
                        GetEquivalenceKey(diagnostic)
                    );
                }
                default:
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}