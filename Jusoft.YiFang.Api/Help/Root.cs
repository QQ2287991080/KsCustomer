using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Help
{
    public class Form_component_values
    {
        /// <summary>
        /// 测试1
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 测试1
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 987654321
        /// </summary>
        public string value { get; set; }
    }
    public class Operation_records
    {
        /// <summary>
        /// 2019-03-26 09:42:44
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// NONE
        /// </summary>
        public string operation_result { get; set; }
        /// <summary>
        /// START_PROCESS_INSTANCE
        /// </summary>
        public string operation_type { get; set; }
        /// <summary>
        /// 165508095024967439
        /// </summary>
        public string userid { get; set; }
    }
    public class Tasks
    {
        /// <summary>
        /// 2019-03-26 09:42:44
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 2019-03-26 09:42:51
        /// </summary>
        public DateTime finish_time { get; set; }
        /// <summary>
        /// AGREE
        /// </summary>
        public string task_result { get; set; }
        /// <summary>
        /// COMPLETED
        /// </summary>
        public string task_status { get; set; }
        /// <summary>
        /// 61001738360
        /// </summary>
        public string taskid { get; set; }
        /// <summary>
        /// 165508095024967439
        /// </summary>
        public string userid { get; set; }
    }
    public class Process_instance
    {
        /// <summary>
        /// Attached_process_instance_ids
        /// </summary>
        public List<string> attached_process_instance_ids { get; set; }
        /// <summary>
        /// NONE
        /// </summary>
        public string biz_action { get; set; }
        /// <summary>
        /// 201903260942000442960
        /// </summary>
        public string business_id { get; set; }
        /// <summary>
        /// 2019-03-26 09:42:44
        /// </summary>
        public DateTime create_time { get; set; }
        /// <summary>
        /// 2019-03-26 09:42:52
        /// </summary>
        public DateTime finish_time { get; set; }
        /// <summary>
        /// Form_component_values
        /// </summary>
        public List<Form_component_values> form_component_values { get; set; }
        /// <summary>
        /// Operation_records
        /// </summary>
        public List<Operation_records> operation_records { get; set; }
        /// <summary>
        /// -1
        /// </summary>
        public string originator_dept_id { get; set; }
        /// <summary>
        /// S团队
        /// </summary>
        public string originator_dept_name { get; set; }
        /// <summary>
        /// 165508095024967439
        /// </summary>
        public string originator_userid { get; set; }
        /// <summary>
        /// agree
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// COMPLETED
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Tasks
        /// </summary>
        public List<Tasks> tasks { get; set; }
        /// <summary>
        /// 戚昌威提交的aaaa
        /// </summary>
        public string title { get; set; }
    }
    public class Root
    {
        /// <summary>
        /// Errcode
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// Process_instance
        /// </summary>
        public Process_instance process_instance { get; set; }
        /// <summary>
        /// 4lx1bamyi7c5
        /// </summary>
        public string request_id { get; set; }
    }
    public class rowValue
    {
        /// <summary>
        /// 报销事项
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 详见附件
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
    }
    public class Details
    {
        /// <summary>
        /// 
        /// </summary>
        public List<rowValue> rowValue { get; set; }
    }
    public class Enclosure
    {
        /// <summary>
        /// 
        /// </summary>
        public string authMediaId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string spaceId { get; set; }
        /// <summary>
        /// 程贵玲-5月10日涪陵出差-票据.pdf
        /// </summary>
        public string fileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int fileSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string fileId { get; set; }
    }
}