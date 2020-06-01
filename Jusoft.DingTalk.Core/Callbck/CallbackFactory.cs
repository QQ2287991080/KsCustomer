using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class CallbackFactory
    {
        /// <summary>
        /// /钉钉回调接收方法
        /// </summary>
        /// <param name="eventType">钉钉回调类型</param>
        /// <param name="data">钉钉回调内容</param>
        /// <param name="token">token</param>
        public  void DingTalkEventShunt(string eventType,string data,string token)
        {


            var assembly = Assembly.Load("Jusoft.DingTalk.Core");

            CallbackFace callback= assembly.CreateInstance("Jusoft.DingTalk.Core.Callbck."+ eventType) as CallbackFace;
            callback.invoke(data);
            //CallBack call;

            //switch (eventType)
            //{
            //    case "org_dept_create": // 通讯录企业部门创建
            //        call=new org_dept_create();
            //        call.Work(data);
            //        break;
            //    case "org_dept_modify": // 通讯录企业部门修改
            //        new org_dept_modify(data);
            //        break;
            //    case "org_dept_remove": // 通讯录企业部门删除
            //        new org_dept_remove(data);
            //        break;
            //    case "user_add_org": // 通讯录用户增加
            //        new user_add_org(data);
            //        break;
            //    case "user_modify_org": // 通讯录用户修改
            //        new user_modify_org(data);
            //        break;
            //    case "user_leave_org": // 通讯录用户离职
            //        new user_leave_org(data);
            //        break;
            //    case "bpms_instance_change": // 审批实例开始/结束//终止
            //        new bpms_instance_change(data);
            //        break;
            //    case "bpms_task_change"://审批任务开始/结束/转交
            //        new bpms_task_change(data);
            //        break;
            //}
           
        }
    }
}
