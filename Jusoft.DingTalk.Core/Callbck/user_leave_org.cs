using Jusoft.DingTalk.Core.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jusoft.DingTalk.Core.Callbck
{
    public class user_leave_org:CallbackFace
    {
        public void invoke(string data)
        {
            try
            {
                LogHelper.WriteLog("", "【人员离职成功】");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "【人员离职失败】");
                LogHelper.WriteLog(data, "【人员-失败数据】");
            }
        }
    }
}
