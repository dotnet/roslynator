// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseRegexInstanceInsteadOfStaticMethodRefactoring
    {
        internal static void Analyze(SyntaxNodeAnalysisContext context, MemberInvocationExpressionInfo invocationInfo)
        {
            switch (invocationInfo.NameText)
            {
                case "IsMatch":
                case "Match":
                case "Matches":
                case "Replace":
                case "Split":
                    {
                        if (context.SemanticModel.TryGetMethodInfo(invocationInfo.InvocationExpression, out MethodInfo methodInfo, context.CancellationToken)
                            && methodInfo.IsPublicStaticRegexMethod()
                            && methodInfo.ContainingType != null)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.UseRegexInstanceInsteadOfStaticMethod,
                                invocationInfo.Name);
                        }

                        break;
                    }
            }
        }

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
                BaseTypeDeclarationSyntax typeDeclaration = memberDeclaration.FirstAncestor<BaseTypeDeclarationSyntax>();

                Debug.Assert(typeDeclaration != null, "");

                if (typeDeclaration != null)
                {
                    MemberInvocationExpressionInfo invocationInfo = SyntaxInfo.MemberInvocationExpressionInfo(invocationExpression);

                    string fieldName = NameGenerator.Default.EnsureUniqueLocalName("_regex", semanticModel, invocationExpression.SpanStart, cancellationToken: cancellationToken);

                    MemberAccessExpressionSyntax newMemberAccess = invocationInfo.MemberAccessExpression.WithExpression(IdentifierName(Identifier(fieldName).WithRenameAnnotation()));

                    ArgumentListPair pair = RewriteArgumentLists(invocationInfo.ArgumentList, semanticModel, cancellationToken);

                    InvocationExpressionSyntax newInvocationExpression = invocationExpression
                        .WithExpression(newMemberAccess)
                        .WithArgumentList(pair.ArgumentList1);

                    MemberDeclarationSyntax newTypeDeclaration = typeDeclaration.ReplaceNode(invocationExpression, newInvocationExpression);

                    TypeSyntax regexType = semanticModel.GetTypeByMetadataName(MetadataNames.System_Text_RegularExpressions_Regex).ToMinimalTypeSyntax(semanticModel, typeDeclaration.SpanStart);

                    FieldDeclarationSyntax fieldDeclaration = FieldDeclaration(
                        (ShouldBeStatic(memberDeclaration, semanticModel, cancellationToken))
                            ? Modifiers.PrivateStaticReadOnly()
                            : Modifiers.PrivateReadOnly(),
                        regexType,
                        Identifier(fieldName),
                        EqualsValueClause(
                            ObjectCreationExpression(regexType, pair.ArgumentList2)));

                    SyntaxList<MemberDeclarationSyntax> newMembers = newTypeDeclaration
                        .GetMembers()
                        .InsertMember(fieldDeclaration, MemberDeclarationComparer.ByKind);

                    newTypeDeclaration = newTypeDeclaration.WithMembers(newMembers);

                    return await document.ReplaceNodeAsync(typeDeclaration, newTypeDeclaration, cancellationToken).ConfigureAwait(false);
                }
            }

            return document;
        }

        private static bool ShouldBeStatic(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (memberDeclaration.IsKind(SyntaxKind.FieldDeclaration))
            {
                return true;
            }
            else
            {
                return semanticModel.GetDeclaredSymbol(memberDeclaration, cancellationToken)?.IsStatic == true;
            }
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

        private struct ArgumentListPair
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
