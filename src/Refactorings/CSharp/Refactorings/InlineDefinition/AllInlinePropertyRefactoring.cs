using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.InlineDefinition;
using System.Collections.Immutable;
using System.Threading;

namespace Roslynator.CSharp.Refactorings.CSharp.Refactorings.InlineDefinition
{
    internal class AllInlinePropertyRefactoring
        : AllInlineRefactoring<IdentifierNameSyntax, PropertyDeclarationSyntax, IPropertySymbol>
    {
        public AllInlinePropertyRefactoring(
            IPropertySymbol symbol,
            PropertyDeclarationSyntax declaration,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
            : base(symbol, declaration, declarationSemanticModel, cancellationToken)
        {
            (DeclarationExpression, DeclarationStatements) =
                InlinePropertyAnalyzer.GetPropertyDefinition(Declaration);
        }

        public override SyntaxNode BodyOrExpressionBody =>
            Declaration.ExpressionBody ?? Declaration.AccessorList.Accessors[0].BodyOrExpressionBody();

        public override ImmutableArray<ITypeSymbol> TypeArguments => ImmutableArray<ITypeSymbol>.Empty;

        public override SyntaxList<StatementSyntax> DeclarationStatements { get; }

        public override ExpressionSyntax DeclarationExpression { get; }

        protected override SingleInlineRefactoring<IdentifierNameSyntax, PropertyDeclarationSyntax, IPropertySymbol> CreateSingleRefactoring(
            Document document,
            IdentifierNameSyntax node,
            INamedTypeSymbol nodeEnclosingType,
            SemanticModel nodeSemanticModel,
            CancellationToken cancellationToken)
        {
            if (!(nodeSemanticModel.GetSymbol(node, cancellationToken) is IPropertySymbol invocationSymbol))
            {
                return null;
            }

            if (invocationSymbol.Language != LanguageNames.CSharp)
            {
                return null;
            }

            return new SingleInlinePropertyRefactoring(
                document,
                node,
                nodeEnclosingType,
                invocationSymbol,
                Declaration,
                ImmutableArray<ParameterInfo>.Empty,
                nodeSemanticModel,
                DeclarationSemanticModel,
                cancellationToken);
        }

        public static void ComputeRefactorings(RefactoringContext context, PropertyDeclarationSyntax node, SemanticModel semanticModel)
        {

            if (!(semanticModel.GetDeclaredSymbol(node) is IPropertySymbol symbol))
            {
                return;
            }

            var refactoring = new AllInlinePropertyRefactoring(symbol, node, semanticModel, context.CancellationToken);
            string title = CSharpFacts.GetTitle(node);

            context.RegisterRefactoring(
                $"Inline all references and remove {title}",
                cancellationToken => refactoring.InlineAndRemoveAsync(context.Solution, cancellationToken),
                EquivalenceKey.Join(RefactoringIdentifiers.InlineMethod, "Remove"));
        }
    }
}
