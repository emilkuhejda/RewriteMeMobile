using NLog.Layouts;

namespace RewriteMe.Logging.Layouts
{
    public static class NLogLayouts
    {
        public static Layout GetDefaultLayout()
        {
            /***
             *  When using ${exception:format=toStirng} a Exception os thrown:
             *  NLogConfigurationException - Error when setting property 'Format' on Layout Renderer: ${exception}
             *  
             *  Nevertheless, according to the manual (https://github.com/nlog/nlog/wiki/Exception-Layout-Renderer) it
             *  should be possible to set the format property as stated.
             */

            var layout = "${longdate}|${threadid}|${level}|${logger}|${message}|${exception}[EOL]";
            return layout;
        }
    }
}
