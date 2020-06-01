using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Jusoft.YiFang.Api.Models
{
    public class UploadFile
    {
        public static string[] Upload(HttpPostedFileBase file, string Type)
        {
            //获取Guid值
            string Guid = System.Guid.NewGuid().ToString();
            string fileEextension = Path.GetExtension(file.FileName); // 获取文件后缀名
            string fileNameNot = Path.GetFileNameWithoutExtension(file.FileName);//获取文件名 不包含后缀
            string uploadDatefile = Guid + fileEextension;//最终储存文件名
            string uploadDate = DateTime.Now.ToString("yyyyMMdd");
            string virtualPath = string.Format("../files/UploadFile/{0}/{1}/{2}", Type, uploadDate, uploadDatefile); // 虚拟路径
            string fullFileName = MapPathFile(virtualPath); // 物理路径
            string path = Path.GetDirectoryName(fullFileName);  // 截取路径（去除文件名）
            Directory.CreateDirectory(path); // 创建路径
            if (!System.IO.File.Exists(fullFileName))
            {
                file.SaveAs(fullFileName);
            }
            //返回虚拟入径和物理入径
            string[] Name = { virtualPath.Replace("..", ""), fullFileName };
            return Name;
        }
        public static string MapPathFile(string FileName)
        {
            return HttpContext.Current.Server.MapPath(FileName);
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}