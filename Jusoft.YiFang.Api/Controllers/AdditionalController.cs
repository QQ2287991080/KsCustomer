using Jusoft.DingTalk.Core.Logs;
using Jusoft.YiFang.Api.Help;
using Jusoft.YiFang.Dto.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace Jusoft.YiFang.Api.Controllers
{
    public class AdditionalController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetMP3(string url,decimal? Duration,string Remark)
        {
            try
            {
                LogHelper.WriteLog(url);
                //Arm文件的虚拟路径
                string arm = "/files/UploadFile/KS02/" + DateTime.Now.ToString("yyyyMMdd") + "/ARM/" + Guid.NewGuid().ToString() + ".arm";
                //Arm文件的物理路径
                var armPath = HttpContext.Current.Server.MapPath("~" + arm);
                string arm2 = "/files/UploadFile/KS02/" + DateTime.Now.ToString("yyyyMMdd") + "/ARM/";
                var armPath2 = HttpContext.Current.Server.MapPath("~" + arm2);
                if (!Directory.Exists(armPath2))
                {
                    Directory.CreateDirectory(armPath2);
                }
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 3000;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                
                //arm创建本地文件写入流
                Stream stream = new FileStream(armPath, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();

                //创建mp3
                //MP3文件的虚拟路径
                string mp3 = "/files/UploadFile/KS02/" + DateTime.Now.ToString("yyyyMMdd") + "/MP3/" + Guid.NewGuid().ToString() + ".mp3";
                //mp3的物理路径
                var mp3Path = HttpContext.Current.Server.MapPath("~" + mp3);
                string m2 = "/files/UploadFile/KS02/" + DateTime.Now.ToString("yyyyMMdd") + "/MP3/";
                string mp3Path2= HttpContext.Current.Server.MapPath("~" + m2);
                if (!Directory.Exists(mp3Path2))
                {
                    Directory.CreateDirectory(mp3Path2);
                }
                var fullpath = ConvertToMp3(armPath, mp3Path);
                /*string url = HttpContext.Request.Url.ToString().Replace(HttpContext.Request.Url.PathAndQuery, "");*/ //服务器协议+域名+端口

                var xx = this.Request.RequestUri.ToString().Replace(this.Request.RequestUri.PathAndQuery, "") + mp3;
                LogHelper.WriteLog(xx);
                var Mp3ServerPath = HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.Url.PathAndQuery, "") + mp3;
                string x = Remark == null ? "" : Remark;

                string a1 = "http://121.199.49.237:12700" + mp3;
                string a2 = "http://47.110.250.181:12777/" + mp3;
                string a3= "http://47.103.68.172:6076/" + mp3;
                //var data = new
                //{
                //    Url= a2,
                //    Duration,
                //    Remark=x
                //};
                AudioDto dto = new AudioDto()
                {
                    Duration = Duration,
                    MediaId = "",
                    OriginUrl = url,
                    Url = a3,
                    Remark = x
                };
                //LogHelper.WriteLog("服务器" + HttpContext.Current.Request.Url.ToString().Replace(HttpContext.Current.Request.Url.PathAndQuery, ""));
                //LogHelper.WriteLog("文件"+Mp3ServerPath);
                return JsonResultHelper.JsonResult(dto);

            }
            catch (Exception ex)
            {
                return JsonResultHelper.JsonResult(1000, ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult ResultMp3()
        {
            var before = HttpContext.Current.Server.MapPath("~/files/UploadFile/KS02/aaaa.arm");
            var later = HttpContext.Current.Server.MapPath("~/files/UploadFile/KS02/3a.mp3");
            var mp3 = ConvertToMp3(before, later);
            return JsonResultHelper.JsonResult(0, "");
        }
        /// <summary>
        /// 执行Cmd命令
        /// </summary>
        private string RunCmd(string c)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.RedirectStandardOutput = false;
                info.UseShellExecute = false;
                Process p = Process.Start(info);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                p.StandardInput.WriteLine(c);
                p.StandardInput.AutoFlush = true;
                Thread.Sleep(1000);
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
                string outStr = p.StandardOutput.ReadToEnd();
                p.Close();

                return outStr;
            }
            catch (Exception ex)
            {
                return "error" + ex.Message;
            }
        }

        public string ConvertToMp3(string pathBefore, string pathLater)
        {
            string c = HttpContext.Current.Server.MapPath("/ffmpeg/") + @"ffmpeg.exe -i " + pathBefore + " " + pathLater;
            string str = RunCmd(c);
            return str;
        }
    }
}
