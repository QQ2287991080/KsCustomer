using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.PC.Models
{
    public class ResultPageData : Result
    {
        public int PageIndex { get; set; }
        public object Data { get; set; }
        public int PageCount { get; set; }
        public int TotalItemCount { get; set; }
        public bool HasNextPage { get; set; }

    }
    public class ResultData:Result
    {
        public object Data { get; set; }
    }
    public class Result
    {
        public int Code { get; set; }
        public string strResult { get; set; }
    }
}