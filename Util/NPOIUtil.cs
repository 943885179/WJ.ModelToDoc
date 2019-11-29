using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using WJ.ModelToDoc.Dto;
using WJ.ModelToDoc.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WJ.ModelToDoc.Util
{
    public class NPOIUtil
    {
        public static object CreateDicDocx(List<TableModel> tables, RequestDto dto)
        {
            #region // 创建Doc
            var doc = new XWPFDocument();
            var p1 = doc.CreateParagraph();
            p1.Alignment = ParagraphAlignment.CENTER;
            var titleRun = p1.CreateRun();
            titleRun.IsBold = false;//斜体
            titleRun.SetText(dto.WordName);
            titleRun.FontSize = 20;
            titleRun.SetFontFamily("黑体", FontCharRange.None);
            // 添加表
            var tabIndex = 0;
            foreach (var tab in tables)
            {
                tabIndex++;
                var p2 = doc.CreateParagraph();
                p2.Alignment = ParagraphAlignment.LEFT;
                var run1 = p2.CreateRun();
                run1.IsBold = false;
                run1.SetText(tabIndex+"."+tab.Name);
                run1.FontSize = 16;
                run1.SetFontFamily("宋体", FontCharRange.None);
                // 添加表描述
                var tabCard = doc.CreateTable(2,2);
                tabCard.Width = 5000;
                tabCard.SetColumnWidth(0, 1000);
                tabCard.SetColumnWidth(1,4000);
                tabCard.GetRow(0).GetCell(0).SetText("表名");
                tabCard.GetRow(0).GetCell(1).SetText(tab.Name);
                tabCard.GetRow(1).GetCell(0).SetText("描述");
                tabCard.GetRow(1).GetCell(1).SetText(string.IsNullOrEmpty(tab.Display)?"": tab.Display);
                //添加列
                var row = tab.ColumnModels.Count + 1;
                var p3 = doc.CreateParagraph();
                var colCard = doc.CreateTable(row, 7);
                colCard.Width = 5000;
                colCard.SetColumnWidth(0, 500);
                colCard.SetColumnWidth(1, 2000);
                colCard.SetColumnWidth(2, 500);
                colCard.SetColumnWidth(3, 500);
                colCard.SetColumnWidth(4, 500);
                colCard.SetColumnWidth(5, 500);
                colCard.SetColumnWidth(6, 500);
                colCard.GetRow(0).GetCell(0).SetParagraph(SetCellText(doc,colCard,"名称"));
                colCard.GetRow(0).GetCell(1).SetParagraph(SetCellText(doc,colCard,"描述"));
                colCard.GetRow(0).GetCell(2).SetParagraph(SetCellText(doc,colCard,"类型"));
                colCard.GetRow(0).GetCell(3).SetParagraph(SetCellText(doc,colCard,"是否为主键"));
                colCard.GetRow(0).GetCell(4).SetParagraph(SetCellText(doc,colCard,"是否强制"));
                colCard.GetRow(0).GetCell(5).SetParagraph(SetCellText(doc,colCard,"最小长度"));
                colCard.GetRow(0).GetCell(6).SetParagraph(SetCellText(doc, colCard, "最大长度"));
                for (int i = 0; i < tab.ColumnModels.Count; i++)
                {
                    var col = tab.ColumnModels[i];
                   colCard.GetRow(i+1).GetCell(0).SetParagraph(SetCellText(doc,colCard,col.Name));
                   colCard.GetRow(i+1).GetCell(1).SetParagraph(SetCellText(doc,colCard, string.IsNullOrEmpty(col.Display) ? "" : col.Display));
                   colCard.GetRow(i+1).GetCell(2).SetParagraph(SetCellText(doc,colCard,col.Type));
                   colCard.GetRow(i+1).GetCell(3).SetParagraph(SetCellText(doc,colCard,col.IsKey?"是":"否"));
                   colCard.GetRow(i+1).GetCell(4).SetParagraph(SetCellText(doc,colCard,col.IsRequire?"是":"否"));
                   colCard.GetRow(i+1).GetCell(5).SetParagraph(SetCellText(doc,colCard,col.MinLength == 0 ? "" : col.MinLength.ToString()));
                   colCard.GetRow(i+1).GetCell(6).SetParagraph(SetCellText(doc, colCard, col.MaxLength==0?"":col.MaxLength.ToString()));
                }
            }
            #endregion
            if (dto.IsRead)
            {// 直接下载，不做保存
                var ms = new MemoryStream();
                doc.Write(ms);
                return ms;
            }
            else
            {//下载到文件夹中
             // string docPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "DocxWord");
             // if (!Directory.Exists(docPath)) { Directory.CreateDirectory(docPath); }
             // string fileName = string.Format("{0}.doc", HttpUtility.UrlEncode(dto.WordName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), System.Text.Encoding.UTF8));
             // var path = Path.Combine(docPath, fileName);
                if (!Directory.Exists(dto.FilePath)) { Directory.CreateDirectory(dto.FilePath); }
                var path = Path.Combine(dto.FilePath, dto.WordName + ".doc");
                FileStream out1 = new FileStream(path, FileMode.Create);
                doc.Write(out1);
                out1.Close();
                return path;
            }
        }
        public static string CreateDoc()
        {
            //创建document对象
            var doc = new XWPFDocument();
            // 创建段落
            var p1 = doc.CreateParagraph();
            //字体设置居中
            p1.Alignment = ParagraphAlignment.CENTER;
            var runTitle = p1.CreateRun();
            runTitle.IsBold = true;
            runTitle.SetText("Hello");
            runTitle.FontSize = 16;
            runTitle.SetFontFamily("宋体", FontCharRange.None);//设置字体
            // 创建表格
            var tableTop = doc.CreateTable(3, 3);
            tableTop.Width = 1000 * 2;
            tableTop.SetColumnWidth(0, 1300);/* 设置列宽 */
            tableTop.SetColumnWidth(1, 500); /* 设置列宽 */
            tableTop.SetColumnWidth(2, 1000); /* 设置列宽 */
            tableTop.GetRow(0).MergeCells(1, 2); /* 合并行 */
            tableTop.GetRow(0).GetCell(0).SetText("123");
            tableTop.GetRow(0).GetCell(1).SetParagraph(SetCellText(doc, tableTop, "检验要素"));

            //var ms = new MemoryStream();
            //doc.Write(ms);
            string docPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "DocxWord");
            if (!Directory.Exists(docPath)) { Directory.CreateDirectory(docPath); }
            string fileName = string.Format("{0}.doc", HttpUtility.UrlEncode("jjysd" + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), System.Text.Encoding.UTF8));
            FileStream out1 = new FileStream(Path.Combine(docPath, fileName), FileMode.Create);
            doc.Write(out1);
            out1.Close();
            return Path.Combine(docPath, fileName);
        }
        /// <summary>
        ///     设置字体格式
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="table"></param>
        /// <param name="setText"></param>
        /// <returns></returns>
        public static XWPFParagraph SetCellText(XWPFDocument doc, XWPFTable table, string setText)
        {
            //table中的文字格式设置
            var para = new CT_P();
            var pCell = new XWPFParagraph(para, table.Body);
            pCell.Alignment = ParagraphAlignment.CENTER; //字体居中
            pCell.VerticalAlignment = TextAlignment.CENTER; //字体居中

            var r1c1 = pCell.CreateRun();
            r1c1.SetText(setText);
            r1c1.FontSize = 12;
            r1c1.SetFontFamily("华文楷体", FontCharRange.None); //设置雅黑字体

            return pCell;
        }
        /// <summary>
        ///     设置单元格格式
        /// </summary>
        /// <param name="doc">doc对象</param>
        /// <param name="table">表格对象</param>
        /// <param name="setText">要填充的文字</param>
        /// <param name="align">文字对齐方式</param>
        /// <param name="textPos">rows行的高度</param>
        /// <returns></returns>
        public XWPFParagraph SetCellText(XWPFDocument doc, XWPFTable table, string setText, ParagraphAlignment align,
            int textPos)
        {
            var para = new CT_P();
            var pCell = new XWPFParagraph(para, table.Body);
            //pCell.Alignment = ParagraphAlignment.LEFT;//字体
            pCell.Alignment = align;

            var r1c1 = pCell.CreateRun();
            r1c1.SetText(setText);
            r1c1.FontSize = 12;
            r1c1.SetFontFamily("华文楷体", FontCharRange.None); //设置雅黑字体
            r1c1.SetTextPosition(textPos); //设置高度

            return pCell;
        }
}
}
