using ICSharpCode.SharpZipLib.Zip;
using System.IO;
namespace WJ.ModelToDoc.Util
{
    /// <summary>
    /// SharpZipLib压缩文件
    /// </summary>
    public class SharpZipUtil
    {
        /// <summary>
        /// 压缩文件zip
        /// </summary>
        /// <param name="srcFile"></param>
        /// <param name="DstFile"></param>
        /// <param name="bufferSize"></param>
        public static void Zip(string srcFile, string DstFile,int bufferSize=1024)
        {
            var fileStreamIn = new FileStream(srcFile,FileMode.Open,FileAccess.Read);
            var fileStreamOut = new FileStream(DstFile, FileMode.Create, FileAccess.Write);
            var zipOutStream = new ZipOutputStream(fileStreamOut);
            var buffer = new byte[bufferSize];
            var entry = new ZipEntry(Path.GetFileName(srcFile));
            zipOutStream.PutNextEntry(entry);
            int size;
            do
            {
                size = fileStreamIn.Read(buffer, 0, buffer.Length);
                zipOutStream.Write(buffer, 0, size);
             } while (size > 0);
            zipOutStream.Close();
            fileStreamOut.Close();
            fileStreamIn.Close();
        }
        public static void UnZip(string srcFile,string DstFilte,int bufferSize=1024)
        {
            var fileStreamIn = new FileStream(srcFile, FileMode.Open, FileAccess.Read);
            var zipInStream = new ZipInputStream(fileStreamIn);
            var entry = zipInStream.GetNextEntry();
            var fileStreamOut = new FileStream(DstFilte+@"\"+entry.Name, FileMode.Create, FileAccess.Write);
            int size;
            var buffer = new byte[bufferSize];
            do
            {
                size = zipInStream.Read(buffer, 0, buffer.Length);
                fileStreamOut.Write(buffer,0,size);
            } while (size>0);
            zipInStream.Close();
            fileStreamOut.Close();
            fileStreamIn.Close();
        }
    }
}
