using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FlyffUAutoFSPro._Script.CustomAttributes
{
    public static class CustomAttributeUtils
    {
        public static T GetAttribute<T>(this Enum field)
        {
            Type type = field.GetType();

            MemberInfo[] memberInfo = type.GetMember(field.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(T), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((T)attrs[0]);
                }
            }
            return default(T);
        }
    }
}
