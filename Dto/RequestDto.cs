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
        public IFormFile File { get; set; }
        /// <summary>
        /// word名称
        /// </summary>
        public string WordName { get; set; }
        /// <summary>
        /// 是否为预览模式
        /// </summary>
        public bool IsRead { get; set; } = false;
        /// <summary>
        /// 存放路径
        /// </summary>
        public string FilePath { get; set; }

        public string DefaulteId { get; set; }
        public Dictionary<string,string> Dic { get; set; }

        //ToDo mzj 注解
    }
}
