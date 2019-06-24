using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.InlineDefinition;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.CSharp.Refactorings.InlineDefinition
{
    internal class AllInlineMethodRefactoring
        : AllInlineRefactoring<InvocationExpressionSyntax, MethodDeclarationSyntax, IMethodSymbol>
    {
        public AllInlineMethodRefactoring(
            IMethodSymbol symbol,
            MethodDeclarationSyntax declaration,
            SemanticModel declarationSemanticModel,
            CancellationToken cancellationToken)
            : base(symbol, declaration, declarationSemanticModel, cancellationToken)
        {
            (DeclarationExpression, DeclarationStatements) = InlineMethodAnalyzer.GetMethodDefinition(Declaration);
        }

        public override SyntaxNode BodyOrExpressionBody => Declaration.BodyOrExpressionBody();

        public override ImmutableArray<ITypeSymbol> TypeArguments => Symbol.TypeArguments;

        public override SyntaxList<StatementSyntax> DeclarationStatements { get; }

        public override ExpressionSyntax DeclarationExpression { get; }

        protected override SingleInlineRefactoring<InvocationExpressionSyntax, MethodDeclarationSyntax, IMethodSymbol> CreateSingleRefactoring(
            Document document,
            InvocationExpressionSyntax node,
            INamedTypeSymbol nodeEnclosingType,
            SemanticModel nodeSemanticModel,
            CancellationToken cancellationToken)
        {
            if (!(nodeSemanticModel.GetSymbol(node, cancellationToken) is IMethodSymbol invocationSymbol))
            {
                return null;
            }

            if (invocationSymbol.Language != LanguageNames.CSharp)
            {
                return null;
            }

            ImmutableArray<ParameterInfo> parameterInfos = InlineMethodAnalyzer.GetMethodParameterInfos(node, invocationSymbol);
            return new SingleInlineMethodRefactoring(
                document,
                node,
                nodeEnclosingType,
                invocationSymbol,
                Declaration,
                parameterInfos,
                nodeSemanticModel,
                DeclarationSemanticModel,
                cancellationToken);
        }

        public static void ComputeRefactorings(RefactoringContext context, MethodDeclarationSyntax node, SemanticModel semanticModel)
        {

            if (!(semanticModel.GetDeclaredSymbol(node) is IMethodSymbol symbol))
            {
                return;
            }

            var refactoring = new AllInlineMethodRefactoring(symbol, node, semanticModel, context.CancellationToken);
            string title = CSharpFacts.GetTitle(node);

            context.RegisterRefactoring(
                $"Inline all references and remove {title}",
                cancellationToken => refactoring.InlineAndRemoveAsync(context.Solution, cancellationToken),
                EquivalenceKey.Join(RefactoringIdentifiers.InlineMethod, "Remove"));
        }
    }
}
