using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Util
{
    public static class Extension
    {
        public static float ToFloat(this int num) 
        {
            return Convert.ToSingle(num);
        }
    }
}