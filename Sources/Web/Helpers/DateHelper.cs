using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Castle.MonoRail.Framework.Helpers;
using System.Collections;
using System.Text;

namespace CDS.Framework.Tools.Colibri.Web.Helpers
{
    public class DateHelper : FormHelper
    {
        public string CalendarTextField(string target)
        {
            StringBuilder result = new StringBuilder();
            Hashtable formHelperOptions = new Hashtable();
            formHelperOptions["size"] = 20;
            formHelperOptions["length"] = 10;
            formHelperOptions["class"] = "calendar-picker";
            formHelperOptions["id"] = string.Format("calendar-{0}", target);
            object value = base.ObtainValue(target);

            if ((value != null) && (value is DateTime?))
            {
                DateTime? dtValue = (DateTime?)value;
                if (dtValue.HasValue)
                {
                    result.Append(TextFieldValue(target, dtValue.Value.ToString("yyyy-MM-dd"), formHelperOptions));
                } else {
                    result.Append(TextFieldValue(target, string.Empty, formHelperOptions));
                }
            }
            else
            {
                result.Append(TextField(target, formHelperOptions));
            }

            return result.ToString();
        }
    }
}
