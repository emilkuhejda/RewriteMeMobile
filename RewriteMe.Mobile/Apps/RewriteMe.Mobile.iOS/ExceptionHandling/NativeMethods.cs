using System;
using System.Runtime.InteropServices;

namespace RewriteMe.Mobile.iOS.ExceptionHandling
{
    internal static class NativeMethods
    {
        [DllImport("libc")]
        public static extern int sigaction(Signal sig, IntPtr act, IntPtr oact);

        public enum Signal
        {
            SIGBUS = 10,
            SIGSEGV = 11
        }
    }
}