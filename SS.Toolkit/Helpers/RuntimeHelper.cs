using System;
using System.Collections.Generic;
using System.Text;

namespace SS.Toolkit.Helpers
{
    internal static class RuntimeHelper
    {
        // Adapted from: https://stackoverflow.com/questions/721161/how-to-detect-which-net-runtime-is-being-used-ms-vs-mono#721194
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
