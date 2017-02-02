// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateBaseConstructorsRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.Identifier.Span.Contains(context.Span))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

                if (symbol != null
                    && !symbol.IsStatic)
                {
                    INamedTypeSymbol baseSymbol = symbol.BaseType;

                    if (baseSymbol != null)
                    {
                        IEnumerable<IMethodSymbol> declaredConstructors = classDeclaration.Members
                            .Where(f => f.IsKind(SyntaxKind.ConstructorDeclaration))
                            .Select(f => semanticModel.GetDeclaredSymbol(f, context.CancellationToken))
                            .Where(f => f.IsInstanceMethod())
                            .Cast<IMethodSymbol>();

                        using (IEnumerator<IMethodSymbol> en = baseSymbol
                            .InstanceConstructors
                            .Where(f => !f.IsPrivate())
                            .Except(declaredConstructors, ConstructorComparer.Instance)
                            .GetEnumerator())
                        {
                            if (en.MoveNext())
                            {
                                var constructors = new List<IMethodSymbol>();
                                constructors.Add(en.Current);

                                string title = "Generate base constructor";

                                if (en.MoveNext())
                                {
                                    title += "s";
                                    constructors.Add(en.Current);

                                    while (en.MoveNext())
                                        constructors.Add(en.Current);
                                }

                                context.RegisterRefactoring(
                                    title,
                                    cancellationToken => RefactorAsync(context.Document, classDeclaration, constructors.ToArray(), context.CancellationToken));
                            }
                        }
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            IMethodSymbol[] constructorSymbols,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<MemberDeclarationSyntax> members = classDeclaration.Members;

            string name = classDeclaration.Identifier.ValueText;

            int insertIndex = Inserter.GetMemberInsertIndex(members, SyntaxKind.ConstructorDeclaration);

            int position = (insertIndex == 0)
                ? classDeclaration.OpenBraceToken.FullSpan.End
                : members[insertIndex - 1].FullSpan.End;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<ConstructorDeclarationSyntax> constructors = constructorSymbols
                .Select(symbol => CreateConstructor(symbol, name, semanticModel, position));

            ClassDeclarationSyntax newClassDeclaration = classDeclaration
                .WithMembers(members.InsertRange(insertIndex, constructors));

            return await document.ReplaceNodeAsync(classDeclaration, newClassDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static ConstructorDeclarationSyntax CreateConstructor(IMethodSymbol methodSymbol, string name, SemanticModel semanticModel, int position)
        {
            var parameters = new List<ParameterSyntax>();
            var arguments = new List<ArgumentSyntax>();

            foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
            {
                EqualsValueClauseSyntax @default = null;

                if (parameterSymbol.HasExplicitDefaultValue)
                    @default = EqualsValueClause(DefaultValue(parameterSymbol).WithSimplifierAnnotation());

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    ModifierFactory.FromAccessibility(parameterSymbol.DeclaredAccessibility),
                    parameterSymbol.Type.ToMinimalSyntax(semanticModel, position),
                    Identifier(parameterSymbol.Name),
                    @default));

                arguments.Add(Argument(IdentifierName(parameterSymbol.Name)));
            }

            ConstructorDeclarationSyntax constructor = ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                ModifierFactory.FromAccessibility(methodSymbol.DeclaredAccessibility),
                Identifier(name),
                ParameterList(SeparatedList(parameters)),
                BaseConstructorInitializer(ArgumentList(arguments.ToArray())),
                Block());

            return constructor.WithFormatterAnnotation();
        }

        private class ConstructorComparer : EqualityComparer<IMethodSymbol>
        {
            public static ConstructorComparer Instance { get; } = new ConstructorComparer();

            public override bool Equals(IMethodSymbol x, IMethodSymbol y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null)
                    return false;

                ImmutableArray<IParameterSymbol> parameters1 = x.Parameters;
                ImmutableArray<IParameterSymbol> parameters2 = y.Parameters;

                if (parameters1.Length != parameters2.Length)
                    return false;

                for (int i = 0; i < parameters1.Length; i++)
                {
                    if (!parameters1[i].Type.Equals(parameters2[i].Type))
                        return false;
                }

                return true;
            }

            public override int GetHashCode(IMethodSymbol obj)
            {
                return 0;
            }
        }
    }
}