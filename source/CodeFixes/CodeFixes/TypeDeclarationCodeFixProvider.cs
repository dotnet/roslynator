// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using static Roslynator.CSharp.CSharpSyntaxParts;

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

                                    MethodDeclarationSyntax methodDeclaration = ObjectEqualsMethodDeclaration(type);

                                    TypeDeclarationSyntax newNode = typeDeclaration.InsertMember(methodDeclaration, MemberDeclarationComparer.ByKind);

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

                            MethodDeclarationSyntax methodDeclaration = ObjectGetHashCodeMethodDeclaration();

                            CodeAction codeAction = CodeAction.Create(
                                "Override object.GetHashCode",
                                cancellationToken =>
                                {
                                    TypeDeclarationSyntax newNode = typeDeclaration.InsertMember(methodDeclaration, MemberDeclarationComparer.ByKind);

                                    return context.Document.ReplaceNodeAsync(typeDeclaration, newNode, cancellationToken);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }
    }
}
