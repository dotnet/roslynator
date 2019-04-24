// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about invocation expression in an expression statement.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SimpleMemberInvocationStatementInfo
    {
        private readonly SimpleMemberInvocationExpressionInfo _info;

        private SimpleMemberInvocationStatementInfo(in SimpleMemberInvocationExpressionInfo info)
        {
            _info = info;
        }

        /// <summary>
        /// The invocation expression.
        /// </summary>
        public InvocationExpressionSyntax InvocationExpression => _info.InvocationExpression;

        /// <summary>
        /// The member access expression.
        /// </summary>
        public MemberAccessExpressionSyntax MemberAccessExpression => _info.MemberAccessExpression;

        /// <summary>
        /// The expression that contains the member being invoked.
        /// </summary>
        public ExpressionSyntax Expression => _info.Expression;

        /// <summary>
        /// The name of the member being invoked.
        /// </summary>
        public SimpleNameSyntax Name => _info.Name;

        /// <summary>
        /// The argument list.
        /// </summary>
        public ArgumentListSyntax ArgumentList => _info.ArgumentList;

        /// <summary>
        /// A list of arguments.
        /// </summary>
        public SeparatedSyntaxList<ArgumentSyntax> Arguments => _info.Arguments;

        /// <summary>
        /// The name of the member being invoked.
        /// </summary>
        public string NameText => _info.NameText;

        /// <summary>
        /// The expression statement that contains the invocation expression.
        /// </summary>
        public ExpressionStatementSyntax Statement
        {
            get { return (ExpressionStatementSyntax)InvocationExpression?.Parent; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success => _info.Success;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Statement); }
        }

        internal static SimpleMemberInvocationStatementInfo Create(
            SyntaxNode node,
            bool allowMissing = false)
        {
            switch (node)
            {
                case ExpressionStatementSyntax expressionStatement:
                    return Create(expressionStatement, allowMissing);
                case InvocationExpressionSyntax invocationExpression:
                    return Create(invocationExpression, allowMissing);
            }

            return default;
        }

        internal static SimpleMemberInvocationStatementInfo Create(
            ExpressionStatementSyntax expressionStatement,
            bool allowMissing = false)
        {
            if (!(expressionStatement?.Expression is InvocationExpressionSyntax invocationExpression))
                return default;

            return CreateImpl(invocationExpression, allowMissing);
        }

        internal static SimpleMemberInvocationStatementInfo Create(
            InvocationExpressionSyntax invocationExpression,
            bool allowMissing = false)
        {
            if (!invocationExpression.IsParentKind(SyntaxKind.ExpressionStatement))
                return default;

            return CreateImpl(invocationExpression, allowMissing);
        }

        private static SimpleMemberInvocationStatementInfo CreateImpl(InvocationExpressionSyntax invocationExpression, bool allowMissing)
        {
            SimpleMemberInvocationExpressionInfo info = SimpleMemberInvocationExpressionInfo.Create(invocationExpression, allowMissing);

            return new SimpleMemberInvocationStatementInfo(info);
        }
    }
}
