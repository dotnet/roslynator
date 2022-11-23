// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp;

internal readonly struct ArgumentNullCheckAnalysis
{
    private ArgumentNullCheckAnalysis(ArgumentNullCheckStyle style, string name, bool success)
    {
        Style = style;
        Name = name;
        Success = success;
    }

    public ArgumentNullCheckStyle Style { get; }
    public string Name { get; }
    public bool Success { get; }

    public static ArgumentNullCheckAnalysis Create(
        StatementSyntax statement,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return Create(statement, semanticModel, name: null, cancellationToken);
    }

    public static ArgumentNullCheckAnalysis Create(
        StatementSyntax statement,
        SemanticModel semanticModel,
        string name,
        CancellationToken cancellationToken = default)
    {
        if (statement is IfStatementSyntax ifStatement)
        {
            var style = ArgumentNullCheckStyle.None;
            string identifier = null;
            var success = false;

            if (ifStatement.SingleNonBlockStatementOrDefault() is ThrowStatementSyntax throwStatement
                && throwStatement.Expression is ObjectCreationExpressionSyntax objectCreation)
            {
                NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(
                    ifStatement.Condition,
                    semanticModel,
                    NullCheckStyles.EqualsToNull | NullCheckStyles.IsNull,
                    cancellationToken: cancellationToken);

                if (nullCheck.Success)
                {
                    style = ArgumentNullCheckStyle.IfStatement;

                    if (nullCheck.Expression is IdentifierNameSyntax identifierName)
                    {
                        identifier = identifierName.Identifier.ValueText;

                        if (name is null
                            || string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                        {
                            if (semanticModel
                                .GetSymbol(objectCreation, cancellationToken)?
                                .ContainingType?
                                .HasMetadataName(MetadataNames.System_ArgumentNullException) == true)
                            {
                                success = true;
                            }
                        }
                    }
                }
            }

            return new ArgumentNullCheckAnalysis(style, identifier, success);
        }
        else
        {
            return CreateFromArgumentNullExceptionThrowIfNullCheck(statement, semanticModel, name, cancellationToken);
        }
    }

    private static ArgumentNullCheckAnalysis CreateFromArgumentNullExceptionThrowIfNullCheck(
        StatementSyntax statement,
        SemanticModel semanticModel,
        string name,
        CancellationToken cancellationToken)
    {
        var style = ArgumentNullCheckStyle.None;
        string identifier = null;
        var success = false;

        if (statement is ExpressionStatementSyntax expressionStatement)
        {
            SimpleMemberInvocationStatementInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationStatementInfo(expressionStatement);

            if (invocationInfo.Success
                && string.Equals(invocationInfo.NameText, "ThrowIfNull", StringComparison.Ordinal)
                && semanticModel
                    .GetSymbol(invocationInfo.InvocationExpression, cancellationToken)?
                    .ContainingType?
                    .HasMetadataName(MetadataNames.System_ArgumentNullException) == true)
            {
                style = ArgumentNullCheckStyle.ThrowIfNullMethod;

                if (invocationInfo.Arguments.SingleOrDefault(shouldThrow: false)?.Expression is IdentifierNameSyntax identifierName)
                {
                    identifier = identifierName.Identifier.ValueText;

                    if (name is null
                        || string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                    {
                        success = true;
                    }
                }
            }
        }

        return new ArgumentNullCheckAnalysis(style, identifier, success);
    }

    public static bool IsArgumentNullExceptionThrowIfNullCheck(
        StatementSyntax statement,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return CreateFromArgumentNullExceptionThrowIfNullCheck(statement, semanticModel, null, cancellationToken).Success;
    }

    public static bool IsArgumentNullCheck(
        StatementSyntax statement,
        SemanticModel semanticModel,
        CancellationToken cancellationToken = default)
    {
        return IsArgumentNullCheck(statement, semanticModel, name: null, cancellationToken);
    }

    public static bool IsArgumentNullCheck(
        StatementSyntax statement,
        SemanticModel semanticModel,
        string name,
        CancellationToken cancellationToken = default)
    {
        return Create(statement, semanticModel, name, cancellationToken).Success;
    }
}
