// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
//

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Intrinsics;

namespace IntelHardwareIntrinsicTest
{
    class Program
    {
        const int Pass = 100;
        const int Fail = 0;

        static unsafe int Main(string[] args)
        {
            int testResult = Pass;

            if (Sse.IsSupported)
            {
                float* inArray = stackalloc float[4];
                float* outArray = stackalloc float[4];

                var vf = Sse.LoadAlignedVector128(inArray);
                Unsafe.Write(outArray, vf);

                for (var i = 0; i < 4; i++)
                {
                    if (BitConverter.SingleToInt32Bits(inArray[i]) != BitConverter.SingleToInt32Bits(outArray[i]))
                    {
                        Console.WriteLine("SSE LoadAlignedVector128 failed on float:");
                        for (var n = 0; n < 4; n++)
                        {
                            Console.Write(outArray[n] + ", ");
                        }
                        Console.WriteLine();

                        testResult = Fail;
                        break;
                    }
                }
            }

            return testResult;
        }
    }
}
