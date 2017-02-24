using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace versionProject
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = args[0];//@"C:\Users\Neha\Downloads\cvprep";
            string dest = args[1];//@"C:\Users\Neha\Downloads\cvPrepDest";

            bool res = CopyFolderContents(source,dest);
        }

        private static bool CopyFolderContents(string SourcePath, string DestinationPath)
        {
            SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
            DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

            try
            {
                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                    {
                        Directory.CreateDirectory(DestinationPath);
                    }

                    int count = Directory.GetFiles(SourcePath).Count();

                    foreach (string files in Directory.GetFiles(SourcePath))
                    {
                        
                        FileInfo fileInfo = new FileInfo(files);
                        if (fileInfo.Name.Contains('$'))
                        {

                        }
                        else
                        {
                            Directory.CreateDirectory(DestinationPath + fileInfo.Name);
                            fileInfo.CopyTo(string.Format(@"{0}\{1}", DestinationPath + fileInfo.Name, fileInfo.Name), true);
                        }
                    }

                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(drs);
                        if (CopyFolderContents(drs, DestinationPath + directoryInfo.Name) == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
