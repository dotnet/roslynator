// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CodeGeneration.CSharp
{
    public class MethodGenerationContext
    {
        private IMethodSymbol _methodSymbol;

        internal MethodGenerationContext()
        {
            Statements = new List<StatementSyntax>();
            LocalNames = new HashSet<string>();
        }

        public List<StatementSyntax> Statements { get; }

        public HashSet<string> LocalNames { get; }

        public IMethodSymbol MethodSymbol
        {
            get { return _methodSymbol; }
            internal set
            {
                _methodSymbol = value;
                ParameterSymbol = _methodSymbol?.Parameters.Single();
            }
        }

        public string MethodName => MethodSymbol?.Name;

        public IParameterSymbol ParameterSymbol { get; private set; }

        public IPropertySymbol PropertySymbol { get; internal set; }

        public ITypeSymbol ParameterType => ParameterSymbol?.Type;

        public string ParameterName => ParameterSymbol?.Name;

        public ITypeSymbol PropertyType => PropertySymbol?.Type;

        public string PropertyName => PropertySymbol?.Name;

        public void AddStatement(StatementSyntax statement)
        {
            Statements.Add(statement);
        }

        public string CreateVariableName(string name)
        {
            name = StringUtility.ToCamelCase(name);

            name = NameGenerator.Default.EnsureUniqueName(name, LocalNames);

            if (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None)
                name = $"@{name}";

            LocalNames.Add(name);

            return name;
        }
    }
}
