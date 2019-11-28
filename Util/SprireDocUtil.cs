using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.Util
{
    /// <summary>
    /// sprire 试用版会加水印，正式发布来说不合适
    /// </summary>
    public class SprireDocUtil
    {
        public static void CreatDoc()
        {
            //创建一个Document实例
            Document doc = new Document();
            doc.AddSection();
            //添加一个section
            Section s = doc.AddSection();
            //添加段落
            Paragraph para = s.AddParagraph();
            //添加文字
            para.AppendText("Hello World!");
            //添加样式
            ParagraphStyle style = new ParagraphStyle(doc);
            style.Name = "titleStyle";
            style.CharacterFormat.Bold = true;
            style.CharacterFormat.TextColor = Color.Purple;
            style.CharacterFormat.FontName = "宋体";
            style.CharacterFormat.FontSize = 12f;
            doc.Styles.Add(style);
            para.ApplyStyle("titleStyle");
            para.Format.HorizontalAlignment = HorizontalAlignment.Center;// 对齐方式
            //添加段落
            Paragraph para1 = s.AddParagraph();
            //添加文字
            para1.AppendText("Hello World11111!");
            para1.Format.FirstLineIndent = 30f;//首缩进
            para1.Format.AfterSpacing = 15f;//段落间距
            doc.SaveToFile("test.docx", FileFormat.Docm2013);
        }
    }
}
