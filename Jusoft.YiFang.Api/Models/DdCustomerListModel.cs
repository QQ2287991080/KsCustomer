using Jusoft.YiFang.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Jusoft.YiFang.Api.Models
{
    public class DdCustomerListModel
    {
        //类型
        public List<int?> Type { get; set; }
        //当前页
        public int PageIndex { get; set; }
        //每页显示的条数
        public int PageSize { get; set; }
        //筛选开始时间
        public DateTime? StartTime { get; set; }
        //筛选结束时间
        public DateTime? EndTime { get; set; }
        //筛选客诉类型
        public int? CustomerType { get; set; }
        //筛选所属门店
        public int? OwnedStores { get; set; }
        //筛选异常类型
        public int? AbnormalType { get; set; }
        //关键字搜索
        public string Seachkey { get; set; }
    }



    public class CustomerCondition : Page
    {
        private DateTime? ExTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 类型
        /// </summary>
        public decimal? State { get; set; }
        /// <summary>
        /// 筛选开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 筛选结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get
            {
                return ExTime.Value.AddDays(1);
            }
            set
            {
                if (value != null)
                {
                    ExTime = value;
                }
            }
        }
        /// <summary>
        /// 筛选客诉类型
        /// </summary>
        public int? CustomerType { get; set; }
        /// <summary>
        /// 筛选所属门店
        /// </summary>
        public int? OwnedStores { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string SearchKey  { get; set; }
        /// <summary>
        /// 类型id
        /// </summary>
        public bool IsHead { get; set; }

        public string  StoreName { get; set; }

        public short Num { get; set; }

        public int? Number { get; set; }
        ////筛选异常类型
        //public int? AbnormalType { get; set; }
        //关键字搜索
        //public string Seachkey { get; set; }

    }


    public class CustomerCondition2 : Page
    {
        /// <summary>
        /// 类型
        /// </summary>
        public decimal? State { get; set; }
        /// <summary>
        /// 筛选开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 筛选结束时间
        /// </summary>
        /// <summary>
        /// 筛选客诉类型
        /// </summary>
        public int? CustomerType { get; set; }
        /// <summary>
        /// 筛选所属门店
        /// </summary>
        public int? OwnedStores { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string SearchKey { get; set; }
        /// <summary>
        /// 类型id
        /// </summary>
        public bool IsHead { get; set; }

        public string StoreName { get; set; }

        public short Num { get; set; }

        public int? Number { get; set; }
        ////筛选异常类型
        //public int? AbnormalType { get; set; }
        //关键字搜索
        //public string Seachkey { get; set; }

    }
    public abstract class Page {

        /// <summary>
        /// 页数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
    }

    public class UpdateCustmoer
    {
        /// <summary>
        /// 客诉ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 是否同意1  同意 2驳回
        /// </summary>
        public int IsAgree { get; set; }
        /// <summary>
        /// 备注
        /// </summary>

        public string  Remark { get; set; }

        public List<Audio>  Audios { get; set; }
    }
    public static class MyExtension
    {
        public static bool IsExistencePerson(this IQueryable<OR_Person> source,string loginName)
        {
            return source.Any(p => p.LoginName == loginName);
        }

        public static bool IsExistencePerson(this Controller controller, YiFang_CustomerComplaintEntities db)
        {
            string loginName = controller.User.Identity.Name;
            return db.OR_Person.Any(p => p.LoginName == loginName);
        }
    }
}