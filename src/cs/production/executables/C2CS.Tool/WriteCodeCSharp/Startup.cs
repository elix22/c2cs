// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using C2CS.WriteCodeCSharp.Data;
using C2CS.WriteCodeCSharp.Domain.CodeGenerator;
using Microsoft.Extensions.DependencyInjection;

namespace C2CS.WriteCodeCSharp;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<FeatureWriteCodeCSharp>();
        services.AddSingleton<WriteCodeCSharpInputValidator>();

        AddGenerateCodeHandlers(services);
    }

    private static void AddGenerateCodeHandlers(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes().Where(p => !p.IsAbstract && typeof(GenerateCodeHandler).IsAssignableFrom(p))
            .ToImmutableArray();
        foreach (var type in types)
        {
            services.AddSingleton(type, type);
        }
    }
}
