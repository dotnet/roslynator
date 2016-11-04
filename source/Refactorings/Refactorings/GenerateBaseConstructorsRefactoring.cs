// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                            .Where(f => f?.IsMethod() == true && !f.IsStatic)
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
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            string name = classDeclaration.Identifier.ValueText;

            IEnumerable<ConstructorDeclarationSyntax> constructors = constructorSymbols
                .Select(f => CreateConstructor(f, name).WithFormatterAnnotation());

            ClassDeclarationSyntax newClassDeclaration = classDeclaration
                .WithMembers(classDeclaration.Members.InsertRange(FindIndex(classDeclaration), constructors));

            SyntaxNode newRoot = root.ReplaceNode(classDeclaration, newClassDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ConstructorDeclarationSyntax CreateConstructor(IMethodSymbol methodSymbol, string name)
        {
            var parameters = new List<ParameterSyntax>();
            var arguments = new List<ArgumentSyntax>();

            foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
            {
                EqualsValueClauseSyntax @default = null;

                if (parameterSymbol.HasExplicitDefaultValue)
                    @default = EqualsValueClause(CreateDefaultExpression(parameterSymbol.ExplicitDefaultValue));

                parameters.Add(Parameter(
                    default(SyntaxList<AttributeListSyntax>),
                    Modifiers.FromAccessibility(parameterSymbol.DeclaredAccessibility),
                    Type(parameterSymbol.Type).WithSimplifierAnnotation(),
                    Identifier(parameterSymbol.Name),
                    @default));

                arguments.Add(Argument(IdentifierName(parameterSymbol.Name)));
            }

            return ConstructorDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                Modifiers.FromAccessibility(methodSymbol.DeclaredAccessibility),
                Identifier(name),
                ParameterList(parameters),
                BaseConstructorInitializer(ArgumentList(arguments.ToArray())),
                Block());
        }

        private static ExpressionSyntax CreateDefaultExpression(object value)
        {
            if (value == null)
            {
                return NullLiteralExpression();
            }
            else if (value is bool)
            {
                if ((bool)value)
                {
                    return TrueLiteralExpression();
                }
                else
                {
                    return FalseLiteralExpression();
                }
            }
            else if (value is char)
            {
                return CharacterLiteralExpression((char)value);
            }
            else if (value is sbyte)
            {
                return NumericLiteralExpression((sbyte)value);
            }
            else if (value is byte)
            {
                NumericLiteralExpression((byte)value);
            }
            else if (value is short)
            {
                return NumericLiteralExpression((short)value);
            }
            else if (value is ushort)
            {
                return NumericLiteralExpression((ushort)value);
            }
            else if (value is int)
            {
                return NumericLiteralExpression((int)value);
            }
            else if (value is uint)
            {
                return NumericLiteralExpression((uint)value);
            }
            else if (value is long)
            {
                return NumericLiteralExpression((long)value);
            }
            else if (value is ulong)
            {
                return NumericLiteralExpression((ulong)value);
            }
            else if (value is decimal)
            {
                return NumericLiteralExpression((decimal)value);
            }
            else if (value is float)
            {
                return NumericLiteralExpression((float)value);
            }
            else if (value is double)
            {
                return NumericLiteralExpression((double)value);
            }

            return StringLiteralExpression(value.ToString());
        }

        private static int FindIndex(ClassDeclarationSyntax classDeclaration)
        {
            SyntaxList<MemberDeclarationSyntax> members = classDeclaration.Members;

            int index = members.LastIndexOf(SyntaxKind.ConstructorDeclaration);

            if (index == -1)
            {
                return members
                    .TakeWhile(f => f.IsKind(SyntaxKind.FieldDeclaration))
                    .Count();
            }

            return index + 1;
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