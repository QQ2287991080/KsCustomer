namespace Jusoft.YiFang.Api.Models
{
    public class PageDataResult
    {
        public object ListData { get; set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public int TotalItemCount { get; set; }
        public bool HasNextPage { get; set; }
    }
    public class DdTypeCount:PageDataResult
    {
        //客诉未处理状态总条数
        public int Untreated { get; set; }
        //客诉处理中状态总条数
        public int Processing { get; set; }
        //客诉已完成状态总条数
        public int Endover { get; set; }
        //全部总条数
        public int AllCount { get; set; }

    }
   
}