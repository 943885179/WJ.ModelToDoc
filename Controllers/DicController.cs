using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using WJ.ModelToDoc.Dto;
using WJ.ModelToDoc.model;
using WJ.ModelToDoc.Util;
using System.Text.Json;
using System.Linq;

namespace WJ.ModelToDoc.Controllers
{
    /// <summary>
    /// 字段生成Api
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DicController : ControllerBase
    {
        /// <summary>
        /// Asp.net Core程序的基本环境信息
        /// </summary>
        private static IWebHostEnvironment _environment;
        private static string _dllPath;
        private static string _dicPath;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="environment"></param>
        public DicController(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dllPath = Path.Combine(_environment.WebRootPath, "dll");
            _dicPath = Path.Combine(_environment.WebRootPath, "DicWord");

        }
        /// <summary>
        /// 上传文件解析成数据字典
        /// </summary>
        /// <param name="files">批量文件</param>
        /// <param name="wordName">字典名称</param>
        /// <param name="fromdlls">从哪几个dll中找数据实体，用";"分开</param>
        /// <param name="defalutKey">默认主键（全局，用";"分开</param>
        /// <param name="keyJson">特殊主键声明</param>
        /// <returns></returns>
        [HttpPost("CreateDicByDLL")]
        public object CreateDicByDLL(IFormFileCollection files, [FromForm] string wordName, [FromForm] string fromdlls, [FromForm] string defalutKey, [FromForm] string keyJson)
        {
            RemoveFile(_dllPath);
            SaveUploadFiles(files, _dllPath);
            UpLoadFilesIsTrue(files);
            var dto = new RequestDto()
            {
                Files = files,
                FilePath = _dicPath,
                WordName = wordName + ".doc",
                DefalutKey = defalutKey.ToLower(),
                tableKeys = JsonSerializer.Deserialize<Dictionary<string, string>>(keyJson.ToUpper())
            };
            fromdlls.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(d => dto.Paths += Path.Combine(_dllPath, d) + ";");
            var tabs = EntityUtil.GetDicByPath(dto.Paths, dto.DefalutKey, dto.tableKeys);
            var result = NPOIUtil.CreateDicDocx(tabs, dto);
            return Ok(new { code = 200, msg = "成功", Url = this.Request.Scheme + "://" + this.Request.Host + "/DicWord/" + dto.WordName });
        }
        /// <summary>
        /// 上传文件校验
        /// </summary>
        /// <param name="files"></param>
        private void UpLoadFilesIsTrue(IFormFileCollection files)
        {
            if (files == null || files.Count == 0)
            {
                throw new Exception("请上传实体的dll");
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="files"></param>
        /// <param name="path"></param>
        private void SaveUploadFiles(IFormFileCollection files, string path)
        {
            foreach (var file in files)
            {
                using (FileStream fs = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
        }
        /// <summary>
        /// 删除系统文件夹下的所有文件
        /// </summary>
        /// <param name="path"></param>
        private void RemoveFile(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }
    }
}
