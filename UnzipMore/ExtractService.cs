using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using SharpCompress;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;

namespace UnzipMore
{
    public class ExtractService
    {
        private Form1 _form1;
        bool zipIsZero = false;
        bool tarIsZero = false;
        private bool running { get; set; }

        public void Unzip(string rootPath)
        {
            running = true;
            _form1 = Form1.InstanceForm1;
            
            try
            {
                string[] files = Directory.GetFiles(rootPath,"*.zip",SearchOption.AllDirectories);
                
                if(files.Length == 0)
                {
                    zipIsZero = true;
                }
                for (int i = 0; i < files.Length; i++)
                {
                    if (running == false)
                        return;
                    string dirName = Path.GetFileName(files[i]);
                    string originalDir = Path.GetDirectoryName(files[i]);
                    string newDirName = dirName.Substring(0, dirName.Length - 4);
                    string newPath = originalDir + "/" + newDirName;
                    Directory.CreateDirectory(newPath);
                    ZipFile.ExtractToDirectory(files[i], newPath);
                    
                        File.Delete(files[i]);
                }
            }
            catch(Exception e)
            {
                _form1.Message($"Problem occured during extraction ZIP. " + e);
            } 
        } 
        public void Untar(string rootPath)
        {
            _form1 = Form1.InstanceForm1;
            try
            {
                string[] files = Directory.GetFiles(rootPath, "*.gz", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    tarIsZero = true;
                }
                if (tarIsZero == true && zipIsZero == true)
                    _form1.Message("There are no .zip or .tar.gz files");
                for (int i = 0; i < files.Length; i++)
                {
                    if (running == false)
                        return;
                    FileInfo tarFileInfo = new FileInfo(files[i]);
                    DirectoryInfo targetDirectory = new DirectoryInfo(Path.GetDirectoryName(files[i]));
  
                    using (Stream sourceStream = new GZipInputStream(tarFileInfo.OpenRead()))
                    {
                        using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                        {
                            tarArchive.ExtractContents(targetDirectory.FullName);
                        }
                    }
                    
                        File.Delete(files[i]);
                }  
            }
            catch(Exception e)
            {
                _form1.Message("Problem occured during extraction TAR. " + e);
            }   
        }
        public void StopIt()
        {
            running = false;
        }
    }
}
