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
       
        private bool running { get; set; }

        public void DivideWorkAsync(string rootPath)
        {
            _form1 = Form1.InstanceForm1;
            string[] filesZip = Directory.GetFiles(rootPath, "*.zip", SearchOption.AllDirectories);
            string[] filesTarGz = Directory.GetFiles(rootPath, "*.tar.gz", SearchOption.AllDirectories);
            string[] filesGz = Directory.GetFiles(rootPath, "*.gz", SearchOption.AllDirectories);
            List<string> filesGzFinal = new List<string>();

            //filesGzFinal = (List<string>)filesGz.ToList().Where(x => !x.EndsWith(".tar.gz"));
            for(int i = 0; i < filesGz.Length; i++)
            {
                if (!filesGz[i].EndsWith(".tar.gz"))
                {
                    filesGzFinal.Add(filesGz[i]);
                }
            }
            
            if (filesZip.Length == 0 && filesTarGz.Length == 0 && filesGzFinal.Count == 0)
                _form1.Message("There are no .zip or .tar.gz or .gz files");
            else
                running = true;

            var extractingZip = Task.Run(() => Unzip(filesZip));
            var extractingTar = Task.Run(() => Untar(filesTarGz));
            var extractingGz = Task.Run(() => Ungz(filesGzFinal.ToArray()));
            
            var zipCompleted = extractingZip.GetAwaiter().GetResult();
            var tarCompleted = extractingTar.GetAwaiter().GetResult();
            var gzCompleted = extractingGz.GetAwaiter().GetResult();
            
            DeleteFiles(filesZip, zipCompleted);
            DeleteFiles(filesTarGz, tarCompleted);
            DeleteFiles(filesGzFinal.ToArray(), gzCompleted);
        }

        public bool Unzip(string[] files)
        {
            _form1 = Form1.InstanceForm1;
            
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string dirName = Path.GetFileName(files[i]);
                    string originalDir = Path.GetDirectoryName(files[i]);
                    string newDirName = dirName.Substring(0, dirName.Length - 4);
                    string newPath = originalDir + "/" + newDirName;
                    Directory.CreateDirectory(newPath);
                    ZipFile.ExtractToDirectory(files[i], newPath);
                }
            }
            catch(Exception e)
            {
                _form1.Message($"Problem occured during extraction .zip " + e);
            }
            return true;
        } 
        public bool Untar(string[] files)
        {
            _form1 = Form1.InstanceForm1;
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo tarFileInfo = new FileInfo(files[i]);
                    DirectoryInfo targetDirectory = new DirectoryInfo(Path.GetDirectoryName(files[i]));
  
                    using (Stream sourceStream = new GZipInputStream(tarFileInfo.OpenRead()))
                    {
                        using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                        {
                            tarArchive.ExtractContents(targetDirectory.FullName);
                            tarArchive.Close();
                            tarArchive.Dispose();
                        }
                        sourceStream.Close();
                        sourceStream.Dispose();
                    }                    
                }  
            }
            catch(Exception e)
            {
                _form1.Message("Problem occured during extraction .tar.gz " + e);
            }
            return true;
        }
        public bool Ungz(string[] files)
        {
            _form1 = Form1.InstanceForm1;
            try
            {
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileToDecompress = new FileInfo(files[i]);
                    using (FileStream originalFileStream = fileToDecompress.OpenRead())
                    {
                        string currentFileName = fileToDecompress.FullName;
                        string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                        using (FileStream decompressedFileStream = File.Create(newFileName))
                        {
                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                decompressionStream.CopyTo(decompressedFileStream);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                _form1.Message("Problem occured during extraction .gz " + e);
            }

            return true;
        }
        public void UnBr(string[] files)
        {
            if (running == false)
                return;

            BrotliDecoder brotliDecoder = new BrotliDecoder();

            for(int i = 0; i < files.Length; i++)
            {
                /*
                byte[] input = Encoding.UTF8.GetBytes()
                brotliDecoder.Decompress()
                */
            }  
        }

        private void DeleteFiles(string[] files, bool isReady)
        {
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }
        public void StopIt()
        {
            running = false;
        }
    }
}
