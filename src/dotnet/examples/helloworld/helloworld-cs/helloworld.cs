//-------------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the following tool:
//        https://github.com/lithiumtoast/c2cs
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ReSharper disable All
//-------------------------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

public static unsafe partial class helloworld
{
    private const string LibraryName = "helloworld";

    // FunctionExtern @ library.h:4
    [DllImport(LibraryName, EntryPoint = "hello_world", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hello_world();
}