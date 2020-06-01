using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using X.PagedList;

namespace Jusoft.YiFang.Api.Help
{
    public static class JsonResultHelper
    {
        static readonly JsonSerializerSettings setting;
        static JsonResultHelper()
        {
            setting = new JsonSerializerSettings()
            {
                //忽略循环引用，如果设置为Error，则遇到循环引用的时候报错（建议设置为Error，这样更规范）
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                //时间格式
                DateFormatString = "yyyy/MM/dd HH:mm",
                //json中属性开头字母小写的驼峰命名
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
        }
        /// <summary>
        /// 常规
        /// </summary>
        /// <param name="errcode"></param>
        /// <param name="errmsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonResult<Result> JsonResult(int errcode, string errmsg, object data = null)
        {
            var result = new Result
            {
                Data = data,
                errcode = errcode,
                errmsg = new string[] { errmsg }
            };
            return new JsonResult<Result>(result, setting, Encoding.UTF8, new HttpRequestMessage { });
        }

        public static HttpResponseMessage JsonResult(HttpStatusCode code, int errcode, string errmsg, object data = null)
        {
            var result = new Result
            {
                Data = data,
                errcode = errcode,
                errmsg = new string[] { errmsg }
            };
            var resp = new HttpResponseMessage
            {
                StatusCode = code
            };
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.CreateResponse(code);
            HttpResponseMessage responseMessage = new HttpResponseMessage();

            return responseMessage;
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonResult<Result> JsonResult(object data = null)
        {
            var result = new Result
            {
                Data = data,
                errcode = 0,
                errmsg = new string[] { "OK" }
            };
            return new JsonResult<Result>(result, setting, Encoding.UTF8, new HttpRequestMessage { });
        }
        /// <summary>
        /// 控制器自带模型验证
        /// </summary>
        /// <param name="errcode"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public static JsonResult<Result> JsonResult(int errcode, IEnumerable<string> errmsg)
        {
            var result = new Result
            {
                errcode = errcode,
                errmsg = errmsg
            };
            return new JsonResult<Result>(result, setting, Encoding.UTF8, new HttpRequestMessage { });
        }
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static JsonResult<Result> JsonResult(IPagedList list)
        {
            var data = new
            {
                ListData = list,
                PageIndex = list.PageNumber,
                list.PageCount,
                list.TotalItemCount,
                list.HasNextPage
            };
            var result = new Result
            {
                Data = data,
                errcode = 0,
                errmsg = new string[] { "OK" }
            };
            return new JsonResult<Result>(result, setting, Encoding.UTF8, new HttpRequestMessage { });
        }
    }
    /// <summary>
    /// 消息返回类
    /// </summary>
    public class Result
    {
        public int errcode { get; set; }

        public IEnumerable<string> errmsg { get; set; }

        public object Data { get; set; }


    }



    public static class Enumerable
    {
        /// <summary>
        /// 扩展
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        //public static IEnumerable<TSource> SeachKey<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,object searchKey)
        //{
          

           
          
        //}
      
}
}