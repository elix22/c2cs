// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;

namespace C2CS.Data.CSharp.Model;

public sealed record CSharpAbstractSyntaxTree
{
    public ImmutableArray<CSharpFunction> Functions { get; init; }

    public ImmutableArray<CSharpFunctionPointer> FunctionPointers { get; init; }

    public ImmutableArray<CSharpStruct> Structs { get; init; }

    public ImmutableArray<CSharpAliasType> AliasStructs { get; init; }

    public ImmutableArray<CSharpOpaqueType> OpaqueStructs { get; init; }

    public ImmutableArray<CSharpEnum> Enums { get; init; }

    public ImmutableArray<CSharpMacroObject> MacroObjects { get; init; }

    public ImmutableArray<CSharpConstant> Constants { get; init; }
}
