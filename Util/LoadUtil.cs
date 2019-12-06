using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace WJ.ModelToDoc.Util
{
    public class LoadUtil
    {
        /// <summary>
        /// 核心程序集加载
        /// </summary>
        public AssemblyLoadContext _AssemblyLoadContext { get; set; }
        /// <summary>
        /// 获取程序集
        /// </summary>
        public Assembly _Assembly { get; set; }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string filePath = string.Empty;
        /// <summary>
        /// 指定位置的插件库集合
        /// </summary>
        AssemblyDependencyResolver resolver { get; set; }

        public Assembly LoadForm(string filePath)
        {
            this.filePath = filePath;
            try
            {
                resolver = new AssemblyDependencyResolver(filePath);
                _AssemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString("N"), true);
                _AssemblyLoadContext.Resolving += AssemblyLoadContext_Resolving;

                //var assembly = assemblyLoadContext.LoadFromAssemblyPath(filePath);
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var assembly = _AssemblyLoadContext.LoadFromStream(fs);
                    return assembly;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return null;
        }
        public Assembly AssemblyLoadContext_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            var path = resolver.ResolveAssemblyToPath(arg2);
            if (!string.IsNullOrEmpty(path))
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    return _AssemblyLoadContext.LoadFromStream(fs);
                }
            }
            return null;
        }
        public void UnLoad()
        {
            try
            {
                _AssemblyLoadContext?.Unload();
            }
            catch (Exception)
            { }
            finally
            {
                _AssemblyLoadContext = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

    }
}
