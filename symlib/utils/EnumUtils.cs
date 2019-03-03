using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace symlib.utils {
    public static class EnumUtils {
        // http://stackoverflow.com/questions/588643/generic-method-with-multiple-constraints
        public static string GetDescription<T>(this T enumerationValue) where T : IConvertible {
            DescriptionAttribute attribute = enumerationValue.GetAttribute<T, DescriptionAttribute>();
            return attribute == null ? enumerationValue.ToString() : attribute.Description;
        }

        public static TA GetAttribute<TE, TA>(this TE enumerationValue) where TE : IConvertible where TA : Attribute {
            Type type = typeof(TE);
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0) {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(TA), false);

                if (attrs != null && attrs.Length > 0)
                    return (TA)attrs[0];
            }

            return null;
        }

        public static T[] List<T>(params T[] excludes) {
            List<T> values = typeof(T).GetEnumValues().Cast<T>().ToList();
            foreach (T exclude in excludes)
                values.Remove(exclude);

            return values.ToArray();
        }

        public static IEnumerable List(Type type, bool includeNullValue = false) {
            List<object> list = new List<object>();

            if (includeNullValue)
                list.Add(null);

            list.AddRange(Enum.GetValues(type).Cast<object>());

            return list;
        }

        public static int Count(Type type) {
            return List(type).Cast<object>().Count();
        }

        public static T Parse<T>(string text, bool caseSensitive = false) {
            return (T)Parse(typeof(T), text, caseSensitive, true);
        }

        public static T? ParseNullable<T>(string text, bool caseSensitive = false) where T : struct, IConvertible {
            object value = Parse(typeof(T), text, caseSensitive, false);
            return (T?)value;
        }

        public static object Parse(Type t, string text, bool caseSensitive = false, bool throwExceptionOnFail = true) {
            foreach (object item in Enum.GetValues(t)) {
                if (caseSensitive) {
                    if (text == item.ToString())
                        return item;
                } else {
                    if (text.ToLower() == item.ToString().ToLower())
                        return item;
                }
            }

            if (throwExceptionOnFail)
                throw new Exception(string.Format("Could not parse '{0}' as type {1}. Available values are: {2}", text, t.Name, string.Join(", ", Enum.GetValues(t))));

            return null;
        }
    }
}
