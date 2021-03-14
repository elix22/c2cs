// Copyright (c) Lucas Girouard-Stranks (https://github.com/lithiumtoast). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the Git repository root directory (https://github.com/lithiumtoast/c2cs) for full license information.

namespace C2CS.Bindgen.ExploreCCode
{
    public readonly struct COpaqueType
    {
        public readonly string Name;
        public readonly CInfo Info;
        public readonly CType Type;

        public COpaqueType(
            string name,
            CInfo info,
            CType type)
        {
            Name = name;
            Info = info;
            Type = type;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
