using System;
using System.Linq;

namespace RewriteMe.Business.Extensions
{
    public static class TypeExtensions
    {
        public static string GetFriendlyName(this Type type)
        {
            var friendlyName = type.Name;
            if (type.GenericTypeArguments.Any())
            {
                var iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }

                friendlyName += "<";
                var typeParameters = type.GenericTypeArguments;

                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += i == 0 ? typeParamName : "," + typeParamName;
                }

                friendlyName += ">";
            }

            return friendlyName;
        }
    }
}
