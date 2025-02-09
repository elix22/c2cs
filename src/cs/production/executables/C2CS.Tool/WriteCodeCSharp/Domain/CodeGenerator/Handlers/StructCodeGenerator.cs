// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.

using System.Collections.Immutable;
using System.Linq;
using C2CS.Data.CSharp.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;

namespace C2CS.WriteCodeCSharp.Domain.CodeGenerator.Handlers;

public class StructCodeGenerator : GenerateCodeHandler<CSharpStruct>
{
    public StructCodeGenerator(
        ILogger<StructCodeGenerator> logger)
        : base(logger)
    {
    }

    protected override SyntaxNode GenerateCode(CSharpCodeGeneratorContext context, CSharpStruct node)
    {
        return Struct(context, node, false);
    }

    private StructDeclarationSyntax Struct(CSharpCodeGeneratorContext context, CSharpStruct @struct, bool isNested)
    {
        var memberSyntaxes = StructMembers(context, @struct.Name, @struct.Fields, @struct.NestedStructs);
        var memberStrings = memberSyntaxes.Select(x => x.ToFullString());
        var members = string.Join("\n\n", memberStrings);
        var attributesString = context.GenerateCodeAttributes(@struct.Attributes);

        var code = $@"
{attributesString}
[StructLayout(LayoutKind.Explicit, Size = {@struct.SizeOf}, Pack = {@struct.AlignOf})]
public struct {@struct.Name}
{{
	{members}
}}
";

        if (isNested)
        {
            code = code.Trim();
        }

        var member = context.ParseMemberCode<StructDeclarationSyntax>(code);
        return member;
    }

    private MemberDeclarationSyntax[] StructMembers(
        CSharpCodeGeneratorContext context,
        string structName,
        ImmutableArray<CSharpStructField> fields,
        ImmutableArray<CSharpStruct> nestedStructs)
    {
        var builder = ImmutableArray.CreateBuilder<MemberDeclarationSyntax>();

        StructFields(context, structName, fields, builder);

        foreach (var nestedStruct in nestedStructs)
        {
            var syntax = Struct(context, nestedStruct, true);
            builder.Add(syntax);
        }

        var structMembers = builder.ToArray();
        return structMembers;
    }

    private void StructFields(
        CSharpCodeGeneratorContext context,
        string structName,
        ImmutableArray<CSharpStructField> fields,
        ImmutableArray<MemberDeclarationSyntax>.Builder builder)
    {
        foreach (var field in fields)
        {
            if (field.TypeInfo.IsArray)
            {
                var fieldMember = EmitStructFieldFixedBuffer(context, field);
                builder.Add(fieldMember);

                var methodMember = StructFieldFixedBufferProperty(
                    context, structName, field);
                builder.Add(methodMember);
            }
            else
            {
                var fieldMember = StructField(context, field);
                builder.Add(fieldMember);
            }
        }
    }

    private FieldDeclarationSyntax StructField(
        CSharpCodeGeneratorContext context,
        CSharpStructField field)
    {
        var attributesString = context.GenerateCodeAttributes(field.Attributes);

        var code = $@"
{attributesString}
[FieldOffset({field.OffsetOf})] // size = {field.TypeInfo.SizeOf}
public {field.TypeInfo.Name} {field.Name};
".Trim();

        var member = context.ParseMemberCode<FieldDeclarationSyntax>(code);
        return member;
    }

    private FieldDeclarationSyntax EmitStructFieldFixedBuffer(
        CSharpCodeGeneratorContext context,
        CSharpStructField field)
    {
        var attributesString = context.GenerateCodeAttributes(field.Attributes);

        var code = $@"
{attributesString}
[FieldOffset({field.OffsetOf})] // size = {field.TypeInfo.SizeOf}
public fixed byte {field.BackingFieldName}[{field.TypeInfo.SizeOf}]; // {field.TypeInfo.OriginalName}
".Trim();

        return context.ParseMemberCode<FieldDeclarationSyntax>(code);
    }

    private PropertyDeclarationSyntax StructFieldFixedBufferProperty(
        CSharpCodeGeneratorContext context,
        string structName,
        CSharpStructField field)
    {
        string code;

        if (field.TypeInfo.Name == "CString")
        {
            code = $@"
public string {field.Name}
{{
	get
	{{
		fixed ({structName}*@this = &this)
		{{
			var pointer = &@this->{field.BackingFieldName}[0];
            var cString = new CString(pointer);
            return Runtime.CString.ToString(cString);
		}}
	}}
}}
".Trim();
        }
        else if (field.TypeInfo.Name == "CStringWide")
        {
            code = $@"
public string {field.Name}
{{
	get
	{{
		fixed ({structName}*@this = &this)
		{{
			var pointer = &@this->{field.BackingFieldName}[0];
            var cString = new CStringWide(pointer);
            return Runtime.StringWide.ToString(cString);
		}}
	}}
}}
".Trim();
        }
        else
        {
            var fieldTypeName = field.TypeInfo.Name;
            var elementType = fieldTypeName[..^1];
            if (elementType.EndsWith('*'))
            {
                elementType = "nint";
            }

            code = $@"
public Span<{elementType}> {field.Name}
{{
	get
	{{
		fixed ({structName}*@this = &this)
		{{
			var pointer = &@this->{field.BackingFieldName}[0];
			var span = new Span<{elementType}>(pointer, {field.TypeInfo.ArraySizeOf});
			return span;
		}}
	}}
}}
".Trim();
        }

        return context.ParseMemberCode<PropertyDeclarationSyntax>(code);
    }
}
