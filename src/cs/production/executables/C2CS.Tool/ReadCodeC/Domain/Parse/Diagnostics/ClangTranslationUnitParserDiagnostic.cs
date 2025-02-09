// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

namespace C2CS.ReadCodeC.Domain.Parse.Diagnostics;

public sealed class ClangTranslationUnitParserDiagnostic : Diagnostic
{
    public ClangTranslationUnitParserDiagnostic(DiagnosticSeverity severity, string message)
        : base(severity, message)
    {
    }
}
