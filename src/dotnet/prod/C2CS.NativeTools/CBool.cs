﻿// Copyright (c) Lucas Girouard-Stranks (https://github.com/lithiumtoast). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///     A boolean value type with the same memory layout as a <see cref="byte" /> in both managed and unmanaged
///     contexts; equivalent to a standard bool (_Bool) found in C/C++/ObjC where <c>0</c> is <c>false</c> and any other
///     value is <c>true</c>.
/// </summary>
[SuppressMessage("ReSharper", "CheckNamespace", Justification = "Wants to be a builtin type.")]
[SuppressMessage("Microsoft.Design", "CA1050:DeclareTypesInNamespaces", Justification = "Wants to be a builtin type.")]
public readonly struct CBool
{
    private readonly byte _value;

    private CBool(bool value)
    {
        _value = Convert.ToByte(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="bool" /> to a <see cref="CBool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="CBool" />.</returns>
    public static implicit operator CBool(bool value)
    {
        return new(value);
    }

    /// <summary>
    ///     Converts the specified <see cref="CBool" /> to a <see cref="bool" />.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="bool" />.</returns>
    public static implicit operator bool(CBool value)
    {
        return Convert.ToBoolean(value._value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Convert.ToBoolean(_value).ToString();
    }
}
