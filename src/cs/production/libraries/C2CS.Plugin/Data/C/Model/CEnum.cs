// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace C2CS.Data.C.Model;

// NOTE: Properties are required for System.Text.Json serialization
public record CEnum : CNodeWithLocation
{
    [JsonPropertyName("type_integer")]
    public CTypeInfo IntegerTypeInfo { get; set; } = null!;

    [JsonPropertyName("values")]
    public ImmutableArray<CEnumValue> Values { get; set; } = ImmutableArray<CEnumValue>.Empty;

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"Enum '{Name}': {IntegerTypeInfo} @ {Location}";
    }
}
