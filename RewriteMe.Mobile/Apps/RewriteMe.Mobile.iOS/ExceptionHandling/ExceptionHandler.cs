using System;
using System.Runtime.InteropServices;
using RewriteMe.Business.ExceptionHandling;
using RewriteMe.Domain.Interfaces.ExceptionHandling;

namespace RewriteMe.Mobile.iOS.ExceptionHandling
{
    public class ExceptionHandler : ExceptionHandlerBase
    {
        public ExceptionHandler(IExceptionHandlingStrategy exceptionHandlingStrategy)
            : base(exceptionHandlingStrategy)
        {
        }

        protected override void Attach()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            AttachToNativeExceptions();
        }

        private void AttachToNativeExceptions()
        {
            IntPtr sigbus = Marshal.AllocHGlobal(512);
            IntPtr sigsegv = Marshal.AllocHGlobal(512);

            // Store Mono SIGSEGV and SIGBUS handlers
            int result;
            result = NativeMethods.sigaction(NativeMethods.Signal.SIGBUS, IntPtr.Zero, sigbus);
            result = NativeMethods.sigaction(NativeMethods.Signal.SIGSEGV, IntPtr.Zero, sigsegv);

            // Restore Mono SIGSEGV and SIGBUS handlers
            result = NativeMethods.sigaction(NativeMethods.Signal.SIGBUS, sigbus, IntPtr.Zero);
            result = NativeMethods.sigaction(NativeMethods.Signal.SIGSEGV, sigsegv, IntPtr.Zero);

            Marshal.FreeHGlobal(sigbus);
            Marshal.FreeHGlobal(sigsegv);
        }

        protected override void Detach()
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledException;
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
        }
    }
}