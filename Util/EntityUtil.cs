using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WJ.ModelToDoc.model;

namespace WJ.ModelToDoc.Util
{
    /// <summary>
    /// 实体生成数据类别工具类
    /// </summary>
    public class EntityUtil
    {
        public static List<TableModel> GetDicByPath(string paths)
        {
            var result = new List<TableModel>();
            // path = @"F:\维加\WJ.ModelToDoc\bin\Debug\netcoreapp3.0\WJ.ModelToDoc.dll";
            var pathSp = paths.Split(';');
            foreach (var path in pathSp)
            {
                var asm = Assembly.LoadFrom(path);
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    var tab = GetTable(type);
                    if (tab != null && tab.ColumnModels.Any(o => o.IsKey))
                    {// 存在主键的才会生成
                        result.Add(tab);
                    }
                }
            }
            return result;
        }

        public static TableModel GetTable(Type type)
        {
            // 读取每张表
            var tab = new TableModel()
            {
                Name = type.Name,
                ColumnModels = new List<ColumnModel>()
            };
            //是否有TableAttribute，有则读取为表名
            var tabNameAtrris = type.GetCustomAttributes(typeof(TableAttribute), false);
            foreach (TableAttribute tabNameAtrri in tabNameAtrris)
            {
                tab.Name = tabNameAtrri.Name;
            }
            //读取时候有备注，有则读取为字段备注
            var tabDisPlayAtrris = type.GetCustomAttributes(typeof(DisplayAttribute), false);
            foreach (DisplayAttribute tabDis in tabDisPlayAtrris)
            {
                tab.Display = tabDis.Name;
            }
            // 开始遍历字段
            var cols = type.GetProperties(); // 获取字段
            foreach (var col in cols)
            {
                var column = new ColumnModel()
                {
                    Name = col.Name, //字段名称
                    Type = col.PropertyType.Name,// 字段类型
                    IsKey = false,
                    IsRequire = false
                };
                var propertyType = col.PropertyType;

                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {//针对可空类型转化为读取原始类型
                    propertyType = propertyType.GetGenericArguments()[0];
                    column.Type = propertyType.Name;
                }
                else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {//IsGenericType，是否是泛型,泛型只接受可空标识，其他类型List和ICollection 一律排除
                    continue;
                }
                else if (propertyType.IsClass)
                {//排除引入的类字段
                    continue;
                }
                else { }

                //读取字段名
                var colNames = col.GetCustomAttributes(typeof(ColumnAttribute), false);
                foreach (ColumnAttribute colName in colNames)
                {
                    column.Display = colName.Name;
                }
                //读取字段备注
                var colDiss = col.GetCustomAttributes(typeof(DisplayAttribute), false);
                foreach (DisplayAttribute colDis in colDiss)
                {
                    column.Display = colDis.Name;
                }
                //读取字段主键
                var colKeys = col.GetCustomAttributes(typeof(KeyAttribute), false);
                foreach (KeyAttribute colKey in colKeys)
                {
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
                tab.ColumnModels.Add(column);
            }
            return tab;
        }

    }
}
