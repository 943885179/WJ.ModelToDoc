using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.model
{
    /// <summary>
    /// 列名描述
    /// </summary>
    public class ColumnModel
    {
        /// <summary>
        /// 字段名
        /// 读取ColumnAttribute的Name属性，没有则读取自身
        /// </summary>
        [DisplayName("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 字段描述
        /// 读取DisplayAttribute的Name属性,没有读取空
        /// </summary>
        [DisplayName("描述")]
        public string Display { get; set; }
        /// <summary>
        /// 字段类型
        /// 读取字段的类型
        /// </summary>
        [DisplayName("类型")]
        public string Type { get; set; }
        /// <summary>
        /// 是否为主键
        /// 读取KeyAttribute，存在则为主键
        /// </summary>
        [DisplayName("主键")]
        public bool IsKey { get; set; }
        /// <summary>
        /// 是否强制
        /// 读取RequiredAttribute，存在则强制
        /// </summary>
        [DisplayName("强制")]
        public bool IsRequire { get; set; }
        /// <summary>
        /// 最少长度
        /// 读取MinLengthAttribute或者StringLengthAttribute
        /// </summary>
        [DisplayName("最少长度")]
        public int MinLength { get; set; }
        /// <summary>
        /// 最大长度
        /// 读取MinLengthAttribute或者StringLengthAttribute
        /// </summary>
        [DisplayName("最大长度")]
        public int MaxLength { get; set; }

    }
}
