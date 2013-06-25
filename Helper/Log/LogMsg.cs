using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Helper.Log
{
    public class LogMsg
    {
        public DataGridView LogView { get; set; }
        public LogMsg(DataGridView LogView)
        {
            this.LogView = LogView;
        }
        public delegate void LogEvent(int id, string message, DataGridView dgv);
        public event LogEvent WriteLog;
        /// <summary>
        /// 写入日志消息
        /// </summary>
        /// <param name="message"></param>
        public void writeLog(int id, string message)
        {
            if (WriteLog != null)
            {
                WriteLog(id, message, LogView);
            }
        }
    }
}
