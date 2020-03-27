using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.Dto
{
    public class RequestDto
    {
        public string Fromdlls { get; set; }
        /// <summary>
        /// dll地址，用";"分割多个dll
        /// </summary>
        public string Paths { get; set; }
        /// <summary>
        /// 上传Dll,多个
        /// </summary>
        public IFormFileCollection Files { get; set; }
        /// <summary>
        /// word名称
        /// </summary>
        public string WordName { get; set; }
        /// <summary>
        /// 存放路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 全局默认主键
        /// </summary>
        public string DefalutKey { get; set; }
        /// <summary>
        /// 主键表对照
        /// </summary>
        public Dictionary<string,string> tableKeys { get; set; }
    }
}
