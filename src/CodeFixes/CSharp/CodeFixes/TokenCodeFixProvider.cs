// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(TokenCodeFixProvider))]
    [Shared]
    public sealed class TokenCodeFixProvider : CompilerDiagnosticCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.CS0023_OperatorCannotBeAppliedToOperand,
                    CompilerDiagnosticIdentifiers.CS0267_PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid,
                    CompilerDiagnosticIdentifiers.CS1750_ValueCannotBeUsedAsDefaultParameter,
                    CompilerDiagnosticIdentifiers.CS0126_ObjectOfTypeConvertibleToTypeIsRequired,
                    CompilerDiagnosticIdentifiers.CS1031_TypeExpected,
                    CompilerDiagnosticIdentifiers.CS1597_SemicolonAfterMethodOrAccessorBlockIsNotValid,
                    CompilerDiagnosticIdentifiers.CS0030_CannotConvertType,
                    CompilerDiagnosticIdentifiers.CS1737_OptionalParametersMustAppearAfterAllRequiredParameters,
                    CompilerDiagnosticIdentifiers.CS8632_AnnotationForNullableReferenceTypesShouldOnlyBeUsedWithinNullableAnnotationsContext,
                    CompilerDiagnosticIdentifiers.CS8618_NonNullableMemberIsUninitialized,
                    CompilerDiagnosticIdentifiers.CS8403_MethodWithIteratorBlockMustBeAsyncToReturnIAsyncEnumerableOfT,
                    CompilerDiagnosticIdentifiers.CS8602_DereferenceOfPossiblyNullReference,
                    CompilerDiagnosticIdentifiers.CS8604_PossibleNullReferenceArgumentForParameter
                    );
            }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindToken(root, context.Span.Start, out SyntaxToken token))
                return;

            SyntaxKind kind = token.Kind();
            Diagnostic diagnostic = context.Diagnostics[0];
            Document document = context.Document;

            switch (diagnostic.Id)
            {
                case CompilerDiagnosticIdentifiers.CS0023_OperatorCannotBeAppliedToOperand:
                    {
                        if (kind == SyntaxKind.QuestionToken
                            && token.Parent is ConditionalAccessExpressionSyntax conditionalAccess)
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(conditionalAccess.Expression, context.CancellationToken);

                            if (typeSymbol?.IsErrorType() == false
                                && !typeSymbol.IsNullableType())
                            {
                                if (typeSymbol.IsValueType)
                                {
                                    if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveConditionalAccess, document, root.SyntaxTree))
                                    {
                                        CodeAction codeAction = CodeAction.Create(
                                            "Remove '?' operator",
                                            ct => document.WithTextChangeAsync(token.Span, "", ct),
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                                else if (typeSymbol.IsReferenceType)
                                {
                                    if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddArgumentList, document, root.SyntaxTree)
                                        && conditionalAccess.WhenNotNull is MemberBindingExpressionSyntax memberBindingExpression)
                                    {
                                        ConditionalAccessExpressionSyntax newNode = conditionalAccess.WithWhenNotNull(
                                            InvocationExpression(
                                                memberBindingExpression.WithoutTrailingTrivia(),
                                                ArgumentList().WithTrailingTrivia(memberBindingExpression.GetTrailingTrivia())));

                                        CodeAction codeAction = CodeAction.Create(
                                            "Add argument list",
                                            ct => document.ReplaceNodeAsync(conditionalAccess, newNode, ct),
                                            GetEquivalenceKey(diagnostic));

                                        context.RegisterCodeFix(codeAction, diagnostic);
                                    }
                                }
                            }
                        }

                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS0267_PartialModifierCanOnlyAppearImmediatelyBeforeClassStructInterfaceOrVoid:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.OrderModifiers, document, root.SyntaxTree))
                            break;

                        ModifiersCodeFixRegistrator.MoveModifier(context, diagnostic, token.Parent, token);
                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS1750_ValueCannotBeUsedAsDefaultParameter:
                    {
                        if (token.Parent is not ParameterSyntax parameter)
                            break;

                        ExpressionSyntax value = parameter.Default?.Value;

                        if (value == null)
                            break;

                        if (value.IsKind(SyntaxKind.NullLiteralExpression))
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReplaceNullLiteralExpressionWithDefaultValue, document, root.SyntaxTree))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                CodeFixRegistrator.ReplaceNullWithDefaultValue(context, diagnostic, value, semanticModel);
                            }
                        }
                        else if (!value.IsKind(SyntaxKind.DefaultExpression, SyntaxKind.DefaultLiteralExpression))
                        {
                            if (IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeParameterType, document, root.SyntaxTree))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(value, context.CancellationToken);

                                if (!typeSymbol.IsKind(SymbolKind.ErrorType))
                                {
                                    CodeFixRegistrator.ChangeType(context, diagnostic, parameter.Type, typeSymbol, semanticModel);
                                }
                            }
                        }

                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS0126_ObjectOfTypeConvertibleToTypeIsRequired:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ReturnDefaultValue, document, root.SyntaxTree))
                            break;

                        if (!token.IsKind(SyntaxKind.ReturnKeyword))
                            break;

                        if (!token.IsParentKind(SyntaxKind.ReturnStatement))
                            break;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ISymbol symbol = semanticModel.GetEnclosingSymbol(token.SpanStart, context.CancellationToken);

                        if (symbol == null)
                            break;

                        SymbolKind symbolKind = symbol.Kind;

                        ITypeSymbol typeSymbol = null;

                        if (symbolKind == SymbolKind.Method)
                        {
                            var methodSymbol = (IMethodSymbol)symbol;

                            typeSymbol = methodSymbol.ReturnType;

                            if (methodSymbol.IsAsync
                                && (typeSymbol is INamedTypeSymbol namedTypeSymbol))
                            {
                                ImmutableArray<ITypeSymbol> typeArguments = namedTypeSymbol.TypeArguments;

                                if (typeArguments.Any())
                                    typeSymbol = typeArguments[0];
                            }
                        }
                        else if (symbolKind == SymbolKind.Property)
                        {
                            typeSymbol = ((IPropertySymbol)symbol).Type;
                        }
                        else
                        {
                            Debug.Fail(symbolKind.ToString());
                        }

                        if (typeSymbol == null)
                            break;

                        if (typeSymbol.Kind == SymbolKind.ErrorType)
                            break;

                        if (!typeSymbol.SupportsExplicitDeclaration())
                            break;

                        var returnStatement = (ReturnStatementSyntax)token.Parent;

                        CodeAction codeAction = CodeAction.Create(
                            "Return default value",
                            ct =>
                            {
                                ExpressionSyntax expression = typeSymbol.GetDefaultValueSyntax(document.GetDefaultSyntaxOptions());

                                if (expression.IsKind(SyntaxKind.DefaultExpression)
                                    && document.SupportsLanguageFeature(CSharpLanguageFeature.DefaultLiteral))
                                {
                                    expression = CSharpFactory.DefaultLiteralExpression().WithTriviaFrom(expression);
                                }

                                ReturnStatementSyntax newNode = returnStatement.WithExpression(expression);

                                return document.ReplaceNodeAsync(returnStatement, newNode, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS1031_TypeExpected:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddMissingType, document, root.SyntaxTree))
                            break;

                        if (!token.IsKind(SyntaxKind.CloseParenToken))
                            break;

                        if (token.Parent is not DefaultExpressionSyntax defaultExpression)
                            break;

                        if (defaultExpression.Type is not IdentifierNameSyntax identifierName)
                            break;

                        if (!identifierName.IsMissing)
                            break;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        TypeInfo typeInfo = semanticModel.GetTypeInfo(defaultExpression, context.CancellationToken);

                        ITypeSymbol convertedType = typeInfo.ConvertedType;

                        if (convertedType?.SupportsExplicitDeclaration() != true)
                            break;

                        CodeAction codeAction = CodeAction.Create(
                            $"Add type '{SymbolDisplay.ToMinimalDisplayString(convertedType, semanticModel, defaultExpression.SpanStart, SymbolDisplayFormats.DisplayName)}'",
                            ct =>
                            {
                                TypeSyntax newType = convertedType.ToTypeSyntax()
                                    .WithTriviaFrom(identifierName)
                                    .WithFormatterAndSimplifierAnnotation();

                                return document.ReplaceNodeAsync(identifierName, newType, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS1597_SemicolonAfterMethodOrAccessorBlockIsNotValid:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveSemicolon, document, root.SyntaxTree))
                            break;

                        if (!token.IsKind(SyntaxKind.SemicolonToken))
                            break;

                        switch (token.Parent)
                        {
                            case MethodDeclarationSyntax methodDeclaration:
                                {
                                    BlockSyntax body = methodDeclaration.Body;

                                    if (body == null)
                                        break;

                                    CodeAction codeAction = CodeAction.Create(
                                        "Remove semicolon",
                                        ct =>
                                        {
                                            SyntaxTriviaList trivia = body
                                                .GetTrailingTrivia()
                                                .EmptyIfWhitespace()
                                                .AddRange(token.LeadingTrivia.EmptyIfWhitespace())
                                                .AddRange(token.TrailingTrivia);

                                            MethodDeclarationSyntax newNode = methodDeclaration
                                                .WithBody(body.WithTrailingTrivia(trivia))
                                                .WithSemicolonToken(default(SyntaxToken));

                                            return document.ReplaceNodeAsync(methodDeclaration, newNode, ct);
                                        },
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                    break;
                                }
                            case PropertyDeclarationSyntax propertyDeclaration:
                                {
                                    AccessorListSyntax accessorList = propertyDeclaration.AccessorList;

                                    if (accessorList == null)
                                        break;

                                    CodeAction codeAction = CodeAction.Create(
                                        "Remove semicolon",
                                        ct =>
                                        {
                                            SyntaxTriviaList trivia = accessorList
                                                .GetTrailingTrivia()
                                                .EmptyIfWhitespace()
                                                .AddRange(token.LeadingTrivia.EmptyIfWhitespace())
                                                .AddRange(token.TrailingTrivia);

                                            PropertyDeclarationSyntax newNode = propertyDeclaration
                                                .WithAccessorList(accessorList.WithTrailingTrivia(trivia))
                                                .WithSemicolonToken(default(SyntaxToken));

                                            return document.ReplaceNodeAsync(propertyDeclaration, newNode, ct);
                                        },
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                    break;
                                }
                            case AccessorDeclarationSyntax accessorDeclaration:
                                {
                                    BlockSyntax body = accessorDeclaration.Body;

                                    if (body == null)
                                        break;

                                    CodeAction codeAction = CodeAction.Create(
                                        "Remove semicolon",
                                        ct =>
                                        {
                                            SyntaxTriviaList trivia = body
                                                .GetTrailingTrivia()
                                                .EmptyIfWhitespace()
                                                .AddRange(token.LeadingTrivia.EmptyIfWhitespace())
                                                .AddRange(token.TrailingTrivia);

                                            AccessorDeclarationSyntax newNode = accessorDeclaration
                                                .WithBody(body.WithTrailingTrivia(trivia))
                                                .WithSemicolonToken(default(SyntaxToken));

                                            return document.ReplaceNodeAsync(accessorDeclaration, newNode, ct);
                                        },
                                        GetEquivalenceKey(diagnostic));

                                    context.RegisterCodeFix(codeAction, diagnostic);
                                    break;
                                }
                        }

                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS0030_CannotConvertType:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.ChangeForEachType, document, root.SyntaxTree))
                            break;

                        if (!token.IsKind(SyntaxKind.ForEachKeyword))
                            break;

                        if (token.Parent is not ForEachStatementSyntax forEachStatement)
                            break;

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ForEachStatementInfo info = semanticModel.GetForEachStatementInfo(forEachStatement);

                        ITypeSymbol typeSymbol = info.ElementType;

                        if (typeSymbol.SupportsExplicitDeclaration())
                            CodeFixRegistrator.ChangeType(context, diagnostic, forEachStatement.Type, typeSymbol, semanticModel, CodeFixIdentifiers.ChangeForEachType);

                        CodeFixRegistrator.ChangeTypeToVar(context, diagnostic, forEachStatement.Type, CodeFixIdentifiers.ChangeTypeToVar);
                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS1737_OptionalParametersMustAppearAfterAllRequiredParameters:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddDefaultValueToParameter, document, root.SyntaxTree))
                            break;

                        if (token.Parent is not BaseParameterListSyntax parameterList)
                            break;

                        SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

                        ParameterSyntax parameter = null;

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            ParameterSyntax p = parameters[i];

                            if (p.FullSpan.End <= token.SpanStart)
                            {
                                parameter = p;
                            }
                            else
                            {
                                break;
                            }
                        }

                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        IParameterSymbol parameterSymbol = semanticModel.GetDeclaredSymbol(parameter, context.CancellationToken);

                        ITypeSymbol typeSymbol = parameterSymbol.Type;

                        if (typeSymbol.Kind == SymbolKind.ErrorType)
                            break;

                        CodeAction codeAction = CodeAction.Create(
                            "Add default value",
                            ct =>
                            {
                                ExpressionSyntax defaultValue = typeSymbol.GetDefaultValueSyntax(document.GetDefaultSyntaxOptions());

                                ParameterSyntax newParameter = parameter
                                    .WithDefault(EqualsValueClause(defaultValue).WithTrailingTrivia(parameter.GetTrailingTrivia()))
                                    .WithoutTrailingTrivia()
                                    .WithFormatterAnnotation();

                                return document.ReplaceNodeAsync(parameter, newParameter, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS8632_AnnotationForNullableReferenceTypesShouldOnlyBeUsedWithinNullableAnnotationsContext:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.RemoveAnnotationForNullableReferenceTypes, document, root.SyntaxTree))
                            break;

                        if (!token.IsKind(SyntaxKind.QuestionToken))
                            return;

                        CodeAction codeAction = CodeAction.Create(
                            "Remove 'nullable' annotation",
                            ct =>
                            {
                                var textChange = new TextChange(token.Span, "");

                                return document.WithTextChangeAsync(textChange, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS8602_DereferenceOfPossiblyNullReference:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseNullForgivingOperator, document, root.SyntaxTree))
                            break;

                        Debug.Assert(token.IsKind(SyntaxKind.IdentifierToken), token.Kind().ToString());

                        if (!token.IsKind(SyntaxKind.IdentifierToken))
                            return;

                        if (!token.IsParentKind(SyntaxKind.IdentifierName))
                            return;

                        SyntaxNode node = token.Parent
                            .WalkUp(f => f.IsKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.InvocationExpression, SyntaxKind.ElementAccessExpression))
                            .Parent;

                        if (node.IsKind(SyntaxKind.ExpressionStatement)
                            && token.SpanStart == node.SpanStart)
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use null propagation operator",
                                ct =>
                                {
                                    var textChange = new TextChange(new TextSpan(token.Span.End, 0), "?");

                                    return document.WithTextChangeAsync(textChange, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }

                        return;
                    }
                case CompilerDiagnosticIdentifiers.CS8604_PossibleNullReferenceArgumentForParameter:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseNullForgivingOperator, document, root.SyntaxTree))
                            break;

                        Debug.Assert(token.IsKind(SyntaxKind.IdentifierToken), token.Kind().ToString());

                        if (!token.IsKind(SyntaxKind.IdentifierToken))
                            return;

                        if (!token.IsParentKind(SyntaxKind.IdentifierName))
                            return;

                        CodeAction codeAction = CodeAction.Create(
                            "Use null-forgiving operator",
                            ct =>
                            {
                                var identifierName = (IdentifierNameSyntax)token.Parent;

                                PostfixUnaryExpressionSyntax newExpression = SuppressNullableWarningExpression(identifierName.WithoutTrivia())
                                    .WithTriviaFrom(identifierName);

                                return document.ReplaceNodeAsync(identifierName, newExpression, ct);
                            },
                            GetEquivalenceKey(diagnostic));

                        context.RegisterCodeFix(codeAction, diagnostic);
                        return;
                    }
                case CompilerDiagnosticIdentifiers.CS8618_NonNullableMemberIsUninitialized:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.UseNullForgivingOperator, document, root.SyntaxTree))
                            break;

                        Debug.Assert(token.IsKind(SyntaxKind.IdentifierToken), token.Kind().ToString());

                        if (!token.IsKind(SyntaxKind.IdentifierToken))
                            return;

                        if (token.IsParentKind(SyntaxKind.PropertyDeclaration))
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Use null-forgiving operator",
                                ct =>
                                {
                                    var property = (PropertyDeclarationSyntax)token.Parent;

                                    PropertyDeclarationSyntax newProperty = property
                                        .WithoutTrailingTrivia()
                                        .WithInitializer(EqualsValueClause(SuppressNullableWarningExpression(NullLiteralExpression())))
                                        .WithSemicolonToken(SemicolonToken().WithTrailingTrivia(property.GetTrailingTrivia()));

                                    return document.ReplaceNodeAsync(property, newProperty, ct);
                                },
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                        }
                        else
                        {
                            SyntaxDebug.Assert(
                                (token.IsParentKind(SyntaxKind.VariableDeclarator)
                                    && token.Parent.IsParentKind(SyntaxKind.VariableDeclaration)
                                    && token.Parent.Parent.IsParentKind(SyntaxKind.FieldDeclaration, SyntaxKind.EventFieldDeclaration))
                                    || token.IsParentKind(SyntaxKind.ConstructorDeclaration),
                                token);

                            if (token.IsParentKind(SyntaxKind.VariableDeclarator)
                                && token.Parent.IsParentKind(SyntaxKind.VariableDeclaration)
                                && token.Parent.Parent.IsParentKind(SyntaxKind.FieldDeclaration))
                            {
                                CodeAction codeAction = CodeAction.Create(
                                    "Use null-forgiving operator",
                                    ct =>
                                    {
                                        var declarator = (VariableDeclaratorSyntax)token.Parent;

                                        VariableDeclaratorSyntax newDeclarator = declarator
                                            .WithoutTrailingTrivia()
                                            .WithInitializer(
                                                EqualsValueClause(SuppressNullableWarningExpression(NullLiteralExpression()))
                                                    .WithTrailingTrivia(declarator.GetTrailingTrivia()));

                                        return document.ReplaceNodeAsync(declarator, newDeclarator, ct);
                                    },
                                    GetEquivalenceKey(diagnostic));

                                context.RegisterCodeFix(codeAction, diagnostic);
                            }
                        }

                        break;
                    }
                case CompilerDiagnosticIdentifiers.CS8403_MethodWithIteratorBlockMustBeAsyncToReturnIAsyncEnumerableOfT:
                    {
                        if (!IsEnabled(diagnostic.Id, CodeFixIdentifiers.AddAsyncModifier, document, root.SyntaxTree))
                            break;

                        SyntaxDebug.Assert(token.IsKind(SyntaxKind.IdentifierToken), token);
                        SyntaxDebug.Assert(token.IsParentKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement), token.Parent);

                        if (token.IsParentKind(SyntaxKind.MethodDeclaration, SyntaxKind.LocalFunctionStatement))
                        {
                            ModifiersCodeFixRegistrator.AddModifier(
                                context,
                                diagnostic,
                                token.Parent,
                                SyntaxKind.AsyncKeyword,
                                additionalKey: CodeFixIdentifiers.AddAsyncModifier);
                        }

                        break;
                    }
            }
        }
    }
}
