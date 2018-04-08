// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;
using static Roslynator.CSharp.CSharpSnippets;
using static Roslynator.CSharp.CSharpTypeFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TypeDeclarationCodeFixProvider))]
    [Shared]
    public class TypeDeclarationCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.TypeDefinesEqualityOperatorButDoesNotOverrideObjectEquals,
                    CompilerDiagnosticIdentifiers.TypeDefinesEqualityOperatorButDoesNotOverrideObjectGetHashCode,
                    CompilerDiagnosticIdentifiers.TypeOverridesObjectEqualsButDoesNotOverrideObjectGetHashCode);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsAnyCodeFixEnabled(
                CodeFixIdentifiers.DefineObjectEquals,
                CodeFixIdentifiers.DefineObjectGetHashCode))
            {
                return;
            }

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out TypeDeclarationSyntax typeDeclaration))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.TypeDefinesEqualityOperatorButDoesNotOverrideObjectEquals:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.DefineObjectEquals))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken) as ITypeSymbol;

                            if (typeSymbol?.IsErrorType() != false)
                                break;

                            CodeAction codeAction = CodeAction.Create(
                                "Override object.Equals",
                                cancellationToken =>
                                {
                                    TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, typeDeclaration.Identifier.SpanStart);

                                    MethodDeclarationSyntax methodDeclaration = ObjectEqualsMethodDeclaration(type, semanticModel, typeDeclaration.OpenBraceToken.Span.End);

                                    TypeDeclarationSyntax newNode = MemberDeclarationInserter.Default.Insert(typeDeclaration, methodDeclaration);

                                    return context.Document.ReplaceNodeAsync(typeDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case CompilerDiagnosticIdentifiers.TypeDefinesEqualityOperatorButDoesNotOverrideObjectGetHashCode:
                    case CompilerDiagnosticIdentifiers.TypeOverridesObjectEqualsButDoesNotOverrideObjectGetHashCode:
                        {
                            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.DefineObjectGetHashCode))
                                break;

                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            MethodDeclarationSyntax methodDeclaration = ObjectGetHashCodeMethodDeclaration(semanticModel, typeDeclaration.CloseBraceToken.Span.End);

                            CodeAction codeAction = CodeAction.Create(
                                "Override object.GetHashCode",
                                cancellationToken =>
                                {
                                    TypeDeclarationSyntax newNode = MemberDeclarationInserter.Default.Insert(typeDeclaration, methodDeclaration);

                                    return context.Document.ReplaceNodeAsync(typeDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static MethodDeclarationSyntax ObjectEqualsMethodDeclaration(
            TypeSyntax type,
            SemanticModel semanticModel,
            int position,
            string parameterName = "obj",
            string localName = "other")
        {
            return MethodDeclaration(
                Modifiers.PublicOverride(),
                BoolType(),
                Identifier("Equals"),
                ParameterList(Parameter(ObjectType(), parameterName)),
                Block(
                    IfNotReturnFalse(
                        IsPatternExpression(
                            IdentifierName(parameterName),
                            DeclarationPattern(
                                type,
                                SingleVariableDesignation(Identifier(localName))))),
                    ThrowNewNotImplementedExceptionStatement(semanticModel, position)));
        }

        private static MethodDeclarationSyntax ObjectGetHashCodeMethodDeclaration(SemanticModel semanticModel, int position)
        {
            return MethodDeclaration(
                Modifiers.PublicOverride(),
                IntType(),
                Identifier("GetHashCode"),
                ParameterList(),
                Block(ThrowNewNotImplementedExceptionStatement(semanticModel, position)));
        }
    }
}
