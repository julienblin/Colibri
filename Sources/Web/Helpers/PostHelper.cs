using System;
using Castle.MonoRail.Framework.Helpers;
using System.Text;
using System.Collections;

namespace CDS.Framework.Tools.Colibri.Web.Helpers
{
    public class PostHelper : AbstractHelper
    {
        public string LinkConfirm(string label, string confirm, string action, IDictionary parameters)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("<a onclick=\"if (confirm('{0}'))", confirm.Replace("'", "\\'"));
            result.Append("{ var f = document.createElement('form');");
            result.Append("f.style.display = 'none';");
            result.Append("this.parentNode.appendChild(f);");
            result.Append("f.method = 'POST';");
            result.Append("f.action = this.href;");
            foreach (string key in parameters.Keys)
            {
                result.AppendFormat("var {0} = document.createElement('input');", key);
                result.AppendFormat("{0}.setAttribute('type', 'hidden');", key);
                result.AppendFormat("{0}.setAttribute('name', 'id');", key);
                result.AppendFormat("{0}.setAttribute('value', '{1}');", key, parameters[key]);
                result.AppendFormat("f.appendChild({0});", key);
            }
            result.Append("f.submit(); }");
            result.AppendFormat(";return false;\" href=\"{0}\">{1}</a>", action, label);
            return result.ToString();
        }
    }
}
