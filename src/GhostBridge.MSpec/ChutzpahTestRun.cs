using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using Chutzpah;
using Chutzpah.Models;

namespace GhostBridge.MSpec
{
    public class ChutzpahTestRun
    {
        static string phantomLocationCache;
        static bool phantomExists;
        readonly Mutex mutex;
        readonly string assemblyDir;
        readonly ChutzpahRunnerCallback testRunnerCallback;
        readonly ChutzpahTestSetup setup;
        public bool Success { get; set; }
        public List<string> Errors { get { return testRunnerCallback.Errors; } }
        public List<string> PassedTests { get { return testRunnerCallback.PassedTests; } }
        public List<string> FailedTests { get { return testRunnerCallback.FailedTests; } }
        public bool Timeout { get; set; }
        public int FailedCount { get; set; }



        public ChutzpahTestRun(ChutzpahTestSetup setup)
        {
            if (setup == null)
                throw new ArgumentNullException("setup");
            this.setup = setup;
            assemblyDir = Path.GetDirectoryName(typeof(TestRunner).Assembly.Location);
            mutex = new Mutex(false,"pc-" + assemblyDir.GetHashCode());
            testRunnerCallback = new ChutzpahRunnerCallback(setup.TeamCity,setup.File.FullName);
        }




        internal TestingMode GetTestingMode(FileInfo file)
        {
            switch (file.Extension.ToLower())
            {
                case ".js":
                    return TestingMode.JavaScript;
                case ".ts":
                    return TestingMode.TypeScript;
                case ".coffee":
                    return TestingMode.CoffeeScript;
                case "html":
                case "htm":
                    return TestingMode.HTML;
                default:
                    return TestingMode.All;
            }
        }

        string EnsurePhantom()
        {
            if (phantomExists)
                return phantomLocationCache;

            mutex.WaitOne();
            try
            {
                if (phantomExists)
                    return phantomLocationCache;

                ;
                var targetLocation = Path.Combine(assemblyDir, "phantomjs.exe");

                if (File.Exists(targetLocation))
                    return phantomLocationCache = targetLocation;
                
                try
                {
                    File.WriteAllBytes(targetLocation, Resources.phantomjs);
                }
                catch (IOException err)
                {
                    if (!err.Message.Contains("it is being used by another process"))
                        throw;
                }
                
                if (Directory.Exists(Path.Combine(assemblyDir, "JSRunners")))
                    return phantomLocationCache = targetLocation;

                using (var stream = new MemoryStream(Resources.runners))
                {
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                        archive.ExtractToDirectory(assemblyDir);
                }

                phantomExists = true;


                return phantomLocationCache = targetLocation;
            }
            finally
            {
                mutex.ReleaseMutex();
            }

        }

        string FindBrowser()
        {
            if (!string.IsNullOrEmpty(setup.HeadlessBrowser))
                return setup.HeadlessBrowser;

            if (phantomLocationCache != null)
                return phantomLocationCache;

            return phantomLocationCache = new[]
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly(),
                Assembly.GetEntryAssembly(),
                typeof (TestRunner).Assembly
            }
            .Where(a => a != null).Select(a => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(a.Location), "phantomjs.exe")))
            .FirstOrDefault(File.Exists);
        }

        public void Run()
        {
            var phantomLocation = FindBrowser() ?? EnsurePhantom();

            var testRunner = TestRunner.Create(setup.Debug);
            if (!string.IsNullOrEmpty(phantomLocation))
                TestRunner.HeadlessBrowserName = phantomLocation;
          
            var testOptions = new TestOptions
            {
                OpenInBrowser = setup.OpenInBrowser,
                TestFileTimeoutMilliseconds = setup.Timeout,
                MaxDegreeOfParallelism = setup.Parallelism,
                TestingMode = GetTestingMode(setup.File)
            };

            var testResultsSummary = testRunner.RunTests(setup.File.FullName, testOptions, testRunnerCallback);

            FailedCount = testResultsSummary.FailedCount;

            Success = FailedCount == 0;
        }

        
    }
}