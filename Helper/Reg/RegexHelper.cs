using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace Helper.Reg
{
    public static class RegexHelper
    {
        #region 正则
        /// <summary>
        /// 取得匹配的第一个字符串
        /// </summary>
        /// <param name="str">需匹配的字符串</param>
        /// <param name="pattern">匹配正则</param>
        /// <param name="key">正则里需提取的KEY</param>
        /// <returns></returns>
        public static string GetMatchByKey(string str, string pattern, string key)
        {
            string[] ss = GetMatchsByKey(str, pattern, key);
            if (ss.Length > 0)
            {
                return ss[0];
            }
            return string.Empty;
        }
        /// <summary>
        /// 取得匹配的字符串数组
        /// </summary>
        /// <param name="str">需匹配的字符串</param>
        /// <param name="pattern">匹配正则</param>
        /// <param name="key">正则里需提取的KEY</param>
        /// <returns></returns>
        public static string[] GetMatchsByKey(string str, string pattern, string key)
        {
            Regex reg = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(str);
            ArrayList arr = new ArrayList();
            for (int i = 0; i < mc.Count; i++)
            {
                Group g = mc[i].Groups[key];
                if (g != null)
                {
                    arr.Add(g.Value);
                }
            }
            if (arr.Count < 1)
            {
                return new string[0];
            }
            return (string[])arr.ToArray(typeof(string));
        }
        #endregion
    }
}
