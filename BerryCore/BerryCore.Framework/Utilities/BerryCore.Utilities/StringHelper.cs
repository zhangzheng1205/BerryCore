﻿#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：BerryCore.Utilities
* 项目描述 ：
* 类 名 称 ：StringHelper
* 类 描 述 ：
* 所在的域 ：MRZHAOYI
* 命名空间 ：BerryCore.Utilities
* 机器名称 ：MRZHAOYI 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：赵轶
* 创建时间 ：2019/5/3 22:00:46
* 更新时间 ：2019/5/3 22:00:46
* 版 本 号 ：V1.0.0.0
***********************************************************************
* Copyright © 大師兄丶 2019. All rights reserved.                     *
***********************************************************************
//----------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BerryCore.Utilities
{
    /// <summary>
    /// 功能描述    ：字符串操作帮助类  
    /// 创 建 者    ：赵轶
    /// 创建日期    ：2019/5/3 22:00:46 
    /// 最后修改者  ：赵轶
    /// 最后修改日期：2019/5/3 22:00:46 
    /// </summary>
    public static class StringHelper
    {
        #region 防止SQL注入，过滤字符串

        ///<summary>
        /// 过滤字符串中注入SQL脚本的方法
        ///</summary>
        ///<param name="source">传入的字符串</param>
        ///<returns>过滤后的字符串</returns>
        public static string SqlFilters(string source)
        {
            //半角括号替换为全角括号
            source = source.Replace("'", "'''").Replace(";", "；").Replace("(", "（").Replace(")", "）");
            //去除执行SQL语句的命令关键字
            source = Regex.Replace(source, "select", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "insert", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "update", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "delete", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "drop", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "truncate", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "declare", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "xp_cmdshell", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "/add", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "net user", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字
            source = Regex.Replace(source, "exec", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "execute", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, "xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, "0x", "0 x", RegexOptions.IgnoreCase);

            return source;
        }

        #endregion 防止SQL注入，过滤字符串

        #region Splice(拼接集合元素)

        /// <summary>
        /// 拼接集合元素
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="quotes">引号，默认不带引号，范例：单引号 "'"</param>
        /// <param name="separator">分隔符，默认使用逗号分隔</param>
        public static string Splice<T>(IEnumerable<T> list, string quotes = "", string separator = ",")
        {
            if (list == null)
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            foreach (var each in list)
            {
                result.AppendFormat("{0}{1}{0}{2}", quotes, each, separator);
            }

            return result.ToString().TrimEnd(separator.ToCharArray());
        }

        #endregion Splice(拼接集合元素)

        #region FirstUpper(将值的首字母大写)

        /// <summary>
        /// 将值的首字母大写
        /// </summary>
        /// <param name="value">值</param>
        public static string FirstUpper(string value)
        {
            string firstChar = value.Substring(0, 1).ToUpper();
            return firstChar + value.Substring(1, value.Length - 1);
        }

        #endregion FirstUpper(将值的首字母大写)

        #region 将字符串大写转小写，小写转大写，数字保留，其他除外
        /// <summary>
        /// 将字符串大写转小写，小写转大写，数字保留，其他除外
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CapitalizeAndLowercase(string value)
        {
            //0~9数字对应十进制48－57
            //a~z字母对应的十进制97－122，十六进制61－7A
            //A~Z字母对应的十进制65－90，十六进制41－5A

            string newStr = string.Empty;//用于存放新字符串

            //字符串-->byte[]
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytes = ascii.GetBytes(value);

            //循环byte[]
            foreach (byte item in bytes)
            {
                //临时存放
                byte[] temp = new byte[] { item };

                if (item >= 97 && item <= 122)
                {
                    //小写字母转大写
                    newStr += ascii.GetString(temp).ToUpper();
                }
                else if (item >= 65 && item <= 90)
                {
                    //大写字母转小写
                    newStr += ascii.GetString(temp).ToLower();
                }
                else if (item >= 48 && item <= 57)
                {
                    //数字不变
                    newStr += ascii.GetString(temp);
                }
            }
            return newStr;
        }
        #endregion

        #region ToCamel(将字符串转成驼峰形式)

        /// <summary>
        /// 将字符串转成驼峰形式
        /// </summary>
        /// <param name="value">原始字符串</param>
        public static string ToCamel(string value)
        {
            return FirstUpper(value.ToLower());
        }

        #endregion ToCamel(将字符串转成驼峰形式)

        #region ContainsChinese(是否包含中文)

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="text">文本</param>
        public static bool ContainsChinese(string text)
        {
            const string pattern = "[\u4e00-\u9fa5]+";
            return Regex.IsMatch(text, pattern);
        }

        #endregion ContainsChinese(是否包含中文)

        #region ContainsNumber(是否包含数字)

        /// <summary>
        /// 是否包含数字
        /// </summary>
        /// <param name="text">文本</param>
        public static bool ContainsNumber(string text)
        {
            const string pattern = "[0-9]+";
            return Regex.IsMatch(text, pattern);
        }

        #endregion ContainsNumber(是否包含数字)

        #region Distinct(去除重复)

        /// <summary>
        /// 去除重复
        /// </summary>
        /// <param name="value">值，范例1："5555",返回"5",范例2："4545",返回"45"</param>
        public static string Distinct(string value)
        {
            var array = value.ToCharArray();
            return new string(array.Distinct().ToArray());
        }

        #endregion Distinct(去除重复)

        #region Truncate(截断字符串)

        /// <summary>
        /// 截断字符串
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="length">返回长度</param>
        /// <param name="endCharCount">添加结束符号的个数，默认0，不添加</param>
        /// <param name="endChar">结束符号，默认为省略号</param>
        public static string Truncate(string text, int length, int endCharCount = 0, string endChar = ".")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            if (text.Length < length)
            {
                return text;
            }

            return text.Substring(0, length) + GetEndString(endCharCount, endChar);
        }

        /// <summary>
        /// 获取结束字符串
        /// </summary>
        private static string GetEndString(int endCharCount, string endChar)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < endCharCount; i++)
            {
                result.Append(endChar);
            }

            return result.ToString();
        }

        #endregion Truncate(截断字符串)

        #region 删除最后一个字符之后的字符

        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(",", StringComparison.Ordinal));
        }

        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar, StringComparison.Ordinal));
        }

        /// <summary>
        /// 删除最后结尾的长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string DelLastLength(string str, int Length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            str = str.Substring(0, str.Length - Length);
            return str;
        }

        #endregion 删除最后一个字符之后的字符

        #region 快速验证一个字符串是否符合指定的正则表达式。

        /// <summary>
        /// 快速验证一个字符串是否符合指定的正则表达式。
        /// </summary>
        /// <param name="express">正则表达式的内容。</param>
        /// <param name="value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool QuickValidate(string express, string value)
        {
            if (value == null)
            {
                return false;
            }

            Regex myRegex = new Regex(express);
            if (value.Length == 0)
            {
                return false;
            }
            return myRegex.IsMatch(value);
        }

        /// <summary>
        /// 提取匹配到的内容
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static List<string> GetValueByRegex(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new List<string>();
            }

            List<string> res = new List<string>();
            Regex regex = new Regex(pattern);
            Match match = regex.Match(text);
            if (match.Success)
            {
                int total = match.Groups.Count;
                if (total > 0)
                {
                    for (int i = 0; i < total; i++)
                    {
                        res.Add(match.Groups[i].Value);
                    }
                }
            }
            return res;
        }

        #endregion 快速验证一个字符串是否符合指定的正则表达式。

        #region 去除Html标签

        /// <summary>
        /// 去除Html标签
        /// </summary>
        /// <param name="html"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ReplaceHtmlTag(string html, int length = 0)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
            {
                return strText.Substring(0, length);
            }

            return strText;
        }

        #endregion 去除Html标签

        #region HtmlEncode(对html字符串进行编码)

        /// <summary>
        /// 对html字符串进行编码
        /// </summary>
        /// <param name="html">html字符串</param>
        public static string HtmlEncode(string html)
        {
            return HttpUtility.HtmlEncode(html ?? "");
        }
        /// <summary>
        /// 对html字符串进行解码
        /// </summary>
        /// <param name="html">html字符串</param>
        public static string HtmlDecode(string html)
        {
            return HttpUtility.HtmlDecode(html ?? "");
        }

        #endregion

        #region UrlEncode(对Url进行编码)

        /// <summary>
        /// 对Url进行编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, bool isUpper = false)
        {
            return UrlEncode(url, Encoding.UTF8, isUpper);
        }

        /// <summary>
        /// 对Url进行编码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码</param>
        /// <param name="isUpper">编码字符是否转成大写,范例,"http://"转成"http%3A%2F%2F"</param>
        public static string UrlEncode(string url, Encoding encoding, bool isUpper = false)
        {
            var result = HttpUtility.UrlEncode(url, encoding);
            if (!isUpper)
            {
                return result;
            }

            return GetUpperEncode(result);
        }

        /// <summary>
        /// 获取大写编码字符串
        /// </summary>
        private static string GetUpperEncode(string encode)
        {
            var result = new StringBuilder();
            int index = int.MinValue;
            for (int i = 0; i < encode.Length; i++)
            {
                string character = encode[i].ToString();
                if (character == "%")
                {
                    index = i;
                }

                if (i - index == 1 || i - index == 2)
                {
                    character = character.ToUpper();
                }

                result.Append(character);
            }
            return result.ToString();
        }

        #endregion

        #region UrlDecode(对Url进行解码)

        /// <summary>
        /// 对Url进行解码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码
        /// </summary>
        /// <param name="url">url</param>
        public static string UrlDecode(string url)
        {
            return HttpUtility.UrlDecode(url);
        }

        /// <summary>
        /// 对Url进行解码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="encoding">字符编码,对于javascript的encodeURIComponent函数编码参数,应使用utf-8字符编码来解码</param>
        public static string UrlDecode(string url, Encoding encoding)
        {
            return HttpUtility.UrlDecode(url, encoding);
        }

        #endregion
    }
}
