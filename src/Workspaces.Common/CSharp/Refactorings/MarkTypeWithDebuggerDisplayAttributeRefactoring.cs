// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings;

internal static class MarkTypeWithDebuggerDisplayAttributeRefactoring
{
    public static async Task<Document> RefactorAsync(
        Document document,
        TypeDeclarationSyntax typeDeclaration,
        CancellationToken cancellationToken)
    {
        TypeDeclarationSyntax newTypeDeclaration = typeDeclaration;

        if (typeDeclaration.OpenBraceToken.IsKind(SyntaxKind.None))
        {
            newTypeDeclaration = typeDeclaration.WithSemicolonToken(default)
                .WithOpenBraceToken(OpenBraceToken())
                .WithCloseBraceToken(CloseBraceToken());
        }

        int position = newTypeDeclaration.Identifier.Span.End;

        SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        string propertyName = NameGenerator.Default.EnsureUniqueName(DefaultNames.DebuggerDisplayPropertyName, semanticModel, position);

        AttributeListSyntax attributeList = AttributeList(
            Attribute(
                ParseName("System.Diagnostics.DebuggerDisplayAttribute").WithSimplifierAnnotation(),
                AttributeArgument(LiteralExpression($"{{{propertyName},nq}}"))));

        INamedTypeSymbol typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration, cancellationToken)!;

        ExpressionSyntax returnExpression = (typeSymbol.TypeKind == TypeKind.Struct && typeSymbol.IsRefLikeType)
            ? StringLiteralExpression("")
            : InvocationExpression(IdentifierName("ToString"));

        PropertyDeclarationSyntax propertyDeclaration = DebuggerDisplayPropertyDeclaration(propertyName, returnExpression);

        newTypeDeclaration = SyntaxRefactorings.AddAttributeLists(newTypeDeclaration, keepDocumentationCommentOnTop: true, attributeList);

        newTypeDeclaration = MemberDeclarationInserter.Default.Insert(newTypeDeclaration, propertyDeclaration);

        return await document.ReplaceNodeAsync(typeDeclaration, newTypeDeclaration, cancellationToken).ConfigureAwait(false);
    }

    public static PropertyDeclarationSyntax DebuggerDisplayPropertyDeclaration(string name, ExpressionSyntax returnExpression)
    {
        return PropertyDeclaration(
            SingletonList(
                AttributeList(
                    Attribute(
                        ParseName("System.Diagnostics.DebuggerBrowsableAttribute"),
                        AttributeArgument(
                            SimpleMemberAccessExpression(
                                ParseName("System.Diagnostics.DebuggerBrowsableState").WithSimplifierAnnotation(),
                                IdentifierName("Never"))
                        )
                    )
                        .WithSimplifierAnnotation()
                )
            ),
            Modifiers.Private(),
            CSharpTypeFactory.StringType(),
            default(ExplicitInterfaceSpecifierSyntax),
            Identifier(name).WithRenameAnnotation(),
            AccessorList(
                GetAccessorDeclaration(
                    Block(
                        ReturnStatement(returnExpression))
                    )
                )
            )
            .WithFormatterAnnotation();
    }
}
