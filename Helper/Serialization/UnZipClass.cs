using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Helper.Serialization
{
    public class UnZipClass
    {
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                Stream zipStream = new GZipStream(ms, CompressionMode.Decompress);
                byte[] dc_data = null;
                dc_data = ExtractBytesFromStream(zipStream, data.Length);
                return dc_data;
            }
            catch
            {
                return null;
            }
        }
        public static byte[] ExtractBytesFromStream(Stream zipStream, int dataBlock)
        {
            byte[] data = null;
            int totalBytesRead = 0;
            try
            {
                while (true)
                {
                    Array.Resize(ref data, totalBytesRead + dataBlock + 1);
                    int bytesRead = zipStream.Read(data, totalBytesRead, dataBlock);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    totalBytesRead += bytesRead;
                }
                Array.Resize(ref data, totalBytesRead);
                return data;
            }
            catch
            {
                return null;
            }
        }
    }
}
