using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string processPath = Environment.ProcessPath;
            string path = Path.GetDirectoryName(processPath);

            Console.WriteLine("Process executing in " + path);

            List<EndingPairs> pairs =
            [
                new EndingPairs("png","Images"),
                new EndingPairs("jpg","Images"),
                new EndingPairs("gif","Images"),
                new EndingPairs("jpeg","Images"),
                new EndingPairs("webp","Images"),
                new EndingPairs("PNG", "Images"),
                new EndingPairs("ico", "Images"),
                new EndingPairs("psd", "Images"),

                new EndingPairs("7z","Archieves"),
                new EndingPairs("zip","Archieves"),
                new EndingPairs("rar", "Archieves"),
                new EndingPairs("7zip", "Archieves"),

                new EndingPairs("mp4", "Videos"),
                new EndingPairs("mkv", "Videos"),

                new EndingPairs("pdf", "Documents"),

                new EndingPairs("mp3", "Music"),
                new EndingPairs("wav", "Music"),

                new EndingPairs("exe", "Applications"),
                new EndingPairs("", "Other"),
            ];

            List<string> tabuDirectories = pairs.Select(x => x.folder).ToList();

            MoveDirectories(path, tabuDirectories);
            MoveFiles(processPath, path, pairs);

            while (true)
            {
                
            }
        }

        private static void MoveFiles(string processPath, string path, List<EndingPairs> pairs)
        {
            string filePattern = "(.*\\\\)(.*)\\.(.*)$";

            string[] files = Directory.GetFiles(path);

            for (int i = 0; i < files.Length; i++)
            {
                // Skip the exe file that excutes this process
                if (files[i] == processPath || files[i].Equals(processPath)) continue;

                string fullFilePath = files[i];
                Match match = Regex.Match(fullFilePath, filePattern);
                string filePath = match.Groups[1].Value;
                string fileName = match.Groups[2].Value;
                string fileEnding = match.Groups[3].Value;

                EndingPairs pair = pairs.Find(x => x.ending.Equals(fileEnding));

                string newFilePath = GetNewPath(filePath, fileName, fileEnding, pair);

                try
                {
                    File.Move(fullFilePath, newFilePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }

                if (!(pair.folder == null || string.IsNullOrEmpty(pair.folder)))
                {
                    string dir = Path.Combine(filePath, pair.folder);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                }
            }
        }

        private static string GetNewPath(string path, string fileName, string ending, EndingPairs pair)
        {
            string dir = "";

            Console.Write("File " + fileName + " with ending " + ending);

            // If pair exists
            if (!(pair.folder == null || string.IsNullOrEmpty(pair.folder)))
            {
                Console.WriteLine(" fits the pair: " + pair.ending + " " + pair.folder);
                dir = Path.Combine(path, pair.folder);
            }
            // If pair doesnt exist move file to "Other" folder
            else
            {
                Console.WriteLine(" does not fit any pair and is moved to others");
                dir = Path.Combine(path, "Other");
            }

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string expectedPath = Path.Combine(dir, fileName + "." + ending);

            // If file with the expected path exists add a random number to the name and return that
            if (File.Exists(expectedPath))
            {
                return Path.Combine(dir, fileName + "_" + RandomNumberGenerator.GetInt32(1000).ToString() + "." + ending);
            }
            //Else return path normally
            else
            {
                return expectedPath;
            }
        }

        private static void MoveDirectories(string path, List<string> tabuDirectories)
        {
            string[] directories = Directory.GetDirectories(path);

            string directoryPathRegex = "(.*\\\\)(.*$)";

            for (int i = 0; i < directories.Length; i++)
            {
                try
                {
                    string oldPath = directories[i];

                    Match match = Regex.Match(oldPath, directoryPathRegex);

                    string folderName = match.Groups[2].Value;

                    if (folderName.Equals("Directories") || tabuDirectories.Contains(folderName)) continue;

                    string dirFolder = match.Groups[1].Value + "Directories\\";

                    if (!Directory.Exists(dirFolder)) Directory.CreateDirectory(dirFolder);

                    string newPath = dirFolder + folderName;

                    Console.WriteLine("Moved Directory from OldPath: " + oldPath + " to NewPath: " + newPath);

                    if (Directory.Exists(oldPath))
                    {
                        Directory.Move(oldPath, newPath);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        private struct EndingPairs
        {
            public string ending;
            public string folder;

            public EndingPairs(string ending, string folder)
            {
                this.ending = ending;
                this.folder = folder;
            }
        }
    }
}
