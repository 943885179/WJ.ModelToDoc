using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using WJ.ModelToDoc.model;

namespace WJ.ModelToDoc.Util
{
    /// <summary>
    /// 实体生成数据类别工具类(存在一个问题就是不能重复加载相同的dll)
    /// </summary>
    public class EntityUtil
    {
        protected EntityUtil()
        {
        }
        /// <summary>
        /// 获取数据json
        /// 主键生成规则（有IsKey注解、符合默认主键属性、符合主键对应关系字段）
        /// </summary>
        /// <param name="paths">解析的dll</param>
        /// <param name="defalutKey">默认主键,多个使用;分割</param>
        /// <param name="keyDic"></param>
        /// <returns></returns>
        public static List<TableModel> GetDicByPath(string paths, string defalutKey = "", Dictionary<string, string> keyDic = null)
        {
            try
            {
                var result = new List<TableModel>();
                var pathSp = paths.Split(';');
                foreach (var path in pathSp)
                {
                    if (string.IsNullOrWhiteSpace(path))
                        continue;
                    // loadfrom和loadFile会锁住文件无法删除修改 Assembly.LoadFile只载入相应的dll文件 Assembly.LoadFrom则不一样，它会载入dll文件及其引用的其他dll
                    // Assembly.LoadFrom(程序集的文件路径，包括扩展名)而Assembly.Load（程序集名称,而不是文件名）
                    var load = new LoadUtil();
                    var asm = load.LoadForm(path);
                    var types = asm.GetTypes();
                    foreach (var type in types)
                    {
                        var tab = GetTable(type, defalutKey, keyDic);
                        if (tab != null && tab.ColumnModels.Any(o => o.IsKey))
                        {// 存在主键的才会生成
                            result.Add(tab);
                        }
                    }
                    load.UnLoad();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 读取表信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="defalutKey"></param>
        /// <param name="keyDic"></param>
        /// <returns></returns>
        public static TableModel GetTable(Type type, string defalutKey, Dictionary<string, string> keyDic = null)
        {
            var tab = new TableModel() { Name = type.Name, ColumnModels = new List<ColumnModel>() };
            if (keyDic.Any(o => o.Key == tab.Name.ToUpper()))
                defalutKey = keyDic[tab.Name.ToUpper()];
            tab.Name = TableAttributeName(type);
            tab.Display = DisplayAttributeName(type);
            tab.Display = DescriptAttributeName(type);
            var cols = type.GetProperties(); // 获取字段
            foreach (var col in cols)
            {
                var colModel = GetCol(col, defalutKey);
                if (colModel != null)
                    tab.ColumnModels.Add(colModel);
            }
            tab.ColumnModels = tab.ColumnModels.OrderByDescending(x => x.IsKey).ToList();
            return tab;
        }
        /// <summary>
        /// 读取具体字段信息
        /// </summary>
        /// <param name="col"></param>
        /// <param name="defalutKey"></param>
        /// <param name="keyDic"></param>
        /// <returns></returns>
        public static ColumnModel GetCol(PropertyInfo col, string defalutKey)
        {
            var column = new ColumnModel()
            {
                Name = col.Name, //字段名称
                Type = col.PropertyType.Name,// 字段类型
                IsKey = IsHaveKeyAttribute(col),
                IsRequire = false
            };
            //读取字段名
            column.Display = ColumnAttributeName(col);
            //读取字段备注
            column.Display = DisplayAttributeName(col);
            column.Display += DescriptAttributeName(col);
            var propertyType = col.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {//针对可空类型转化为读取原始类型
                propertyType = propertyType.GetGenericArguments()[0];
                column.Type = propertyType.Name;
            }
            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {//IsGenericType，是否是泛型,泛型只接受可空标识，其他类型List和ICollection 一律排除
                return null;
            }
            // else if (propertyType.IsClass)
            // {//排除引入的类字段
            //     continue;
            // }
            else if (propertyType.IsEnum)
            {//排除引入的类字段
                var emums = propertyType.GetFields(BindingFlags.Static | BindingFlags.Public);
                var sb = new StringBuilder();
                foreach (var emu in emums)
                {
                    var emumStr = "【" + emu.Name + ":" + emu.GetRawConstantValue() + "】"; 
                    var emuAttr = emu.GetCustomAttributes(typeof(DisplayAttribute), false);
                    foreach (DisplayAttribute at in emuAttr)
                    {
                        emumStr += at.Name;
                    }
                    var emuAttrs = emu.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    foreach (DescriptionAttribute at in emuAttrs)
                    {
                        emumStr += at.Description;
                    }
                    sb.AppendLine(emumStr);
                }
                column.Display += sb.ToString();
            }
            else { }
            if (defalutKey.Split(';').Any(o => o == column.Name.ToUpper()))
            {//设置该字段为主键，默认Id为主键；
                column.IsKey = true;
            }
            //读取字段是否强制
            var colReqs = col.GetCustomAttributes(typeof(RequiredAttribute), false);
            foreach (RequiredAttribute colReq in colReqs)
            {
                column.IsRequire = true;
            }
            //读取字段最长、最短长度
            var colLengths = col.GetCustomAttributes(typeof(StringLengthAttribute), false);
            foreach (StringLengthAttribute colLength in colLengths)
            {
                column.MaxLength = colLength.MaximumLength;
                column.MinLength = colLength.MinimumLength;
            }
            //读取字段最长长度
            var colMaxLengths = col.GetCustomAttributes(typeof(MaxLengthAttribute), false);
            foreach (MaxLengthAttribute colMaxLength in colMaxLengths)
            {
                column.MaxLength = colMaxLength.Length;
            }
            //读取字段最短长度
            var colMinLengths = col.GetCustomAttributes(typeof(MinLengthAttribute), false);
            foreach (MinLengthAttribute colMinLength in colMinLengths)
            {
                column.MinLength = colMinLength.Length;
            }
            return column;
        }

        /// <summary>
        /// 读取TableAttribute的name;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string TableAttributeName(Type type)
        {
            var result = "";
            var x = type.GetCustomAttribute(typeof(TableAttribute));
            var tabNameAtrris = type.GetCustomAttributes(typeof(TableAttribute), false);
            foreach (TableAttribute tabNameAtrri in tabNameAtrris)
            {
                result = tabNameAtrri.Name;
                break;
            }
            return result;
        }
        /// <summary>
        /// 读取DisplayAttribute的name;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string DisplayAttributeName(Type type)
        {
            var result = "";
            var displayAtrris = type.GetCustomAttributes(typeof(DisplayAttribute), false);
            foreach (DisplayAttribute displayAtrri in displayAtrris)
            {
                result = displayAtrri.Name;
                break;
            }
            return result;
        }
        /// <summary>
        /// 读取DescriptionAttribute的Description;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string DescriptAttributeName(Type type)
        {
            //是否有TableAttribute，有则读取为表名
            var result = "";
            var descriptionAttributes = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            foreach (DescriptionAttribute descriptionAttribute in descriptionAttributes)
            {
                result = descriptionAttribute.Description;
                break;
            }
            return result;
        }
         /// <summary>
        /// 读取DisplayAttribute的name;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string DisplayAttributeName(PropertyInfo type)
        {
            var result = "";
            var displayAtrris = type.GetCustomAttributes(typeof(DisplayAttribute), false);
            foreach (DisplayAttribute displayAtrri in displayAtrris)
            {
                result = displayAtrri.Name;
                break;
            }
            return result;
        }
        /// <summary>
        /// 读取DescriptionAttribute的Description;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string DescriptAttributeName(PropertyInfo type)
        {
            //是否有TableAttribute，有则读取为表名
            var result = "";
            var descriptionAttributes = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            foreach (DescriptionAttribute descriptionAttribute in descriptionAttributes)
            {
                result += descriptionAttribute.Description;
            }
            return result;
        }
        /// <summary>
        /// 读取KeyAttribute;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsHaveKeyAttribute(PropertyInfo type)
        {
            var keyAttributes = type.GetCustomAttributes(typeof(KeyAttribute), false);
            return keyAttributes.Length > 0;
        }
        /// <summary>
        /// 读取ColumnAttribute的name;
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string ColumnAttributeName(PropertyInfo type)
        {
            var result = "";
            var displayAtrris = type.GetCustomAttributes(typeof(ColumnAttribute), false);
            foreach (ColumnAttribute displayAtrri in displayAtrris)
            {
                result = displayAtrri.Name;
                break;
            }
            return result;
        }
    }
}
