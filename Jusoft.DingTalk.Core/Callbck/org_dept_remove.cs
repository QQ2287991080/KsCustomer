using Jusoft.DingTalk.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class org_dept_remove:CallbackFace
    {
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="data">钉钉返回数据</param>
        public void invoke(string data)
        {
            try
            {
                LogHelper.WriteLog("", "【部门删除成功】");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "【部门删除失败】");
                LogHelper.WriteLog(data, "【部门-失败数据】");
            }
        }
    }
}
