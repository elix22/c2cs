# C2CS

C to C# bindings code generator. In go `.h` file, out come `.cs`.

## Background: Why?

### Problem

When creating applications with C# (especially games), it's sometimes necessary to dip down into C/C++ for better performance. However, maintaining the bindings becomes time consuming, error-prone, and in some cases quite tricky.

### Solution

Auto generate the bindings by parsing a C `.h` file, essentially transpiling the C API to C#. This includes all functions (`static`s in C#) and all types (`struct`s in C#). This is accomplished by using [libclang](https://clang.llvm.org/docs/Tooling.html) for C and [Roslyn](https://github.com/dotnet/roslyn) for C#. All naming is left as found in the header file of the C code.

## Developers: Building from Source

### Prerequisites

1. Download and install [.NET 5](https://dotnet.microsoft.com/download).
2. Clone the repository with submodules: `git clone --recurse-submodules https://github.com/lithiumtoast/c2cs.git`.

### Visual Studio / Rider / MonoDevelop

Open `./src/dotnet/C2CS.sln`

### Command Line Interface (CLI)

`dotnet build ./src/dotnet/C2CS.sln`

## Using C2CS

```
C2CS.CommandLine:
  C# Platform Invoke Code Generator

Usage:
  C2CS.CommandLine [options]

Options:
  -i, --inputFilePath <inputFilePath> (REQUIRED)      File path of the input .h file.
  -o, --outputFilePath <outputFilePath> (REQUIRED)    File path of the output .cs file.
  -u, --unattended                                    Don't ask for further input.
  -l, --libraryName <libraryName>                     The name of the library. Default value is the file name of the input file path.
  -s, --includeDirectories <includeDirectories>       Include directories to use for parsing C code.
  -d, --defineMacros <defineMacros>                   Macros to define for parsing C code.
  -a, --additionalArgs <additionalArgs>               Additional arguments for parsing C code.
  --version                                           Show version information
  -?, -h, --help                                      Show help and usage information
```

## Examples

### sokol_gfx: https://github.com/floooh/sokol

Input:
```bash
-i
"/PATH/TO/sokol/sokol_gfx.h"
-o
"./sokol_gfx.cs"
-u
-l
"sokol_gfx"
```

```c
...

SOKOL_GFX_API_DECL void sg_draw(int base_element, int num_elements, int num_instances);
SOKOL_GFX_API_DECL void sg_end_pass(void);
SOKOL_GFX_API_DECL void sg_commit(void);

...
```

Output:
```cs
// <auto-generated/>
// ReSharper disable All

using System.Runtime.InteropServices;

public static unsafe class sokol_gfx
{
    ...

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void sg_draw(int base_element, int num_elements, int num_instances);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void sg_end_pass();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void sg_commit();

    ...
}    

```

### Soloud: https://github.com/jarikomppa/soloud

Input:
```bash
-i
"/PATH/TO/SOLOUD/include/soloud_c.h"
-o
"./soloud.cs"
-u
-l
"soloud"
```

```c
...

void Soloud_destroy(Soloud * aSoloud);
Soloud * Soloud_create();
int Soloud_init(Soloud * aSoloud);
int Soloud_initEx(Soloud * aSoloud, unsigned int aFlags /* = Soloud::CLIP_ROUNDOFF */, unsigned int aBackend /* = Soloud::AUTO */, unsigned int aSamplerate /* = Soloud::AUTO */, unsigned int aBufferSize /* = Soloud::AUTO */, unsigned int aChannels /* = 2 */);
void Soloud_deinit(Soloud * aSoloud);

...
```

Output:
```cs
// <auto-generated/>
// ReSharper disable All

using System.Runtime.InteropServices;

public static unsafe class soloud
{
    private const string LibraryName = "soloud";

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Soloud_destroy(Soloud* aSoloud);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Soloud* Soloud_create();

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Soloud_init(Soloud* aSoloud);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Soloud_initEx(Soloud* aSoloud, uint aFlags, uint aBackend, uint aSamplerate, uint aBufferSize, uint aChannels);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Soloud_deinit(Soloud* aSoloud);

    ...
}
```

### libclang: https://github.com/llvm/llvm-project/tree/main/clang/include/clang-c

Input:
```bash
-i
"PATH/TO/CLANG/include/clang-c/Index.h"
-o
"./libclang.cs"
-u
-l
"libclang"
-s
"PATH/TO/CLANG/include"
-a
"-fno-blocks"
```

C
```c
...
CINDEX_LINKAGE CXTranslationUnit clang_parseTranslationUnit(
    CXIndex CIdx, const char *source_filename,
    const char *const *command_line_args, int num_command_line_args,
    struct CXUnsavedFile *unsaved_files, unsigned num_unsaved_files,
    unsigned options);
...
```

Output:
```cs
// <auto-generated/>
// ReSharper disable All

using System.Runtime.InteropServices;

...
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    public static extern CXTranslationUnit clang_parseTranslationUnit(CXIndex CIdx, [In] sbyte* source_filename, [In] sbyte* * command_line_args, int num_command_line_args, CXUnsavedFile* unsaved_files, uint num_unsaved_files, uint options);
...
```

## Troubleshooting

1. macOS: `fatal error: 'stdint.h' file not found` or `fatal error: 'stdbool.h' file not found`

Install CommandLineTools if you have not already:
```bash
xcode-select --install
```

Then use the following as an include directory:
```bash
/Library/Developer/CommandLineTools/usr/lib/clang/12.0.0/include
```

## Lessons learned

### Marshalling

There exist hidden costs for interoperability in C#. How to avoid them?

For C#, the Common Language Runtime (CLR) marshals data between managed and unmanaged contexts (forwards and possibly backwards). In layman's terms, marshalling is transforming the bit representation of a data structure to be correct for the target programming language. For best performance, at worse, marshalling should be minimal, and at best, marshalling should be pass-through. Pass through is the ideal situation when considering performance because both languages agree on the bit representation of data structures without any further processing. C# calls such data structures "blittable" after the bit-block transfer (bit-blit) data operation commonly found in computer graphics. However, to achieve blittable data structures in C#, the garbage collector (GC) is avoided. Why? Because class instances in C# are objects which the allocation of bits can't be controlled precisely by the developer; it's an "implementation detail."

### The garbage collector is a software industry hack

The software industry's attitude, especially business-developers and web-developers, to say that memory is an "implementation detail" and then ignore memory ultimately is very dangerous.

A function call that changes the state of the system is a side effect. Humans are imperfect at reasoning about side effects, to reason about non-linear systems. An example of a side effect is calling `fopen` in C because it leaves a file in an open state. `malloc` in C is another example of a side effect because it leaves a block of memory allocated. Notice that side effects come in pairs. To close a file, `fclose` is called. To deallocate a block of memory, `free` is called. Other languages have their versions of such function pairs. Some languages went as far as inventing language-specific features, some of which become part of our software programs, so we humans don't have to deal with such pairs of functions. In theory, this is a great idea. And thus, we invented garbage collection to take us to the promised land of never having to deal with the specific pair of functions `malloc` and `free` ever again.

In practice, using garbage collection to manage your memory automatically turns out to be a horrible idea. This becomes evident if you ever worked on an extensive enough system with the need for real-time responsiveness. In fairness, most applications don't require real-time responsiveness, and it is a lot easier to write safe programs with a garbage collector. However, this is where I think the problem starts. The problem is that developers have become ignorant of why good memory management is essential. This "Oh, the system will take care of it, don't worry." attitude is like a disease that spreads like wild-fire in the industry. The reason is simple: it lowers the bar of experience + knowledge + time required to write safe software. The consequence is that a large number of developers have learned to use a [Golden Hammer](https://en.wikipedia.org/wiki/Law_of_the_instrument#Computer_programming). They have learned to ignore how the hardware operates when solving problems, even up to the extreme point that they deny that the hardware even exists. Optimizing code for performance has become an exercise of stumbling around in the pitch-black dark until you find something of interest; it's an afterthought. Even if the developer does find something of interest, it likely opposes his/her worldview of understandable code because they have lost touch with the hardware, lost touch with reality. C# is a useful tool, but you and I have to admit that people mostly use it as Golden Hammer. Just inspect the source code that this tool generates for native bindings as proof of this fact. From my experience, a fair amount of C# developers don't spend their time with such code, don't know how to use structs properly, or even what blittable data structures are. C# developers (including myself) may need to take a hard look in the mirror, especially if we are open to critizing developers of other languages who have the same problem with Java or JavaScript.

## License

C2CS is licensed under the MIT License (`MIT`). See the [LICENSE](LICENSE) file for details.
