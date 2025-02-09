// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using C2CS.Options;

namespace C2CS.Tests.C;

public class TestReaderCCode : IReaderCCode
{
    public ReaderCCodeOptions Options { get; }

    public TestReaderCCode()
    {
        Options = CreateOptions();
    }

    private static ReaderCCodeOptions CreateOptions()
    {
        var result = new ReaderCCodeOptions
        {
            InputHeaderFilePath = "../../../../src/c/tests/_container_library/main.c",
            OutputAbstractSyntaxTreesFileDirectory = "./c/tests/_container_library/ast",
            UserIncludeDirectories = new[] { "../../../../src/c/production/pinvoke_helper/include" }.ToImmutableArray(),
        };

        var platforms = ImmutableArray<TargetPlatform>.Empty;
        var hostOperatingSystem = Native.OperatingSystem;
        if (hostOperatingSystem == NativeOperatingSystem.Windows)
        {
            platforms = new[]
            {
                TargetPlatform.aarch64_pc_windows_msvc,
                TargetPlatform.x86_64_pc_windows_msvc
            }.ToImmutableArray();
        }
        else if (hostOperatingSystem == NativeOperatingSystem.macOS)
        {
            platforms = new[]
            {
                TargetPlatform.aarch64_apple_darwin,
                TargetPlatform.aarch64_apple_ios,
                TargetPlatform.x86_64_apple_darwin
            }.ToImmutableArray();
        }
        else if (hostOperatingSystem == NativeOperatingSystem.Linux)
        {
            platforms = new[]
            {
                TargetPlatform.aarch64_unknown_linux_gnu,
                TargetPlatform.x86_64_unknown_linux_gnu
            }.ToImmutableArray();
        }

        var builder = ImmutableDictionary.CreateBuilder<TargetPlatform, ReaderCCodeOptionsPlatform>();
        foreach (var platform in platforms)
        {
            builder.Add(platform, new ReaderCCodeOptionsPlatform());
        }

        result.Platforms = builder.ToImmutable();

        return result;
    }
}
