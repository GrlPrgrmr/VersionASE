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
        private static List<string> fileTreeData;

        public static List<string> FileTreeData
        {
            get { return fileTreeData; }
            set { fileTreeData = value; }
        }



        static void Main(string[] args)
        {
            FileTreeData = new List<string>();
            Console.WriteLine("Enter Command : ");
            string command = Console.ReadLine();
            switch (command)
            {
                case "Create Repo":
                    {
                        Console.WriteLine("Enter Source Path for Create Repo : \n");
                        string source = Console.ReadLine();//@"C:\Users\Neha\Downloads\cvprep";
                        Console.WriteLine("Enter Destination Path : \n");
                        string dest = Console.ReadLine();//@"C:\Users\Neha\Downloads\cvPrepDest";
                        bool res = CopyFolderContents(source, dest);

                        if (res == true)
                        {
                            //create manifest file with global filetreedata and other details
                            manifestWrite(command, source, dest);

                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Invalid Command");
                        break;
                    }
            }

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

                    foreach (string file in Directory.GetFiles(SourcePath))
                    {

                        FileInfo fileInfo = new FileInfo(file);
                        if (fileInfo.Name.Contains('$'))
                        {

                        }
                        else
                        {
                            Directory.CreateDirectory(DestinationPath + fileInfo.Name);
                            string artifactIDFile = CreateArtifactID(fileInfo.DirectoryName + "\\" + fileInfo.Name);
                            fileInfo.CopyTo(string.Format(@"{0}\{1}", DestinationPath + fileInfo.Name, artifactIDFile + fileInfo.Extension), true);
                            FileTreeData.Add(artifactIDFile + fileInfo.Extension + "\t" + fileInfo.Name + "\t" + fileInfo.DirectoryName);
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

        private static string CreateArtifactID(string path)
        {
            try
            {
                string fileContent;
                string artifactID = "";

                int checkSum = 0;
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                using (StreamReader sr = new StreamReader(fs))
                {
                    fileContent = sr.ReadToEnd();
                }

                char[] fileDataArray = fileContent.ToCharArray();

                for (int i = 0; i < fileContent.Length; i++)
                {
                    switch (i % 4)
                    {
                        case 0:
                            checkSum += ((int)fileDataArray[i] * 1);
                            break;

                        case 1:
                            checkSum += ((int)fileDataArray[i] * 3);
                            break;

                        case 2:
                            checkSum += ((int)fileDataArray[i] * 11);
                            break;

                        case 3:
                            checkSum += ((int)fileDataArray[i] * 17);
                            break;
                    }
                }


                artifactID = checkSum.ToString() + "." + fileContent.Length.ToString();

                return artifactID;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                Console.WriteLine(err);
                return "";
            }


        }



        public static void manifestWrite(String command, String source, String destination)
        {
            var projectName = source.Split('\\');
            String name = "Project Name : "+projectName[projectName.Length - 1];
            DateTime timeStamp = DateTime.Now;
            String data = name + "\n Source Path of the project :" + "\t" + source + "\t" + "\n Destination Path of the project :" + destination + "\n Time of Creation: "+"\t" +timeStamp.ToString();
            destination += "\\activity";
            try
            {
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }
                File.AppendAllText(Path.Combine(destination, "manifest.txt"), data);
                foreach(var eachLine in FileTreeData){
                    File.AppendAllText(Path.Combine(destination, "manifest.txt"), "\n"+eachLine);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
