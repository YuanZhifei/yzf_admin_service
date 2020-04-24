using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace YZF.Common
{
    public static class FunctionExtensions
    {
        public static void RegisterAllService(this IServiceCollection services, Type typeSource, string parten = "Service")
        {
            var assembly = Assembly.GetAssembly(typeSource);
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name.EndsWith(parten))
                {
                    services.AddTransient(type);
                }
            }
        }

        /// <summary>
        /// 判断字符串 是否为空字符串或null
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Boolean IsEmpty(this String item)
        {
            return String.IsNullOrEmpty(item);
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="en">枚举</param>
        /// <returns>返回枚举的描述</returns>
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType(); //获取类型
            MemberInfo[] memberInfos = type.GetMember(en.ToString()); //获取成员
            if (memberInfos != null && memberInfos.Length > 0)
            {
                if (memberInfos[0].GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute attr)
                    return attr.Description;
            }

            return en.ToString();
        }

        public static TEnum ToEnum<TEnum>(this string desc)
        {
            foreach (var value in Enum.GetValues(typeof(TEnum)))
            {
                var attr = value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
                if (attr != null && attr.Description == desc)
                {
                    return (TEnum)value;
                }
            }
            return default(TEnum);
        }

        public static byte[] HexStringToBytes(this string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            // 移除空格
            str = str.Replace(" ", "");
            var len = str.Length % 2 == 0 ? str.Length / 2 : str.Length + 1;
            var data = new byte[len];

            if (str.Length % 2 != 0)
            {
                str = "0" + str;
            }

            for (int i = 0; i < len; i++)
            {
                data[i] = (byte)Convert.ToInt16(str.Substring(i * 2, 2), 16);
            }

            return data;
        }
        public static int ToInt32(this string val, int defaultVal = 0)
        {
            int rawVal;
            if (!int.TryParse(val, out rawVal))
            {
                rawVal = defaultVal;
            }

            return rawVal;
        }


        public static Byte[] GetBytes(this String message)
        {
            return Encoding.UTF8.GetBytes(message);
        }

        public static String GetString(this Byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
