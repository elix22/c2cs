// Copyright (c) Bottlenose Labs Inc. (https://github.com/bottlenoselabs). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory for full license information.
// <auto-generated />

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS1591

/// <summary>
///     Utility methods for dealing with native C libraries.
/// </summary>
public static class CLibrary
{
    private static bool IsWindows
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsWindows();
#elif NETFRAMEWORK || NETSTANDARD || NETCOREAPP
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
            false;
#endif
    }

    private static bool IsDarwin
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsMacOS() ||
            OperatingSystem.IsMacCatalyst() ||
            OperatingSystem.IsIOS() ||
            OperatingSystem.IsTvOS() ||
            OperatingSystem.IsWatchOS();
#elif NETFRAMEWORK || NETSTANDARD || NETCOREAPP
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
            false;
#endif
    }

    private static bool IsLinux
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get =>
#if NET5_0_OR_GREATER
            OperatingSystem.IsLinux() ||
            OperatingSystem.IsFreeBSD() ||
            OperatingSystem.IsAndroid();
#elif NETFRAMEWORK || NETSTANDARD || NETCOREAPP
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
            false;
#endif
    }

    /// <summary>
    ///     Loads a C shared library (`.dll`/`.dylib`/`.so`) into the application's memory space given the C
    ///     library's file name, partially qualified file path, or a fully qualified file path.
    /// </summary>
    /// <param name="name">The C library's file name, partially qualified file path, or a fully qualified file path.</param>
    /// <returns>A <see cref="IntPtr" /> handle if the C library was loaded; otherwise, a <see cref="IntPtr.Zero" /> handle.</returns>
    /// <remarks>
    ///     <para>
    ///         If the library was already loaded, calling <see cref="Load" /> returns the handle of the previous loaded
    ///         library and the reference count for the handle is incremented.
    ///     </para>
    ///     <para>You must call <see cref="Free" /> when finished using a handle of a dynamically loaded library.</para>
    /// </remarks>
    public static IntPtr Load(string name)
    {
        if (IsLinux)
        {
            return libdl.dlopen(name, 0x101); // RTLD_GLOBAL | RTLD_LAZY
        }

        if (IsWindows)
        {
            return Kernel32.LoadLibrary(name);
        }

        if (IsDarwin)
        {
            return libSystem.dlopen(name, 0x101); // RTLD_GLOBAL | RTLD_LAZY
        }

        return IntPtr.Zero;
    }

    /// <summary>
    ///     Unloads a C shared library from the application's memory space given the library's handle.
    /// </summary>
    /// <param name="handle">The library's handle previously created by calling <see cref="Load" />.</param>
    /// <remarks>
    ///     <para>
    ///         If the library was loaded multiple times, the reference count for the handle is decremented upon calling
    ///         <see cref="Free" /> and the library is not yet unloaded from memory.
    ///     </para>
    /// </remarks>
    public static void Free(IntPtr handle)
    {
        if (IsLinux)
        {
            libdl.dlclose(handle);
        }

        if (IsWindows)
        {
            Kernel32.FreeLibrary(handle);
        }

        if (IsDarwin)
        {
            libSystem.dlclose(handle);
        }
    }

    public static IntPtr GetExport(IntPtr handle, string symbolName)
    {
        if (IsLinux)
        {
            return libdl.dlsym(handle, symbolName);
        }

        if (IsWindows)
        {
            return Kernel32.GetProcAddress(handle, symbolName);
        }

        if (IsDarwin)
        {
            return libSystem.dlsym(handle, symbolName);
        }

        return IntPtr.Zero;
    }

    [System.Security.SuppressUnmanagedCodeSecurity]
#pragma warning disable CS8981
    private static class libdl
#pragma warning restore CS8981
    {
        private const string LibraryName = "libdl";

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)] // silence CA5392
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)] // silence CA5392
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)] // silence CA5392
        public static extern int dlclose(IntPtr handle);
    }

    [System.Security.SuppressUnmanagedCodeSecurity]
    private static class libSystem
    {
        private const string LibraryName = "libSystem";

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)] // silence CA5392
        public static extern IntPtr dlopen(string fileName, int flags);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)] // silence CA5392
        public static extern IntPtr dlsym(IntPtr handle, string name);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)] // silence CA5392
        public static extern int dlclose(IntPtr handle);
    }

    [System.Security.SuppressUnmanagedCodeSecurity]
    private static class Kernel32
    {
        private const string LibraryName = "kernel32";

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi,
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr LoadLibrary(string fileName);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi,
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport(LibraryName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern int FreeLibrary(IntPtr module);
    }
}
