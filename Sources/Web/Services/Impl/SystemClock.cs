using System;

namespace CDS.Framework.Tools.Colibri.Web.Services.Impl
{
    public class SystemClock : IClock
    {
        #region IClock Members

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime Today
        {
            get { return DateTime.Today; }
        }

        #endregion
    }
}
