// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ImplementIEquatableOfTRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            SyntaxToken identifier = classDeclaration.Identifier;

            if (identifier.IsMissing)
                return;

            TextSpan span = identifier.Span;

            BaseListSyntax baseList = classDeclaration.BaseList;

            if (baseList != null)
                span = TextSpan.FromBounds(span.Start, baseList.Span.End);

            TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

            if (typeParameterList != null)
                span = TextSpan.FromBounds(span.Start, typeParameterList.Span.End);

            if (!span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            INamedTypeSymbol classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

            if (classSymbol?.IsErrorType() != false)
                return;

            if (classSymbol.IsStatic)
                return;

            INamedTypeSymbol equatableSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_IEquatable_T);

            if (equatableSymbol == null)
                return;

            equatableSymbol = equatableSymbol.Construct(classSymbol);

            if (classSymbol.Implements(equatableSymbol, allInterfaces: true))
                return;

            context.RegisterRefactoring(
                GetTitle(equatableSymbol, semanticModel, classDeclaration.SpanStart),
                f => RefactorAsync(context.Document, classDeclaration, classSymbol, equatableSymbol, semanticModel, f));
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            SyntaxToken identifier = structDeclaration.Identifier;

            if (identifier.IsMissing)
                return;

            TextSpan span = identifier.Span;

            BaseListSyntax baseList = structDeclaration.BaseList;

            if (baseList != null)
                span = TextSpan.FromBounds(span.Start, baseList.Span.End);

            TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

            if (typeParameterList != null)
                span = TextSpan.FromBounds(span.Start, typeParameterList.Span.End);

            if (!span.Contains(context.Span))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(structDeclaration, context.CancellationToken);

            if (typeSymbol?.IsErrorType() != false)
                return;

            INamedTypeSymbol equatableSymbol = semanticModel.GetTypeByMetadataName(MetadataNames.System_IEquatable_T);

            if (equatableSymbol == null)
                return;

            equatableSymbol = equatableSymbol.Construct(typeSymbol);

            if (typeSymbol.Implements(equatableSymbol, allInterfaces: true))
                return;

            context.RegisterRefactoring(
                GetTitle(equatableSymbol, semanticModel, structDeclaration.SpanStart),
                f => RefactorAsync(context.Document, structDeclaration, typeSymbol, equatableSymbol, semanticModel, f));
        }

        private static string GetTitle(INamedTypeSymbol equatableSymbol, SemanticModel semanticModel, int position)
        {
            return $"Implement {SymbolDisplay.ToMinimalDisplayString(equatableSymbol, semanticModel, position, SymbolDisplayFormats.Default)}";
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol typeSymbol,
            INamedTypeSymbol equatableSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int position = classDeclaration.SpanStart;

            SimpleBaseTypeSyntax baseType = SimpleBaseType(equatableSymbol.ToMinimalTypeSyntax(semanticModel, position));

            ClassDeclarationSyntax newNode = AddBaseType(classDeclaration, baseType);

            TypeSyntax classType = typeSymbol.ToMinimalTypeSyntax(semanticModel, position);

            newNode = MemberDeclarationInserter.Default.Insert(newNode, CreateEqualsMethod(classType, semanticModel, position));

            return document.ReplaceNodeAsync(classDeclaration, newNode, cancellationToken);
        }

        private static ClassDeclarationSyntax AddBaseType(ClassDeclarationSyntax classDeclaration, BaseTypeSyntax baseType)
        {
            BaseListSyntax baseList = classDeclaration.BaseList;

            if (baseList == null)
            {
                baseList = BaseList(baseType);

                TypeParameterListSyntax typeParameterList = classDeclaration.TypeParameterList;

                if (typeParameterList != null)
                {
                    return classDeclaration
                        .WithTypeParameterList(typeParameterList.WithoutTrailingTrivia())
                        .WithBaseList(baseList.WithTrailingTrivia(typeParameterList.GetTrailingTrivia()));
                }
                else
                {
                    SyntaxToken identifier = classDeclaration.Identifier;

                    return classDeclaration
                        .WithIdentifier(identifier.WithoutTrailingTrivia())
                        .WithBaseList(baseList.WithTrailingTrivia(identifier.TrailingTrivia));
                }
            }
            else
            {
                SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;

                BaseTypeSyntax lastType = types.LastOrDefault();

                if (lastType == null
                    || (types.Count == 1 && types[0].IsMissing))
                {
                    SyntaxToken colonToken = baseList.ColonToken;

                    baseType = baseType
                        .WithLeadingTrivia(Space)
                        .WithTrailingTrivia(colonToken.TrailingTrivia);

                    baseList = baseList
                        .WithColonToken(colonToken.WithoutTrailingTrivia())
                        .WithTypes(SingletonSeparatedList(baseType));

                    return classDeclaration.WithBaseList(baseList);
                }
                else
                {
                    types = types
                        .Replace(lastType, lastType.WithoutTrailingTrivia())
                        .Add(baseType.WithTrailingTrivia(lastType.GetTrailingTrivia()));

                    return classDeclaration.WithBaseList(baseList.WithTypes(types));
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StructDeclarationSyntax structDeclaration,
            INamedTypeSymbol typeSymbol,
            INamedTypeSymbol equatableSymbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            int position = structDeclaration.SpanStart;

            SimpleBaseTypeSyntax baseType = SimpleBaseType(equatableSymbol.ToMinimalTypeSyntax(semanticModel, position));

            StructDeclarationSyntax newNode = AddBaseType(structDeclaration, baseType);

            TypeSyntax classType = typeSymbol.ToMinimalTypeSyntax(semanticModel, position);

            newNode = MemberDeclarationInserter.Default.Insert(newNode, CreateEqualsMethod(classType, semanticModel, position));

            return document.ReplaceNodeAsync(structDeclaration, newNode, cancellationToken);
        }

        private static StructDeclarationSyntax AddBaseType(StructDeclarationSyntax structDeclaration, BaseTypeSyntax baseType)
        {
            BaseListSyntax baseList = structDeclaration.BaseList;

            if (baseList == null)
            {
                baseList = BaseList(baseType);

                TypeParameterListSyntax typeParameterList = structDeclaration.TypeParameterList;

                if (typeParameterList != null)
                {
                    return structDeclaration
                        .WithTypeParameterList(typeParameterList.WithoutTrailingTrivia())
                        .WithBaseList(baseList.WithTrailingTrivia(typeParameterList.GetTrailingTrivia()));
                }
                else
                {
                    SyntaxToken identifier = structDeclaration.Identifier;

                    return structDeclaration
                        .WithIdentifier(identifier.WithoutTrailingTrivia())
                        .WithBaseList(baseList.WithTrailingTrivia(identifier.TrailingTrivia));
                }
            }
            else
            {
                SeparatedSyntaxList<BaseTypeSyntax> types = baseList.Types;

                BaseTypeSyntax lastType = types.LastOrDefault();

                if (lastType == null
                    || (types.Count == 1 && types[0].IsMissing))
                {
                    SyntaxToken colonToken = baseList.ColonToken;

                    baseType = baseType
                        .WithLeadingTrivia(Space)
                        .WithTrailingTrivia(colonToken.TrailingTrivia);

                    baseList = baseList
                        .WithColonToken(colonToken.WithoutTrailingTrivia())
                        .WithTypes(SingletonSeparatedList(baseType));

                    return structDeclaration.WithBaseList(baseList);
                }
                else
                {
                    types = types
                        .Replace(lastType, lastType.WithoutTrailingTrivia())
                        .Add(baseType.WithTrailingTrivia(lastType.GetTrailingTrivia()));

                    return structDeclaration.WithBaseList(baseList.WithTypes(types));
                }
            }
        }

        private static MethodDeclarationSyntax CreateEqualsMethod(TypeSyntax type, SemanticModel semanticModel, int position)
        {
            return MethodDeclaration(
                Modifiers.Public(),
                BoolType(),
                Identifier("Equals"),
                ParameterList(Parameter(type, Identifier("other"))),
                Block(
                    ThrowStatement(
                        ObjectCreationExpression(semanticModel.GetTypeByMetadataName(MetadataNames.System_NotImplementedException).ToMinimalTypeSyntax(semanticModel, position), ArgumentList()))));
        }
    }
}