// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GenerateOnEventMethodRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            SemanticModel semanticModel = null;

            VariableDeclarationSyntax variableDeclaration = eventFieldDeclaration.Declaration;

            if (variableDeclaration != null)
            {
                foreach (VariableDeclaratorSyntax variableDeclarator in variableDeclaration.Variables)
                {
                    if (context.Span.IsContainedInSpanOrBetweenSpans(variableDeclarator.Identifier))
                    {
                        semanticModel = semanticModel ?? await context.GetSemanticModelAsync().ConfigureAwait(false);

                        var eventSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator, context.CancellationToken) as IEventSymbol;

                        if (eventSymbol?.IsStatic == false)
                        {
                            INamedTypeSymbol containingType = eventSymbol.ContainingType;

                            if (containingType?.IsInterface() == false)
                            {
                                var eventHandlerType = eventSymbol.Type as INamedTypeSymbol;

                                if (eventHandlerType != null)
                                {
                                    ITypeSymbol eventArgsSymbol = GetEventArgsSymbol(eventHandlerType, semanticModel);

                                    if (eventArgsSymbol != null)
                                    {
                                        string methodName = "On" + eventSymbol.Name;

                                        if (!containingType.ExistsMethod(
                                            $"On{eventSymbol.Name}",
                                            methodSymbol => eventArgsSymbol.Equals(methodSymbol.SingleParameterOrDefault()?.Type)))
                                        {
                                            methodName = NameGenerator.Default.EnsureUniqueMemberName(methodName, containingType);

                                            context.RegisterRefactoring(
                                                $"Generate '{methodName}' method",
                                                cancellationToken =>
                                                {
                                                    return RefactorAsync(
                                                        context.Document,
                                                        eventFieldDeclaration,
                                                        eventSymbol,
                                                        eventArgsSymbol,
                                                        context.SupportsCSharp6,
                                                        cancellationToken);
                                                });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static ITypeSymbol GetEventArgsSymbol(INamedTypeSymbol eventHandlerType, SemanticModel semanticModel)
        {
            ImmutableArray<ITypeSymbol> typeArguments = eventHandlerType.TypeArguments;

            if (typeArguments.Length == 0)
            {
                return semanticModel.GetTypeByMetadataName(MetadataNames.System_EventArgs);
            }
            else if (typeArguments.Length == 1)
            {
                return typeArguments[0];
            }

            return null;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            EventFieldDeclarationSyntax eventFieldDeclaration,
            IEventSymbol eventSymbol,
            ITypeSymbol eventArgsSymbol,
            bool supportsCSharp6,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var containingMember = (MemberDeclarationSyntax)eventFieldDeclaration.Parent;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            MethodDeclarationSyntax method = CreateOnEventMethod(eventSymbol, eventArgsSymbol, supportsCSharp6)
                .WithFormatterAnnotation();

            SyntaxList<MemberDeclarationSyntax> newMembers = members.InsertMember(method, MemberDeclarationComparer.ByKind);

            return document.ReplaceNodeAsync(containingMember, containingMember.WithMembers(newMembers), cancellationToken);
        }

        private static MethodDeclarationSyntax CreateOnEventMethod(
            IEventSymbol eventSymbol,
            ITypeSymbol eventArgsSymbol,
            bool supportCSharp6)
        {
            TypeSyntax eventArgsType = eventArgsSymbol.ToTypeSyntax().WithSimplifierAnnotation();

            return MethodDeclaration(
                default(SyntaxList<AttributeListSyntax>),
                (eventSymbol.ContainingType.IsSealed || eventSymbol.ContainingType.IsStruct())
                    ? Modifiers.Private()
                    : Modifiers.ProtectedVirtual(),
                VoidType(),
                default(ExplicitInterfaceSpecifierSyntax),
                Identifier($"On{eventSymbol.Name}"),
                default(TypeParameterListSyntax),
                ParameterList(Parameter(eventArgsType, Identifier(DefaultNames.EventArgsVariable))),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                Block(CreateOnEventMethodBody(eventSymbol, supportCSharp6)),
                default(ArrowExpressionClauseSyntax));
        }

        private static IEnumerable<StatementSyntax> CreateOnEventMethodBody(
            IEventSymbol eventSymbol,
            bool supportsCSharp6)
        {
            if (supportsCSharp6)
            {
                yield return ExpressionStatement(
                    ConditionalAccessExpression(
                        IdentifierName(eventSymbol.Name),
                        InvocationExpression(
                            MemberBindingExpression(IdentifierName("Invoke")),
                            ArgumentList(
                                Argument(ThisExpression()),
                                Argument(IdentifierName(DefaultNames.EventArgsVariable))))));
            }
            else
            {
                yield return LocalDeclarationStatement(
                    VariableDeclaration(
                        eventSymbol.Type.ToTypeSyntax().WithSimplifierAnnotation(),
                        VariableDeclarator(
                            Identifier(DefaultNames.EventHandlerVariable),
                            EqualsValueClause(IdentifierName(eventSymbol.Name)))));

                yield return IfStatement(
                    NotEqualsExpression(
                        IdentifierName(DefaultNames.EventHandlerVariable),
                        NullLiteralExpression()),
                    ExpressionStatement(
                        InvocationExpression(
                            IdentifierName(DefaultNames.EventHandlerVariable),
                            ArgumentList(Argument(ThisExpression()), Argument(IdentifierName(DefaultNames.EventArgsVariable))))));
            }
        }
    }
}