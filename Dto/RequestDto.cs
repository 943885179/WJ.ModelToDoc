using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.Dto
{
    public class RequestDto
    {
        /// <summary>
        /// dll地址，用";"分割多个dll
        /// </summary>
        public string Paths { get; set; }
        /// <summary>
        /// word名称
        /// </summary>
        public string WordName { get; set; }
        /// <summary>
        /// 是否为预览模式
        /// </summary>
        public bool IsRead { get; set; } = false;

        //ToDo mzj 注解
    }
}
