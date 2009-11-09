using System;
using Castle.MonoRail.Framework.Helpers;

namespace CDS.Framework.Tools.Colibri.Web.Helpers
{
    public class FormatHelper : AbstractHelper
    {
        public string ConvertNewLines(string text)
        {
            return text.Replace(Environment.NewLine, "<br />");
        }
    }
}
