// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class GeneratePropertyForDebuggerDisplayAttributeRefactoring
    {
        private const string PropertyName = "DebuggerDisplay";

        public static async Task ComputeRefactoringAsync(RefactoringContext context, AttributeSyntax attribute)
        {
            if (attribute.ArgumentList?.Arguments.Count(f => f.NameEquals == null) != 1)
                return;

            if (!attribute.IsParentKind(SyntaxKind.AttributeList))
                return;

            if (!attribute.Parent.IsParentKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            string value = semanticModel
                .GetDeclaredSymbol((TypeDeclarationSyntax)attribute.Parent.Parent, context.CancellationToken)
                .GetAttribute(semanticModel.GetTypeByMetadataName(MetadataNames.System_Diagnostics_DebuggerDisplayAttribute))?
                .ConstructorArguments
                .SingleOrDefault(shouldThrow: false)
                .Value?
                .ToString();

            if (value == null)
                return;

            if (string.Equals(value, $"{{{PropertyName},nq}}", StringComparison.Ordinal))
                return;

            if (!CanRefactor(value))
                return;

            context.RegisterRefactoring(
                $"Generate property '{PropertyName}'",
                cancellationToken => RefactorAsync(context.Document, attribute, cancellationToken));
        }

        private static bool CanRefactor(string value)
        {
            int length = value.Length;

            if (length == 0)
                return true;

            int i = 0;

            while (true)
            {
                FindOpenBrace();

                if (i == -1)
                    return false;

                if (i == length)
                    break;

                i++;

                FindCloseBrace();

                if (i == -1)
                    return false;

                if (i == length)
                    break;

                i++;
            }

            return true;

            void FindOpenBrace()
            {
                while (i < length)
                {
                    switch (value[i])
                    {
                        case '{':
                            {
                                return;
                            }
                        case '}':
                            {
                                i = -1;
                                return;
                            }
                        case '\\':
                            {
                                i++;

                                if (i < length)
                                {
                                    char ch = value[i];

                                    if (ch == '{'
                                        || ch == '}')
                                    {
                                        i++;
                                    }

                                    continue;
                                }
                                else
                                {
                                    return;
                                }
                            }
                    }

                    i++;
                }
            }

            void FindCloseBrace()
            {
                while (i < length)
                {
                    switch (value[i])
                    {
                        case '{':
                            {
                                i = -1;
                                return;
                            }
                        case '}':
                            {
                                return;
                            }
                        case '(':
                            {
                                i++;

                                if (i < length
                                    && value[i] == ')')
                                {
                                    break;
                                }

                                i = -1;
                                return;
                            }
                        case ',':
                            {
                                i++;
                                if (i < length
                                    && value[i] == 'n')
                                {
                                    i++;
                                    if (i < length
                                        && value[i] == 'q')
                                    {
                                        i++;
                                        if (i < length
                                            && value[i] == '}')
                                        {
                                            return;
                                        }
                                    }
                                }

                                i = -1;
                                return;
                            }
                    }

                    i++;
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            AttributeSyntax attribute,
            CancellationToken cancellationToken)
        {
            TypeDeclarationSyntax typeDeclaration = attribute.FirstAncestor<TypeDeclarationSyntax>();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string propertyName = NameGenerator.Default.EnsureUniqueMemberName(PropertyName, semanticModel, typeDeclaration.OpenBraceToken.Span.End, cancellationToken: cancellationToken);

            AttributeArgumentSyntax argument = attribute.ArgumentList.Arguments.First();

            TypeDeclarationSyntax newTypeDeclaration = typeDeclaration.ReplaceNode(
                argument,
                argument.WithExpression(
                    StringLiteralExpression($"{{{propertyName},nq}}")).WithTriviaFrom(argument.Expression));

            string value = semanticModel
                .GetDeclaredSymbol(typeDeclaration, cancellationToken)
                .GetAttribute(semanticModel.GetTypeByMetadataName(MetadataNames.System_Diagnostics_DebuggerDisplayAttribute))
                .ConstructorArguments[0]
                .Value
                .ToString();

            ExpressionSyntax returnExpression = GetReturnExpression(value, SyntaxInfo.StringLiteralExpressionInfo(argument.Expression).IsVerbatim);

            PropertyDeclarationSyntax propertyDeclaration = PropertyDeclaration(
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
                Identifier(propertyName).WithRenameAnnotation(),
                AccessorList(
                    GetAccessorDeclaration(
                        Block(
                            ReturnStatement(returnExpression)))));

            propertyDeclaration = propertyDeclaration.WithFormatterAnnotation();

            newTypeDeclaration = MemberDeclarationInserter.Default.Insert(newTypeDeclaration, propertyDeclaration);

            return await document.ReplaceNodeAsync(typeDeclaration, newTypeDeclaration, cancellationToken).ConfigureAwait(false);
        }

        private static ExpressionSyntax GetReturnExpression(string value, bool isVerbatim)
        {
            StringBuilder sb = StringBuilderCache.GetInstance(capacity: value.Length);

            sb.Append('$');

            if (isVerbatim)
                sb.Append('@');

            sb.Append('"');

            int length = value.Length;

            int i = 0;

            int lastPos = i;

            while (true)
            {
                lastPos = i;

                AppendInterpolatedText();

                if (i == length)
                    break;

                i++;

                lastPos = i;

                AppendInterpolation();

                if (i == length)
                    break;

                i++;
            }

            sb.Append(value, lastPos, i - lastPos);
            sb.Append("\"");

            return ParseExpression(StringBuilderCache.GetStringAndFree(sb));

            void AppendInterpolatedText()
            {
                while (i < length)
                {
                    switch (value[i])
                    {
                        case '{':
                            {
                                sb.Append(value, lastPos, i - lastPos);
                                return;
                            }
                        case '\\':
                            {
                                sb.Append(value, lastPos, i - lastPos);

                                i++;

                                if (i < length)
                                {
                                    char ch = value[i];

                                    if (ch == '{'
                                        || ch == '}')
                                    {
                                        sb.Append(ch);
                                        sb.Append(ch);
                                        i++;
                                        lastPos = i;
                                        continue;
                                    }
                                }

                                sb.Append((isVerbatim) ? "\\" : "\\\\");
                                lastPos = i;
                                continue;
                            }
                        case '"':
                            {
                                sb.Append(value, lastPos, i - lastPos);
                                sb.Append((isVerbatim) ? "\"\"" : "\\\"");
                                i++;
                                lastPos = i;
                                continue;
                            }
                    }

                    i++;
                }
            }

            void AppendInterpolation()
            {
                while (i < length)
                {
                    switch (value[i])
                    {
                        case '}':
                            {
                                sb.Append('{');
                                sb.Append(value, lastPos, i - lastPos);
                                sb.Append('}');
                                return;
                            }
                        case '(':
                            {
                                i++;
                                break;
                            }
                        case ',':
                            {
                                sb.Append('{');
                                sb.Append(value, lastPos, i - lastPos);
                                sb.Append('}');

                                i += 3;
                                return;
                            }
                    }

                    i++;
                }
            }
        }
    }
}