using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Helper.Img
{
    public class DownLoad
    {
        public enum LoadStatus
        {
            Success = 1,
            Error,
            NotFound
        }
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="url">远程路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static LoadStatus DownloadFile(string url, string fileName)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.01; Windows NT 5.0)";
                request.Method = "GET";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                byte[] buffer = new byte[1024];
                long fileLength = 0L;
                using (Stream responseStream = response.GetResponseStream())
                {
                    int count = responseStream.Read(buffer, 0, 1024);
                    if (count != 0)
                    {
                        FileInfo fileInfo = new FileInfo(fileName);
                        if (!Directory.Exists(fileInfo.DirectoryName))
                        {
                            Directory.CreateDirectory(fileInfo.DirectoryName);
                        }
                        using (FileStream stream = File.Create(fileName))
                        {
                            while (count != 0)
                            {
                                fileLength += count;
                                stream.Write(buffer, 0, count);
                                count = responseStream.Read(buffer, 0, 1024);
                            }
                        }
                    }
                    else
                        return LoadStatus.Error;
                }
                return LoadStatus.Success;
            }
            catch (Exception ex)
            {
              //  Log4Helper.AddError("下载失败，错误URL：" + url + "", ex);
                return LoadStatus.Error;
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileUrl">远程url</param>
        /// <param name="fileName">文件名</param>
        //public static void DownLoadFile(string fileUrl, string fileName)
        //{
        //    if (HttpHelper.GetStatusCode(fileUrl) == 200)
        //    {
        //        FileInfo fileInfo = new FileInfo(fileName);
        //        if (!Directory.Exists(fileInfo.DirectoryName))
        //            Directory.CreateDirectory(fileInfo.DirectoryName);
        //        using (WebClient myWebClient = new WebClient())
        //        {
        //            try
        //            {
        //                myWebClient.DownloadFile(fileUrl, fileName);
        //            }
        //            catch (WebException ex)
        //            {

        //            }
        //        }
        //    }
        //}
    }
}
