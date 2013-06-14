using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Helper.Http
{
    public class HttpHelper
    {
        public static string[] HttpRequestHeader = { "HTTP/1.1 Sosospider+(+ http://help.soso.com/webspider.htm)",
                                                     "HTTP/1.1 Baiduspider+(+ http://www.baidu.com/search/spider.htm)",
                                                     "HTTP/1.1 Mozilla/5.0+(compatible;+Googlebot/2.1;++ http://www.google.com/bot.html)",
                                                     "HTTP/1.1 Mozilla/4.0+(compatible;+MSIE+8.0;+Windows+NT+6.1;+WOW64;+Trident/4.0;+SLCC2;+.NET+CLR+2.0.50727;+.NET+CLR+3.5.30729;+.NET+CLR+3.0.30729;+Media+Center+PC+6.0;+MDDC)",
                                                     "HTTP/1.1 Mozilla/5.0+(Windows;+U;+Windows+NT+6.1;+en-US)+AppleWebKit/534.10+(KHTML,+like+Gecko)+Chrome/8.0.552.224+Safari/534.10 ASP.NET_SessionId=k5mdnp2yy1zidz45jagvc455",
                                                     "Mozilla/5.0 (compatible; +Googlebot/2.1;++http://www.google.com/bot.html)",
                                                     "Mozilla/4.0 (compatible; MSIE 6.01; Windows NT 5.0)" };
        private static string encode = "utf-8";

        /// <summary>
        /// 如果存在跳转url，返回重定向后的URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string GetReturnUrl(string url, int timeout)
        {
            string returnUrl = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[new Random().Next(0, HttpRequestHeader.Length)];
                hwr.Method = "GET";
                hwr.Timeout = timeout;
                hwr.AllowAutoRedirect = true;
                Encoding encoding = Encoding.GetEncoding(encode);

                HttpWebResponse hwrs = null;

                hwrs = (HttpWebResponse)hwr.GetResponse();
                returnUrl = hwrs.ResponseUri.ToString();
            }
            catch (Exception ex)
            {
              //  Log4Helper.AddError(ex);
                returnUrl = string.Empty;
                return string.Empty;
            }
            return returnUrl;
        }
        /// <summary>
        /// 获取URL的HTML数据
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="timeout">超时时间</param>
        /// <param name="rurl">对于跳转的URL，返回跳转后的URL</param>
        /// <returns></returns>
        public static string GetInnerHtml(string url, int timeout, out string rurl)
        {
            string content = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[new Random().Next(0, HttpRequestHeader.Length)];
                hwr.Method = "GET";
                hwr.Timeout = timeout;
                hwr.AllowAutoRedirect = true;
                Encoding encoding = Encoding.GetEncoding(encode);

                HttpWebResponse hwrs = null;

                hwrs = (HttpWebResponse)hwr.GetResponse();
                rurl = hwrs.ResponseUri.ToString();
                Stream s = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(s, encoding);
                content = sr.ReadToEnd();
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
             //   Log4Helper.AddError(ex);
                rurl = string.Empty;
                return string.Empty;
            }
            return content;
        }
        /// <summary>
        /// 返回网页的状态码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetInnerHtml(string url, int timeout, out int status)
        {
            string content = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[new Random().Next(0, HttpRequestHeader.Length)];
                hwr.Method = "GET";
                hwr.Timeout = timeout;
                hwr.AllowAutoRedirect = false;
                Encoding encoding = Encoding.GetEncoding(encode);

                HttpWebResponse hwrs = null;

                hwrs = (HttpWebResponse)hwr.GetResponse();
                status = (int)hwrs.StatusCode;
                Stream s = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(s, encoding);
                content = sr.ReadToEnd();
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
              //  Log4Helper.AddError(ex);
                status = -1;
                return string.Empty;
            }
            return content;
        }

        public static string GetInnerHtml(string url, int timeout, string charset)
        {
            string content = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[new Random().Next(0, HttpRequestHeader.Length)];
                hwr.Method = "GET";
                hwr.Timeout = timeout;
                hwr.AllowAutoRedirect = true;
                Encoding encoding = Encoding.GetEncoding(charset);

                HttpWebResponse hwrs = null;

                hwrs = (HttpWebResponse)hwr.GetResponse();
                ////rurl = hwrs.ResponseUri.ToString();
                Stream s = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(s, encoding);
                content = sr.ReadToEnd();
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
              //  Log4Helper.AddError(ex);
                return string.Empty;

            }
            return content;
        }

        public static string GetInnerHtml(string url, CookieContainer cookie, string charset)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;
            string content = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[random.Next(6)];
                hwr.Method = "GET";
                hwr.Timeout = 20000;
                hwr.CookieContainer = cookie;
                Encoding encoding = Encoding.GetEncoding(charset);
                HttpWebResponse hwrs = null;
                hwrs = (HttpWebResponse)hwr.GetResponse();
                Stream s = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(s, encoding);
                content = sr.ReadToEnd();
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
               // Log4Helper.AddError(ex);
            }
            return content;
        }
        /// <summary>
        /// 请求URL并下载HTML，同时返回真正的URL还有页面状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <param name="rurl"></param>
        /// <param name="pagestatus"></param>
        /// <returns></returns>
        public static string GetInnerHtml(string url, int timeout, out string rurl, out string pagestatus)
        {
            string content = string.Empty;
            try
            {
                Uri uri = new Uri(url);
                pagestatus = string.Empty;
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[new Random().Next(0, HttpRequestHeader.Length)];
                hwr.Method = "GET";
                hwr.Timeout = timeout;
                hwr.AllowAutoRedirect = true;
                Encoding encoding = Encoding.GetEncoding(encode);

                HttpWebResponse hwrs = null;

                hwrs = (HttpWebResponse)hwr.GetResponse();
                rurl = hwrs.ResponseUri.ToString();
                Stream s = hwrs.GetResponseStream();
                StreamReader sr = new StreamReader(s, encoding);
                content = sr.ReadToEnd();
                s.Close();
                sr.Close();
            }
            catch (Exception ex)
            {
             //   Log4Helper.AddError(ex);
                rurl = string.Empty;
                pagestatus = string.Empty;
                return string.Empty;
            }
            return content;
        }

        private static readonly Random random = new Random();
        private static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 获取HTML，返回真实地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="realUrl"></param>
        /// <returns></returns>
        public static string GetHTML(string url, out string realUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = HttpRequestHeader[random.Next(0, HttpRequestHeader.Length)];
                request.Method = "GET";
                request.AllowAutoRedirect = true;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                realUrl = response.ResponseUri.ToString();
                string html = null;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        html = reader.ReadToEnd();
                        reader.Close();
                    }
                    stream.Close();
                }
                return html;
            }
            catch (Exception ex)
            {
            //    Log4Helper.AddError(ex);
                realUrl = string.Empty;
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取HTML
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string GetInnerHtml(string url, int timeout)
        {
            string content = string.Empty;
            Stream s = null;
            StreamReader sr = null;
            try
            {
                Uri uri = new Uri(url);
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(uri);
                hwr.ContentType = "application/x-www-form-urlencoded";
                hwr.UserAgent = HttpRequestHeader[random.Next(0, HttpRequestHeader.Length)];
                hwr.Method = "GET";
                hwr.Timeout = timeout;
                Encoding encoding = Encoding.GetEncoding(encode);
                HttpWebResponse hwrs = null;
                hwrs = (HttpWebResponse)hwr.GetResponse();

                s = hwrs.GetResponseStream();
                sr = new StreamReader(s, encoding);
                content = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
            //    Log4Helper.AddError(ex + "，错误URL：" + url + "");
                return null;
            }
            finally
            {
                if (s != null)
                    s.Close();
                if (sr != null)
                    sr.Close();
            }
            return content;
        }
        /// <summary>
        /// 仅仅获取真实的URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetRealUrl(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = HttpRequestHeader[random.Next(0, HttpRequestHeader.Length)];
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.Timeout = 10000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                return response.ResponseUri.ToString();
            }
            catch (Exception ex)
            {
              //  Log4Helper.AddError(ex);
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取网页返回状态码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static int GetStatusCode(string url,int timeOut)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = HttpRequestHeader[new Random().Next(0, HttpRequestHeader.Length)];
                request.Timeout = timeOut;
                request.Method = "GET";
                //如果请允许跳转，请求的地址跳转到最后一个页面的地址状态
                request.AllowAutoRedirect = false;
                response = (HttpWebResponse)request.GetResponse();
                return (int)response.StatusCode;
            }
            catch (WebException webex)
            {
                response = (HttpWebResponse)webex.Response;
                if (response != null)
                    return (int)response.StatusCode;
                return -1;
            }
            catch
            {
                return -1;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }
    }
}
