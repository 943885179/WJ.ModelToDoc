using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.model
{
    public class TableModel
    {
        /// <summary>
        /// 表名
        /// 读取TableAttribute属性，如果没有则读取自身实体名称
        /// </summary>
        [DisplayName("表名")]
        public string Name { get; set; }
        /// <summary>
        /// 表描述
        /// 读取DisplayAttribute属性，如果没有则读取空
        /// </summary>
        [DisplayName("描述")]
        public string Display { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        [DisplayName("字段集合")]
        public List<ColumnModel> ColumnModels { get; set; }
    }
}
