using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data;
using System.IO.Compression;

namespace Helper.Serialization
{
    public class Compression
    {
        /// <summary>
        /// 取得序列化后的字节
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static byte[] GetDataSetSurrogateZipBytes(DataTable dt)
        {
            DataTableSurrogate dss = new DataTableSurrogate(dt);
            BinaryFormatter ser = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ser.Serialize(ms, dss);
            byte[] buffer = ms.ToArray();
            byte[] zipBuffer = Compress(buffer);
            return zipBuffer;
        }
        /// <summary>
        /// 取得序列化后的字节
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static byte[] GetDataSetSurrogateZipBytes(DataSet ds)
        {
            DataSetSurrogate dss = new DataSetSurrogate(ds);
            BinaryFormatter ser = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ser.Serialize(ms, dss);
            byte[] buffer = ms.ToArray();
            byte[] zipBuffer = Compress(buffer);
            return zipBuffer;
        }
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                Stream zipStream = null;
                zipStream = new GZipStream(ms, CompressionMode.Compress, true);
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                ms.Position = 0;
                byte[] compressed_data = new byte[ms.Length];
                ms.Read(compressed_data, 0, int.Parse(ms.Length.ToString()));
                return compressed_data;
            }
            catch
            {
                return null;
            }
        }
    }
}
