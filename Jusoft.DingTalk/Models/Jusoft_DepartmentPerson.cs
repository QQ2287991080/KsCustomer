using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Models
{
    public class Jusoft_DepartmentPerson
    {
        public string errcode { get; set; }
        public string errmsg { get; set; }
        public List<UserList> userlist { get; set; }
    }
    public class UserList
    {
        public string unionid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isBoss { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<int> department { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long order { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isLeader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string active { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isAdmin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string isHide { get; set; }
        /// <summary>
        /// 黄祥
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string stateCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string position { get; set; }
    }
}
