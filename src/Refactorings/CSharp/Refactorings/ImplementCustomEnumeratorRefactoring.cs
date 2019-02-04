// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ImplementCustomEnumeratorRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            TypeDeclarationSyntax typeDeclaration,
            SemanticModel semanticModel)
        {
            INamedTypeSymbol symbol = semanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken);

            if (symbol.IsAbstract)
                return;

            INamedTypeSymbol ienumerableOfT = symbol.Interfaces.FirstOrDefault(f => f.OriginalDefinition.HasMetadataName(MetadataNames.System_Collections_Generic_IEnumerable_T));

            if (ienumerableOfT == null)
                return;

            INamedTypeSymbol enumerator = symbol.FindTypeMember(
                "Enumerator",
                f => f.TypeKind == TypeKind.Struct
                    && f.DeclaredAccessibility == Accessibility.Public
                    && f.Arity == 0,
                includeBaseTypes: true);

            if (enumerator != null)
                return;

            context.RegisterRefactoring(
                "Implement custom enumerator",
                ct => RefactorAsync(context.Document, typeDeclaration, symbol, ienumerableOfT.TypeArguments.Single(), ct),
                RefactoringIdentifiers.ImplementCustomEnumerator);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            TypeDeclarationSyntax typeDeclaration,
            INamedTypeSymbol typeSymbol,
            ITypeSymbol elementSymbol,
            CancellationToken cancellationToken)
        {
            TypeSyntax type = typeSymbol.ToTypeSyntax().WithSimplifierAnnotation();

            TypeSyntax elementType = elementSymbol.ToTypeSyntax().WithSimplifierAnnotation();

            string identifier = NameGenerator.CreateName(typeSymbol, firstCharToLower: true) ?? DefaultNames.Variable;

            string identifierWithUnderscore = "_" + identifier;

            MethodDeclarationSyntax getEnumeratorDeclaration = MethodDeclaration(
                Modifiers.Public(),
                IdentifierName("Enumerator"),
                Identifier("GetEnumerator"),
                ParameterList(),
                Block(
                    ReturnStatement(
                        ObjectCreationExpression(
                            IdentifierName("Enumerator"), ArgumentList(Argument(ThisExpression()))))));

            StructDeclarationSyntax enumeratorDeclaration = StructDeclaration(
                Modifiers.Public(),
                Identifier("Enumerator").WithRenameAnnotation(),
                CreateEnumeratorMembers(type, elementType, identifier, identifierWithUnderscore).ToSyntaxList());

            ClassDeclarationSyntax enumeratorImplDeclaration = ClassDeclaration(
                attributeLists: default,
                Modifiers.Private(),
                Identifier("EnumeratorImpl"),
                typeParameterList: default,
                BaseList(
                    SimpleBaseType(
                        ParseTypeName($"global::System.Collections.Generic.IEnumerator<{elementType}>").WithSimplifierAnnotation())),
                constraintClauses: default,
                CreateEnumeratorImplMembers(typeSymbol, type, elementType, identifier).ToSyntaxList());

            enumeratorImplDeclaration = enumeratorImplDeclaration.WithLeadingTrivia(ParseLeadingTrivia(
                "//TODO: IEnumerable.GetEnumerator() and IEnumerable<T>.GetEnumerator() should return instance of EnumeratorImpl." + System.Environment.NewLine));

            MemberDeclarationInserter inserter = MemberDeclarationInserter.Default;

            TypeDeclarationSyntax newNode = inserter.Insert(typeDeclaration, getEnumeratorDeclaration.WithFormatterAnnotation());

            newNode = inserter.Insert(newNode, enumeratorDeclaration.WithFormatterAnnotation());

            newNode = inserter.Insert(newNode, enumeratorImplDeclaration.WithFormatterAnnotation());

            return document.ReplaceNodeAsync(typeDeclaration, newNode, cancellationToken);
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateEnumeratorMembers(
            TypeSyntax type,
            TypeSyntax elementType,
            string identifier,
            string identifierWithUnderscore)
        {
            yield return FieldDeclaration(Modifiers.Private_ReadOnly(), type, identifierWithUnderscore);

            yield return FieldDeclaration(Modifiers.Private(), IntType(), "_index");

            yield return ConstructorDeclaration(
                Modifiers.Internal(),
                Identifier("Enumerator"),
                ParameterList(Parameter(type, identifier)),
                Block(
                    SimpleAssignmentStatement(IdentifierName(identifierWithUnderscore), IdentifierName(identifier)),
                    SimpleAssignmentStatement(IdentifierName("_index"), NumericLiteralExpression(-1))));

            yield return PropertyDeclaration(
                Modifiers.Public(),
                elementType,
                Identifier("Current"),
                AccessorList(
                    GetAccessorDeclaration(
                        Block(ThrowNewStatement(NotImplementedException())))));

            yield return MethodDeclaration(
                Modifiers.Public(),
                BoolType(),
                Identifier("MoveNext"),
                ParameterList(),
                Block(ThrowNewStatement(NotImplementedException())));

            yield return MethodDeclaration(
                Modifiers.Public(),
                VoidType(),
                Identifier("Reset"),
                ParameterList(),
                Block(
                    SimpleAssignmentStatement(IdentifierName("_index"), NumericLiteralExpression(-1)),
                    ThrowNewStatement(NotImplementedException())));

            yield return MethodDeclaration(
                Modifiers.Public_Override(),
                BoolType(),
                Identifier("Equals"),
                ParameterList(Parameter(ObjectType(), "obj")),
                Block(ThrowNewStatement(NotSupportedException())));

            yield return MethodDeclaration(
                Modifiers.Public_Override(),
                IntType(),
                Identifier("GetHashCode"),
                ParameterList(),
                Block(ThrowNewStatement(NotSupportedException())));
        }

        private static IEnumerable<MemberDeclarationSyntax> CreateEnumeratorImplMembers(
            INamedTypeSymbol typeSymbol,
            TypeSyntax type,
            TypeSyntax elementType,
            string identifier)
        {
            yield return FieldDeclaration(Modifiers.Private(), IdentifierName("Enumerator"), "_e");

            yield return ConstructorDeclaration(
                Modifiers.Internal(),
                Identifier("EnumeratorImpl"),
                ParameterList(
                    Parameter(
                        attributeLists: default,
                        (typeSymbol.IsReadOnlyStruct()) ? TokenList(Token(SyntaxKind.InKeyword)) : TokenList(),
                        type,
                        Identifier(identifier),
                        @default: default)),
                Block(
                    SimpleAssignmentStatement(
                        IdentifierName("_e"),
                        ObjectCreationExpression(
                            IdentifierName("Enumerator"),
                            ArgumentList(Argument(IdentifierName(identifier)))))));

            yield return PropertyDeclaration(
                Modifiers.Public(),
                elementType,
                Identifier("Current"),
                AccessorList(
                    GetAccessorDeclaration(
                        Block(
                            ReturnStatement(SimpleMemberAccessExpression(IdentifierName("_e"), IdentifierName("Current")))))));

            yield return PropertyDeclaration(
                attributeLists: default,
                modifiers: default,
                ObjectType(),
                ExplicitInterfaceSpecifier(ParseName("global::System.Collections.IEnumerator").WithSimplifierAnnotation()),
                Identifier("Current"),
                AccessorList(
                    GetAccessorDeclaration(
                        Block(
                            ReturnStatement(SimpleMemberAccessExpression(IdentifierName("_e"), IdentifierName("Current")))))));

            yield return MethodDeclaration(
                Modifiers.Public(),
                BoolType(),
                Identifier("MoveNext"),
                ParameterList(),
                Block(
                    ReturnStatement(
                        SimpleMemberInvocationExpression(IdentifierName("_e"), IdentifierName("MoveNext")))));

            yield return MethodDeclaration(
                attributeLists: default,
                modifiers: default,
                VoidType(),
                ExplicitInterfaceSpecifier(ParseName("global::System.Collections.IEnumerator").WithSimplifierAnnotation()),
                Identifier("Reset"),
                typeParameterList: default,
                ParameterList(),
                constraintClauses: default,
                Block(
                    ExpressionStatement(
                        SimpleMemberInvocationExpression(IdentifierName("_e"), IdentifierName("Reset")))),
                expressionBody: default);

            yield return MethodDeclaration(
                attributeLists: default,
                modifiers: default,
                VoidType(),
                ExplicitInterfaceSpecifier(ParseName("global::System.IDisposable").WithSimplifierAnnotation()),
                Identifier("Dispose"),
                typeParameterList: default,
                ParameterList(),
                constraintClauses: default,
                Block(),
                expressionBody: default);
        }
    }
}
