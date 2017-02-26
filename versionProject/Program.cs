/*
 Application Name: VersionProject
 Owners: Neha Tyagi 
 *       Prerna Nain
 *       Mani Khanuja
 *       Sahib Malhotra
 *Brief Description: This application is intended for implementing a command Create Repo for creating repository of folder system provided in the
 *                   source path by user. As result this application will create a repository at destination folder by copying complete source tree 
 *                   and keeping a unique copy of file leaves using checksum mechanism for naming them.This way we maintain a unique version of source 
 *                   files and have a description of changes made as manifest.txt file.
 */

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
       


        //This is the starting point of application
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

        /// <summary>
        ///List of leaves aka files along with their unique Artifact Id and other details 
        /// </summary>
        private static List<string> fileTreeData;

        public static List<string> FileTreeData
        {
            get { return fileTreeData; }
            set { fileTreeData = value; }
        }


        /// <summary>
        /// This function takes sourcePath and destinationPath as parameters and create a copy for complete
        /// tree structure of folder and files in source folder.
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        /// <returns></returns>
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
                            string artifactIDFile = CreateArtifactID(fileInfo.DirectoryName + "\\" + fileInfo.Name); // generating Artifact ID for each file present at source location to maintain versions
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

        /// <summary>
        /// This function uses rotating checksum mechanism to generate a unique checksum followed by Artifact Id for a file. A cycle of 4 is used with numbers 1,3,11,17
        /// </summary>
        /// <param name="path"></param>
        /// <returns> returns unique Artifact ID for file specified at input location</returns>
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


        /// <summary>
        /// This function creates and maintain a manifest file for the command issued by user along with details such as timeStamp , source , destination etc.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
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
