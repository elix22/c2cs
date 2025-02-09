// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
// ReSharper disable PropertyCanBeMadeInitOnly.Global

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class CNodeAttribute : Attribute
{
    public string Kind { get; set; } = string.Empty;

    public string Platform { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
}
