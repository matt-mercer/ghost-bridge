using System;
using System.IO;

namespace GhostBridge
{
    public class ChutzpahTestSetup
    {

        public ChutzpahTestSetup(string filePath)
        {
            filePath = (filePath ?? string.Empty).Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim();
            if (!Path.IsPathRooted(filePath))
                filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);


            File = new FileInfo(filePath);
            if (!File.Exists)
                throw new FileNotFoundException("File not found", filePath);

            TeamCity = Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null;

            Timeout = 5000;
            Parallelism = 1;
        }

        public FileInfo File { get; private set; }

        public bool OpenInBrowser { get; set; }

        public string HeadlessBrowser { get; set; }

        public int Parallelism { get; set; }

        public bool TeamCity { get; set; }

        public int Timeout { get; set; }

        public bool Debug { get; set; }


    }
}