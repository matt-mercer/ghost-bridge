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

            TestRunner = "chutzpah.console.exe";
            Timeout = 1000;
        }

        public FileInfo File { get; private set; }

        public string TestRunner { get; set; }

        public int Timeout { get; set; }

        public string PhantomOptions { get; set; }

        public string Options { get; set; }

    }
}