using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using WJ.ModelToDoc.Dto;
using WJ.ModelToDoc.model;
using WJ.ModelToDoc.Util;
namespace WJ.ModelToDoc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DicController : ControllerBase
    {
        private static IHostingEnvironment _environment;
        public DicController(IHostingEnvironment environment)
        {
            _environment = environment;
        }
        /// <summary>
        /// 本地本机生成字典
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("CreateDicByDLL")]
        public object CreateDicByDLL(IFormFileCollection files, string wordName)
        {
            GC.Collect();
            var dto = new RequestDto()
            {
                Files = files,
                WordName = wordName,
            };
            if (dto.Files == null || dto.Files.Count == 0)
            {
                return "请上传实体的dll";
            }
            var dllPath = Path.Combine(_environment.WebRootPath, "dll");
            if (Directory.Exists(dllPath))
            {
                Directory.Delete(dllPath, true);
            }
            Directory.CreateDirectory(dllPath);
            var pathList = new List<string>();
            foreach (var file in dto.Files)
            {
                /* dto.Paths += Path.Combine(dllPath, file.FileName) + ";";
                 using (FileStream fs = System.IO.File.Create(Path.Combine(dllPath,file.FileName)))
                 {
                     file.CopyTo(fs);
                     fs.Flush();
                 }*/
                pathList.Add(file.FileName);
            }
            DotnetZipUtil.CompressMulti(pathList, Path.Combine(_environment.WebRootPath, "t1.zip"), true);
            dto.FilePath = _environment.WebRootPath + "/DicWord";
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
        [HttpGet("CreateDicByDLL")]
        public object CreateDicByDLL()
        {
            RequestDto dto = new RequestDto()
            {
                //Paths = @"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\XDSYS.Entity.dll",
                Paths = @"F:\维加\WJ.ModelToDoc\wwwroot\dll\XDSYS.Entity.dll",
                IsRead = false,
                WordName = "数据字典"
            };
            dto.FilePath = Path.Combine(_environment.WebRootPath, "DicWord");
            //读取解析后的Json文件
            var tabs = EntityUtil.GetDicByPath(dto.Paths);
            var result = NPOIUtil.CreateDicDocx(tabs, dto);
            if (dto.IsRead)
            {
                var ms = result as System.IO.MemoryStream;
                return File(ms.ToArray(), "application/msword");
            }
            return Ok(new { code = 200, msg = "成功", Url = this.Request.Scheme + "://" + this.Request.Host + "/DicWord/" + dto.WordName + ".doc" });
        }



        [HttpGet("Zip")]
        public string Zip()
        {
            // SharpZipUtil.Zip(@"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\System.Net.Http.dll", @"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\1.zip");
            DotnetZipUtil.CompressMulti(new List<string>() { @"G:\SMS.dll" }, Path.Combine(_environment.WebRootPath, "t1.zip"), false);
            return "压缩";
        }

        [HttpGet("UpZip")]
        public string UpZip()
        {
            // SharpZipUtil.Zip(@"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\System.Net.Http.dll", @"F:\维加\xdsys-dy\XDSYS.Entity\bin\Debug\1.zip");
            DotnetZipUtil.UnZip(Path.Combine(_environment.WebRootPath, "t1.zip"), Path.Combine(_environment.WebRootPath, "dll"));
            return "解压";
        }

        //[HttpGet("Test")]
        //public string GetDicTest()
        //{
        //    var type = typeof(User);
        //    var tabs= EntityUtil.GetTable(type);

        //    var t = new List<TableModel>();
        //    t.Add(tabs);
        //    return null;
        //    //return NPOIUtil.CreateDicDocx(t, "aaa");
        //}

    }
}