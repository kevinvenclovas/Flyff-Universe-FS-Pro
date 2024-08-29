using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;

namespace FlyffUAutoFSPro._Script
{
    public static class AttributeExtension
    {
        public static string GetDescription(this Enum field)
        {
            return field.GetAttribute<DescriptionAttribute>()?.Description ?? "No Description";
        }

        public static string GetDescription(this Enum field, params object[] _parms)
        {
            try
            {
                return string.Format(field.GetAttribute<DescriptionAttribute>().Description, _parms);
            }
            catch (Exception)
            {
                return $"No description found to resolve message: {field.ToString()} | Parameter: " + JsonConvert.SerializeObject(_parms);
            }

        }

        public static T GetAttribute<T>(this Enum field)
        {
            Type type = field.GetType();

            MemberInfo[] memberInfo = type.GetMember(field.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(T), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return (T)attrs[0];
                }
            }
            return default;
        }
    }
}
