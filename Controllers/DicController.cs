using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WJ.ModelToDoc.Dto;
using WJ.ModelToDoc.model;
using WJ.ModelToDoc.Util;
namespace WJ.ModelToDoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicController : ControllerBase
    {
        [HttpGet("{path}")]
        public string GetDicJson(string paths) {

            var tabs = EntityUtil.GetDicByPath(@"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\XDSYS.Entity.dll");
            // return NPOIUtil.CreateDicDocx(tabs, "aaa");
            return null;
        }
        /// <summary>
        /// 本地本机生成字典
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("CreateDicByDLL")]
        public object CreateDicByDLL(RequestDto dto)
        {
            //读取解析后的Json文件
            var tabs = EntityUtil.GetDicByPath(dto.Paths);
            var result= NPOIUtil.CreateDicDocx(tabs,dto);
            if (dto.IsRead)
            {
                var ms=result as System.IO.MemoryStream;
                return File(ms.ToArray(), "application/msword");
            }
            return result;
        }
        [HttpGet("CreateDicByDLL")]
        public object CreateDicByDLL()
        {
            RequestDto dto = new RequestDto()
            {
                Paths = @"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\XDSYS.Entity.dll",
                IsRead = true,
                WordName = "测试"
            };
            //读取解析后的Json文件
            var tabs = EntityUtil.GetDicByPath(dto.Paths);
            var result = NPOIUtil.CreateDicDocx(tabs, dto);
            if (dto.IsRead)
            {
                var ms = result as System.IO.MemoryStream;
                return File(ms.ToArray(), "application/msword");
            }
            return result;
        }
        [HttpGet("Test")]
        public string GetDicTest()
        {
            var type = typeof(User);
            var tabs= EntityUtil.GetTable(type);
            var t = new List<TableModel>();
            t.Add(tabs);
            return null;
            //return NPOIUtil.CreateDicDocx(t, "aaa");
        }

     }
}