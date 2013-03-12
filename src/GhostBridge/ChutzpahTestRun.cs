using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GhostBridge
{
    public class ChutzpahTestRun
    {
        readonly ChutzpahTestSetup setup;
        public string StdOut { get; set; }
        public string StdErr { get; set; }
        public bool Timeout { get; set; }
        public ChutzpahTestRun(ChutzpahTestSetup setup)
        {
            if (setup == null)
                throw new ArgumentNullException("setup");
            this.setup = setup;
        }

        public string Run()
        {
            var chutzpahExe = (setup.TestRunner ?? "").Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim();
            if (!Path.IsPathRooted(chutzpahExe))
                chutzpahExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, chutzpahExe);

            var runner = new FileInfo(chutzpahExe);
            if(!runner.Exists)
                throw new FileNotFoundException("Runner not found",runner.FullName);

            var arguments = string.Join(" ", new[]
                {
                    "\"" + setup.File.FullName + "\"",
                    setup.Options,
                    "/timeoutMilliseconds" +setup.Timeout,
                    "/vsoutput"
                }.Select(s => (s ?? "").Trim()).Where(s => s != "").ToArray());

            var process = new Process
                {
                    StartInfo = new ProcessStartInfo(runner.FullName, arguments)
                        {
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }

                };
            process.Start();
            process.WaitForExit(setup.Timeout);
            StdOut = process.StandardOutput.ReadToEnd();
            StdErr = process.StandardError.ReadToEnd();

            return string.Empty;
        }


    }
}