using System;
using NUnit.Framework;

namespace GhostBridge.NUnit
{
    public class with_chutzpah_test_runner
    {
        protected static string testFile;
        protected static Exception err;
        protected static ChutzpahTestRun runner;


        [TearDown]
        public void tidy()
        {
            testFile = null;
            err = null;
            runner = null;
        }

        protected static void Init(string file)
        {
            testFile = file;
        }
    

        protected static void Execute()
        {
            try
            {
                runner = new ChutzpahTestRun(new ChutzpahTestSetup(testFile));
                runner.Run();
            }
            catch (Exception error)
            {
                err = error;
                Console.WriteLine(err);
            }
        }

    }
}