// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using C2CS.Data.CSharp.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace C2CS.WriteCodeCSharp.Domain.CodeGenerator.Handlers;

public sealed class FunctionPointerCodeGenerator : GenerateCodeHandler<CSharpFunctionPointer>
{
    public FunctionPointerCodeGenerator(
        ILogger<FunctionPointerCodeGenerator> logger)
        : base(logger)
    {
    }

    protected override SyntaxNode GenerateCode(CSharpCodeGeneratorContext context, CSharpFunctionPointer node)
    {
        string code;
        if (context.Options.IsEnabledFunctionPointers)
        {
            code = GenerateCodeFunctionPointer(context, node);
        }
        else
        {
            code = GenerateCodeDelegate(context, node);
        }

        var result = context.ParseMemberCode<StructDeclarationSyntax>(code);
        return result;
    }

    private string GenerateCodeFunctionPointer(CSharpCodeGeneratorContext context, CSharpFunctionPointer node)
    {
        var attributesString = context.GenerateCodeAttributes(node.Attributes);
        var parameterTypesString = context.GenerateCodeParameters(node.Parameters, false);
        var parameterTypesAndReturnTypeString = string.IsNullOrEmpty(parameterTypesString) ? node.ReturnTypeInfo.Name : $"{parameterTypesString}, {node.ReturnTypeInfo.Name}";

        var code = $@"
{attributesString}
[StructLayout(LayoutKind.Sequential)]
public struct {node.Name}
{{
	public delegate* unmanaged <{parameterTypesAndReturnTypeString}> Pointer;
}}
";
        return code;
    }

    private string GenerateCodeDelegate(CSharpCodeGeneratorContext context, CSharpFunctionPointer node)
    {
        var attributesString = context.GenerateCodeAttributes(node.Attributes);
        var parameterTypesString = context.GenerateCodeParameters(node.Parameters);

        var code = $@"
{attributesString}
[StructLayout(LayoutKind.Sequential)]
public struct {node.Name}
{{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate {node.ReturnTypeInfo.Name} @delegate({parameterTypesString});

    public IntPtr Pointer;

    public {node.Name}(@delegate d)
     {{
         Pointer = Marshal.GetFunctionPointerForDelegate(d);
     }}
}}
";
        return code;
    }
}
