using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Helper.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Helper
{
    public class Test
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns></returns>
        public byte[] GetByte()
        {
            DataTable dt = new DataTable();
            byte[] data = Compression.GetDataSetSurrogateZipBytes(dt);
            return data;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public DataTable GetData()
        {
            byte[] data = this.GetByte();
            byte[] buffer = UnZipClass.Decompress(data);
            BinaryFormatter ser = new BinaryFormatter();
            DataTableSurrogate dss = ser.Deserialize(new MemoryStream(buffer)) as DataTableSurrogate;
            DataTable dt = dss.ConvertToDataTable();
            return dt;
        }
    }
}
