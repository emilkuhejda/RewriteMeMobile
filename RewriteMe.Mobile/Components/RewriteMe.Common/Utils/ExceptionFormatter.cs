using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace RewriteMe.Common.Utils
{
    public static class ExceptionFormatter
    {
        /// <summary>
        /// Formats an exception. Goes recursively through all inner exceptions.
        /// </summary>
        /// <param name="ex">
        /// Exception to be formatted
        /// </param>
        public static string FormatException(Exception ex)
        {
            return FormatException(ex, 0);
        }

        /// <summary>
        /// Formats an Exception. Goes recursively through all inner exceptions.
        /// </summary>
        /// <param name="ex">
        /// Exception to be formatted
        /// </param>
        /// <param name="depth">
        /// Depth of recursion
        /// </param>
        private static string FormatException(Exception ex, int depth)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            var indent = string.Empty.PadLeft(4 * depth);

            // Common Exception Attributes
            sb.AppendLine(FormatSimpleException(ex, indent));

            // Format inner exceptions the same way
            if (ex is AggregateException aggregateException)
            {
                // aggregate exceptions have more than one inner exception
                var c = 1;
                foreach (
                    Exception innerException in
                        aggregateException.InnerExceptions)
                {
                    sb.AppendLine("----------------------------");
                    sb.AppendLine(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            indent + "Aggregated Inner Exceptions({0}/ {1}):",
                            c++,
                            aggregateException.InnerExceptions.Count));

                    sb.AppendLine(FormatException(innerException, ++depth));
                }
            }
            else if (ex.InnerException != null)
            {
                // just a normal inner exception
                sb.AppendLine("----------------------------");
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        indent + "Inner Exception: {0}",
                        ++depth));
                sb.AppendLine(FormatException(ex.InnerException, depth));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Format a plain vanilla exception into a string
        /// </summary>
        /// <param name="ex">
        /// The exception
        /// </param>
        /// <param name="indent">
        /// Indentation to use
        /// </param>
        /// <returns>
        /// Formatted exception
        /// </returns>
        private static string FormatSimpleException(Exception ex, string indent)
        {
            var sb = new StringBuilder();

            // Common Exception Attributes
            sb.AppendLine("Error Summary");
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, indent + "Type:        {0}", ex.GetType()));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, indent + "Message:     {0}", ex.Message));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, indent + "Source:      {0}", ex.Source));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, indent + "Time:        {0}", DateTime.Now));
            sb.AppendLine();
            sb.AppendLine("Error Details");
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, indent + "StackTrace:  {0}", ex.StackTrace));
            sb.AppendLine(string.Format(CultureInfo.InvariantCulture, indent + "HelpLink:    {0}", ex.HelpLink));

            // Data of Exception
            sb.AppendLine(indent + "Data:");
            int i = 1;
            foreach (DictionaryEntry d in ex.Data)
            {
                sb.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        indent + "  {0}: {1} {2}",
                        i++,
                        d.Key,
                        d.Value?.ToString() ?? string.Empty));
            }

            return sb.ToString();
        }
    }
}
