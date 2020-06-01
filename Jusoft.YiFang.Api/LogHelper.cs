using System;
using System.IO;

namespace Demo.Models
{
    public class LogHelper
    {
        #region 写入日志
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="sLog">内容</param>
        /// <param name="sOption">标题</param>
        /// <param name="sSrc">路径</param>
        public static void WriteLog(string sLog, string sOption = "OP", string sSrc = "/LogData/Logs/")
        {
            var now = DateTime.Now;
            StreamWriter sr = null;

            var filePath = AppDomain.CurrentDomain.BaseDirectory + sSrc; // 文件路径
            var file = filePath + "Log_" + DateTime.Now.ToString("yyyy_MM_dd") + ".log"; // 文件

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath); // 判断是否存在目录,不存在则创建

            if (!File.Exists(file))
            {
                sr = File.CreateText(file);//创建日志文件
            }
            else
            {
                sr = File.AppendText(file);//追加日志文件
            }
            sr.WriteLine($"{DateTime.Now}:【{sOption}】:{sLog}");//日志格式
            if (sr != null) sr.Close();     //关闭文件流
        }
        #endregion
    }
}
