// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MarkTypeWithDebuggerDisplayAttributeRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            TypeDeclarationSyntax typeDeclaration,
            CancellationToken cancellationToken)
        {
            int position = typeDeclaration.OpenBraceToken.Span.End;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string propertyName = NameGenerator.Default.EnsureUniqueMemberName(DefaultNames.DebuggerDisplayPropertyName, semanticModel, position, cancellationToken: cancellationToken);

            AttributeListSyntax attributeList = AttributeList(
                Attribute(
                    ParseName("System.Diagnostics.DebuggerDisplayAttribute").WithSimplifierAnnotation(),
                    AttributeArgument(LiteralExpression($"{{{propertyName},nq}}"))));

            PropertyDeclarationSyntax propertyDeclaration = DebuggerDisplayPropertyDeclaration(propertyName, InvocationExpression(IdentifierName("ToString")));

            TypeDeclarationSyntax newTypeDeclaration;

            if (typeDeclaration is ClassDeclarationSyntax classDeclaration)
            {
                newTypeDeclaration = classDeclaration.AddAttributeLists(keepDocumentationCommentOnTop: true, attributeList);
            }
            else
            {
                var structDeclaration = (StructDeclarationSyntax)typeDeclaration;

                newTypeDeclaration = structDeclaration.AddAttributeLists(keepDocumentationCommentOnTop: true, attributeList);
            }

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
                        ).WithSimplifierAnnotation()
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
                ).WithFormatterAnnotation();
        }
    }
}
