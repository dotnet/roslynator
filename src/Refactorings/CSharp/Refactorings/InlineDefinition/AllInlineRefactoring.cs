using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.FindSymbols;
using Roslynator.CSharp.Refactorings.InlineDefinition;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.CSharp.Refactorings.InlineDefinition
{
    internal abstract class AllInlineRefactoring<TNode, TDeclaration, TSymbol>
        where TNode : SyntaxNode
        where TDeclaration : MemberDeclarationSyntax
        where TSymbol : ISymbol
    {
        protected AllInlineRefactoring(
            TSymbol symbol,
            TDeclaration declaration,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
        {
            Symbol = symbol;
            Declaration = declaration;
            DeclarationSemanticModel = declarationSemanticModel;
            CancellationToken = cancellationToken;
        }

        public TSymbol Symbol { get; }

        public TDeclaration Declaration { get; }

        public SemanticModel DeclarationSemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        public abstract SyntaxNode BodyOrExpressionBody { get; }

        public abstract ImmutableArray<ITypeSymbol> TypeArguments { get; }

        public abstract ExpressionSyntax DeclarationExpression { get; }

        public abstract SyntaxList<StatementSyntax> DeclarationStatements { get; }

        public virtual async Task<Solution> InlineAndRemoveAsync(
            Solution solution,
            CancellationToken cancellationToken = default)
        {
            bool allRemoved = true;
            IEnumerable<SymbolCallerInfo> callerSymbols =
                await SymbolFinder.FindCallersAsync(Symbol, solution, cancellationToken).ConfigureAwait(false);
            IEnumerable<IGrouping<SyntaxTree, Location>> updatableLocations =
                callerSymbols.Where(s =>
                {
                    allRemoved &= s.IsDirect;
                    return s.IsDirect;
                })
                .SelectMany(s => s.Locations)
                .Where(l =>
                {
                    allRemoved &= l.IsInSource;
                    return l.IsInSource;
                })
                .GroupBy(l => l.SourceTree);
            IList<Task<(bool, DocumentEditor)>> documentUpdateTasks = new List<Task<(bool, DocumentEditor)>>();
            foreach (IGrouping<SyntaxTree, Location> locationGrouping in updatableLocations)
            {
                Document document = solution.GetDocument(locationGrouping.Key);
                documentUpdateTasks.Add(GetDocumentEditsAsync(document, locationGrouping, cancellationToken));
            }

            Document declarationDocument = solution.GetDocument(Declaration.SyntaxTree);
            Solution newSolution = solution;
            (bool, DocumentEditor)[] documentEditors = await Task.WhenAll(documentUpdateTasks).ConfigureAwait(false);
            DocumentEditor declarationEditor = null;
            foreach(var (allRemovedFromDocument, editor) in documentEditors)
            {
                allRemoved &= allRemovedFromDocument;
                if (editor.OriginalDocument == declarationDocument)
                {
                    declarationEditor = editor;
                }
                else
                {
                    newSolution = newSolution.WithDocumentSyntaxRoot(editor.OriginalDocument.Id, editor.GetChangedRoot());
                }
            }

            Solution allRemovedSolution;
            if(declarationEditor == null)
            {
                declarationEditor = await DocumentEditor.CreateAsync(declarationDocument, cancellationToken).ConfigureAwait(false);
                allRemovedSolution = newSolution;
            } else
            {
                allRemovedSolution =
                    newSolution.WithDocumentSyntaxRoot(
                        declarationEditor.OriginalDocument.Id,
                        declarationEditor.GetChangedRoot());
            }

            if (allRemoved)
            {
                declarationEditor.RemoveNode(Declaration);
                return newSolution.WithDocumentSyntaxRoot(
                    declarationEditor.OriginalDocument.Id,
                    declarationEditor.GetChangedRoot());
            }
            else
            {
                return allRemovedSolution;
            }
        }

        private async Task<(bool, DocumentEditor)> GetDocumentEditsAsync(Document document, IEnumerable<Location> locations, CancellationToken cancellationToken)
        {
            SemanticModel semanticModel =
                await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            DocumentEditor editor =
                await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            bool allInlined = true;
            foreach(Location location in locations)
            {
                SyntaxNode callingNode = root.FindNode(location.SourceSpan);
                allInlined &= TryInlineCall(callingNode, semanticModel, editor, cancellationToken);
            }

            return (allInlined, editor);
        }

        public bool TryInlineCall(SyntaxNode callingNode, SemanticModel semanticModel, DocumentEditor editor, CancellationToken cancellationToken)
        {
            if (callingNode.Parent is MemberAccessExpressionSyntax simpleMemberAccess)
            {
                callingNode = callingNode.Parent;
            }

            if (!(callingNode.Parent is TNode invocationNode))
            {
                return false;
            }

            INamedTypeSymbol enclosingType =
                semanticModel.GetEnclosingNamedType(invocationNode.SpanStart, cancellationToken);
            SingleInlineRefactoring<TNode, TDeclaration, TSymbol> singleInlineRefactoring =
                CreateSingleRefactoring(
                    editor.OriginalDocument,
                    invocationNode,
                    enclosingType,
                    semanticModel,
                    cancellationToken);
            if (singleInlineRefactoring == null)
            {
                return false;
            }

            if (DeclarationExpression != null)
            {
                ParenthesizedExpressionSyntax newDefinition =
                    singleInlineRefactoring.RewriteExpression(invocationNode, DeclarationExpression);
                editor.ReplaceNode(invocationNode, newDefinition);
                return true;
            }
            else if (DeclarationStatements != default)
            {
                SyntaxNode nodeIncludingConditionalAccess =
                    invocationNode.WalkUp(f => f.IsKind(SyntaxKind.ConditionalAccessExpression));
                var invocationStatement = (ExpressionStatementSyntax)nodeIncludingConditionalAccess.Parent;
                StatementSyntax[] newDefinition =
                    singleInlineRefactoring.RewriteStatements(
                        DeclarationStatements,
                        invocationNode.GetLeadingTrivia(),
                        invocationNode.GetTrailingTrivia());
                if (newDefinition.Length == 1)
                {
                    editor.ReplaceNode(invocationStatement, newDefinition[0]);
                }
                else
                {
                    editor.ReplaceNode(invocationStatement, SyntaxFactory.Block(newDefinition));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract SingleInlineRefactoring<TNode, TDeclaration, TSymbol> CreateSingleRefactoring(
            Document document,
            TNode invocation,
            INamedTypeSymbol invocationEnclosingType,
            SemanticModel invocationSemanticModel,
            CancellationToken cancellationToken);
    }
}
