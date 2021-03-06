using System;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.Framework;
using System.Collections.Specialized;
using System.Collections;
using NHibernate.Expression;

namespace CDS.Framework.Tools.Colibri.Web.Helpers
{
    public class OrderHelper : AbstractHelper
    {
        const string QUERY_STRING_PROPERTY_NAME = "order.columnName";
        const string QUERY_STRING_ASC = "order.ascending";

        public string OrderLink(string propertyName)
        {
            return OrderLink(propertyName, propertyName);
        }

        public string OrderLink(string label, string propertyName)
        {
            NameValueCollection inputQueryString = base.Controller.Request.QueryString;
            string filePath = base.CurrentContext.Request.FilePath;
            Hashtable targetQueryString = BuildTargetQueryString(inputQueryString);
            string glyph = "";

            if (targetQueryString.ContainsKey(QUERY_STRING_PROPERTY_NAME))
            {
                if (((string[])targetQueryString[QUERY_STRING_PROPERTY_NAME])[0].Equals(propertyName))
                {
                    if (Convert.ToBoolean(((string[])targetQueryString[QUERY_STRING_ASC])[0]))
                    {
                        glyph = "▲&nbsp;";
                        targetQueryString[QUERY_STRING_ASC] = false;
                    }
                    else
                    {
                        glyph = "▼&nbsp;";
                        targetQueryString[QUERY_STRING_ASC] = true;
                    }
                }
                else
                {
                    targetQueryString[QUERY_STRING_PROPERTY_NAME] = propertyName;
                    targetQueryString[QUERY_STRING_ASC] = true;
                }
            }
            else
            {
                targetQueryString.Add(QUERY_STRING_PROPERTY_NAME, propertyName);
                targetQueryString.Add(QUERY_STRING_ASC, true);
            }

            return string.Format("<a href=\"{0}?{1}\">{2}{3}<a>", filePath, BuildQueryString(targetQueryString), glyph, label);
        }

        public static Order CreateOrder(string defaultPropertyName, bool defaultAscending)
        {
            string resultPropertyName = defaultPropertyName;
            bool resultAscending = defaultAscending;
            Hashtable targetQueryString = BuildTargetQueryString(MonoRailHttpHandler.CurrentContext.CurrentController.Request.QueryString);

            if (targetQueryString.ContainsKey(QUERY_STRING_PROPERTY_NAME))
            {
                resultPropertyName = ((string[])targetQueryString[QUERY_STRING_PROPERTY_NAME])[0];
                resultAscending = Convert.ToBoolean(((string[])targetQueryString[QUERY_STRING_ASC])[0]);
            }

            return new Order(resultPropertyName, resultAscending);
        }

        private static Hashtable BuildTargetQueryString(NameValueCollection inputQueryString)
        {
            Hashtable targetQueryString = new Hashtable(inputQueryString.Count);
            foreach (string str in inputQueryString.Keys)
            {
                if (str != null)
                {
                    targetQueryString[str] = inputQueryString.GetValues(str);
                }
            }
            return targetQueryString;
        }
    }
}
