using System;
using Machine.Specifications;

namespace GhostBridge
{
    public class with_chutzpah_test_runner
    {
        protected static string testFile;
        protected static string chutzpahExe;
        protected static Exception err;
        protected static ChutzpahTestRun ChutzpahTestRun;

        Establish context = () =>
            {
                chutzpahExe = @"..\..\..\..\lib\chutzpah\chutzpah.console.exe";
            };

        Cleanup tidy = () =>
            {
                testFile = null;
                chutzpahExe = null;
                err = null;
                ChutzpahTestRun = null;
            };

        protected static void Init(string test, string exe)
        {
            testFile = test;
            chutzpahExe = exe;
        }
    

        protected static void Execute()
        {
            err = Catch.Exception(() =>
                {
                    Console.WriteLine("Chutzpah running specs in : " + testFile);
                    ChutzpahTestRun = new ChutzpahTestRun(new ChutzpahTestSetup(testFile) { TestRunner = chutzpahExe });
                    ChutzpahTestRun.Run();
                });
            if (err != null)
            {
                Console.WriteLine(err);
                return;
            }

            Console.WriteLine(ChutzpahTestRun.StdErr);
            Console.WriteLine(ChutzpahTestRun.StdOut);


        }

    }
}