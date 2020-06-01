using System;
using System.IO;
using System.Net;
using System.Text;

namespace Jusoft.Jietaigete.Api.Models
{
    public class HttpRequestHelper
    {
        #region HttpPost请求 +1重载
        public static string HttpPost(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = reader.ReadToEnd();
            return retString;
        }

        public static string HttpPost(string url, string postDataStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = bytes.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(bytes, 0, bytes.Length);
            writer.Flush();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string retString = reader.ReadToEnd();
            return retString;
        }
        public static string HpptFormPost(string url, Stream fileStream, string fileName)
        {
            // 文件流开始结束标识
            string boundary = $"----{DateTime.Now.Ticks.ToString("x")}";
            string beginBoundary = $"--{boundary}{Environment.NewLine}";
            string endBoundary = $"{Environment.NewLine}--{boundary}--{Environment.NewLine}";

            byte[] beginBoundaryByts = Encoding.UTF8.GetBytes(beginBoundary);
            byte[] endBoundaryByts = Encoding.UTF8.GetBytes(endBoundary);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                //写入开始标记
                memoryStream.Write(beginBoundaryByts, 0, beginBoundaryByts.Length);
                //写入文件标记
                string fileHeaderTemplate = $"Content-Disposition: form-data; name=\"fileContent\"; filename=\"{fileName}\"{Environment.NewLine}Content-Type: application/octet-stream{Environment.NewLine + Environment.NewLine}";
                byte[] fileHeaderTemplateByts = Encoding.ASCII.GetBytes(fileHeaderTemplate);
                memoryStream.Write(fileHeaderTemplateByts, 0, fileHeaderTemplateByts.Length);
                //写入文件二进制信息
                byte[] buffer = new byte[1024 * 1024];
                int size = fileStream.Read(buffer, 0, buffer.Length);
                while (size > 0)
                {
                    memoryStream.Write(buffer, 0, size);
                    size = fileStream.Read(buffer, 0, buffer.Length);
                }
                //写入结束标记
                memoryStream.Write(endBoundaryByts, 0, endBoundaryByts.Length);

                byte[] postBytes = memoryStream.ToArray();
                // 构造请求数据对象
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "multipart/form-data;boundary=" + boundary;
                request.ContentLength = postBytes.Length;
                // 请求数据
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                }


                HttpWebResponse response = (HttpWebResponse)request.GetResponse();//发送

                Stream myResponseStream = response.GetResponseStream();//获取返回值
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);

                string retString = myStreamReader.ReadToEnd();

                return retString;
            }
            
        }
        #endregion

        #region HttpGet请求
        public static string HttpGet(string url)
        {
            string strBuff = "";
            Uri httpURL = new Uri(url);
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            Stream respStream = httpResp.GetResponseStream();
            StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
            strBuff = respStreamReader.ReadToEnd();
            return strBuff;
        }
        #endregion
    }
}