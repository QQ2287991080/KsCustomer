using Jusoft.DingTalk.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class org_dept_modify:CallbackFace
    {
        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="data">钉钉返回数据</param>
        public void invoke(string data)
        {
            try
            {
                LogHelper.WriteLog("", "【部门修改成功】");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "【部门修改失败】");
                LogHelper.WriteLog(data, "【部门-失败数据】");
            }
        }
    }
}
