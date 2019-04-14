using System;
using Android.Runtime;
using RewriteMe.Business.ExceptionHandling;
using RewriteMe.Domain.Interfaces.ExceptionHandling;

namespace RewriteMe.Mobile.Droid.ExceptionHandling
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
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentUnhandledExceptionRaiser;
        }

        protected override void Detach()
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser -= AndroidEnvironmentUnhandledExceptionRaiser;
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
        }

        private void AndroidEnvironmentUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            if (e.Exception != null)
            {
                e.Handled = HandleException(e.Exception);
            }
        }
    }
}