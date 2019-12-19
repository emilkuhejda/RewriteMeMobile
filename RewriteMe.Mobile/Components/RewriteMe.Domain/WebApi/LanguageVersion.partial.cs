using RewriteMe.Common.Utils;
using RewriteMe.Domain.Enums;

namespace RewriteMe.Domain.WebApi
{
    public partial class LanguageVersion
    {
        public Language Language => EnumHelper.Parse(LanguageString, Language.Undefined);
    }
}
