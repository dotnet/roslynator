// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.Formatting.CodeFixes.CSharp;
using Roslynator.Testing.CSharp;
using Xunit;

namespace Roslynator.Formatting.CSharp.Tests;

public class RCS1270FixBracketFormattingOfBinaryExpressionTests
    : AbstractCSharpDiagnosticVerifier<FixBracketFormattingOfBinaryExpressionAnalyzer, BinaryExpressionCodeFixProvider>
{
    public override DiagnosticDescriptor Descriptor { get; } = DiagnosticRules.FixBracketFormattingOfBinaryExpression;

    public override CSharpTestOptions Options =>
        base.Options.AddConfigOption(ConfigOptionKeys.TargetBracesStyle, "both");

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            { 
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(x &&
                        y &&
                        z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x &&
                        y &&
                        z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if [|(x
                        && y
                        && z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x
                        && y
                        && z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_3()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (x && y)
                    {
                    }
                    else if [|(x
                        && y
                        && z)|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (x && y)
                    {
                    }
                    else if (
                        x
                        && y
                        && z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task TestNoDiagnostic()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (
                        x
                        && y
                        && z
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task TestNoDiagnostic_SingleLine()
    {
        await VerifyNoDiagnosticAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;

                    if (x && y && z)
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_for_nested()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    if [|(x
                        && [|(y
                            || w)|]
                        && (z || v))|]
                    {
                    }
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    if (
                        x
                        && (
                            y
                            || w
                        )
                        && (z || v)
                    )
                    {
                    }
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_not_if_case()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = [|(x
                        && [|(y
                            || w)|]
                        && (z || v))|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = (
                        x
                        && (
                            y
                            || w
                        )
                        && (z || v)
                    );
                }
            }
            """
        );
    }

    [Fact, Trait(Traits.Analyzer, DiagnosticIdentifiers.FixBracketFormattingOfBinaryExpression)]
    public async Task Inserts_new_line_Before_and_After_binary_expression_not_if_case2()
    {
        await VerifyDiagnosticAndFixAsync(
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = 
                        [|(x
                            && [|(y
                                || w)|]
                            && (z || v))|];
                }
            }
            """,
            """
            class C
            {
                void M()
                {
                    bool x = false;
                    bool y = false;
                    bool z = false;
                    bool w = false;
                    bool v = false;

                    bool result = 
                        (
                            x
                            && (
                                y
                                || w
                            )
                            && (z || v)
                        );
                }
            }
            """
        );
    }
}