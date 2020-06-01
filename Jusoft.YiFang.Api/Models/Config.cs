using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Models
{
    public static class Config
    {
        /*客诉4大类*/
        public const int material = 1;//原材料
        public const int facilitya = 2;//设备报修
        public const int operating = 3;//运营
        public const int other = 4;//其他

        /*枚举个大类*/
        public const int Entypeother = 4;//其他
        public const int product = 5;//产品
        public const int Abnormal = 6;//异常分类
        public const int unit = 7;//量（单位）
        public const int RepairType = 8;//报修类型
        public const int RepairProduct = 9;//报修产品
        public const int operatingoptions = 10;//其他-反馈选项
        public const int otheroptions = 11;//其他-反馈选项
        public const int Auxiliary = 23;//其他-辅训反馈选项

        /*附件*/
        public const string KS01 = "KS01";//客诉类型-图片
        public const string KS02 = "KS02";//客诉类型-音频
        public const string KS03 = "KS03";//客诉类型-视频

        public const string KS_BC01 = "KS_BC01";//客诉补充-图片
        public const string KS_BC02 = "KS_BC02";//客诉补充-音频
        public const string KS_BC03 = "KS_BC03";//客诉补充-视频
    }
}