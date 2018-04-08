// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseRegexInstanceInsteadOfStaticMethodRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax memberDeclaration = invocationExpression.FirstAncestor<MemberDeclarationSyntax>();

            Debug.Assert(memberDeclaration != null, "");

            if (memberDeclaration != null)
            {
                TypeDeclarationSyntax typeDeclaration = memberDeclaration.FirstAncestor<TypeDeclarationSyntax>();

                Debug.Assert(typeDeclaration != null, "");

                if (typeDeclaration != null)
                {
                    SimpleMemberInvocationExpressionInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationExpressionInfo(invocationExpression);

                    string fieldName = NameGenerator.Default.EnsureUniqueLocalName("_regex", semanticModel, invocationExpression.SpanStart, cancellationToken: cancellationToken);

                    MemberAccessExpressionSyntax newMemberAccess = invocationInfo.MemberAccessExpression.WithExpression(IdentifierName(Identifier(fieldName).WithRenameAnnotation()));

                    ArgumentListPair pair = RewriteArgumentLists(invocationInfo.ArgumentList, semanticModel, cancellationToken);

                    InvocationExpressionSyntax newInvocationExpression = invocationExpression
                        .WithExpression(newMemberAccess)
                        .WithArgumentList(pair.ArgumentList1);

                    TypeDeclarationSyntax newTypeDeclaration = typeDeclaration.ReplaceNode(invocationExpression, newInvocationExpression);

                    TypeSyntax regexType = semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_RegularExpressions_Regex).ToMinimalTypeSyntax(semanticModel, typeDeclaration.SpanStart);

                    FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                        Modifiers.PrivateStaticReadOnly(),
                        regexType,
                        Identifier(fieldName),
                        EqualsValueClause(
                            ObjectCreationExpression(regexType, pair.ArgumentList2)));

                    SyntaxList<MemberDeclarationSyntax> newMembers = MemberDeclarationInserter.Default.Insert(newTypeDeclaration.Members, fieldDeclaration);

                    newTypeDeclaration = newTypeDeclaration.WithMembers(newMembers);

                    return await document.ReplaceNodeAsync(typeDeclaration, newTypeDeclaration, cancellationToken).ConfigureAwait(false);
                }
            }

            return document;
        }

        private static ArgumentListPair RewriteArgumentLists(
            ArgumentListSyntax argumentList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ArgumentSyntax pattern = null;
            ArgumentSyntax regexOptions = null;
            ArgumentSyntax matchTimeout = null;

            SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

            SeparatedSyntaxList<ArgumentSyntax> newArguments = arguments;

            for (int i = arguments.Count - 1; i >= 0; i--)
            {
                IParameterSymbol parameterSymbol = semanticModel.DetermineParameter(arguments[i], cancellationToken: cancellationToken);

                Debug.Assert(parameterSymbol != null, "");

                if (parameterSymbol != null)
                {
                    if (pattern == null
                        && parameterSymbol.Type.IsString()
                        && parameterSymbol.Name == "pattern")
                    {
                        pattern = arguments[i];
                        newArguments = newArguments.RemoveAt(i);
                    }

                    if (regexOptions == null
                        && parameterSymbol.Type.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_RegularExpressions_RegexOptions)))
                    {
                        regexOptions = arguments[i];
                        newArguments = newArguments.RemoveAt(i);
                    }

                    if (matchTimeout == null
                        && parameterSymbol.Type.Equals(semanticModel.GetTypeByMetadataName(MetadataNames.System_TimeSpan)))
                    {
                        matchTimeout = arguments[i];
                        newArguments = newArguments.RemoveAt(i);
                    }
                }
            }

            argumentList = argumentList.WithArguments(newArguments);

            var arguments2 = new List<ArgumentSyntax>();

            if (pattern != null)
                arguments2.Add(pattern);

            if (regexOptions != null)
                arguments2.Add(regexOptions);

            if (matchTimeout != null)
                arguments2.Add(matchTimeout);

            return new ArgumentListPair(argumentList, ArgumentList(arguments2.ToArray()));
        }

        private readonly struct ArgumentListPair
        {
            public ArgumentListPair(ArgumentListSyntax argumentList1, ArgumentListSyntax argumentList2)
            {
                ArgumentList1 = argumentList1;
                ArgumentList2 = argumentList2;
            }

            public ArgumentListSyntax ArgumentList1 { get; }
            public ArgumentListSyntax ArgumentList2 { get; }
        }
    }
}
