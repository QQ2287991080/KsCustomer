using Jusoft.DingTalk.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class user_modify_org:CallbackFace
    {
        /// <summary>
        /// 人员修改
        /// </summary>
        /// <param name="data">钉钉返回数据</param>
        public void invoke(string data)
        {
            try
            {
                LogHelper.WriteLog("", "【人员修改成功】");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "【人员修改失败】");
                LogHelper.WriteLog(data, "【人员-失败数据】");
            }
        }
    }
}
