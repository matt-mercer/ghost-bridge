using System;
using Machine.Specifications;

namespace GhostBridge
{
    public class with_chutzpah_test_runner
    {
        protected static string testFile;
        protected static Exception err;
        protected static ChutzpahTestRun runner;


        Cleanup tidy = () => {
            testFile = null;
            err = null;
            runner = null;
        };

        protected static void Init(string file)
        {
            testFile = file;
        }
    

        protected static void Execute()
        {
            err = Catch.Exception(() =>
                {
                    runner = new ChutzpahTestRun(new ChutzpahTestSetup(testFile));
                    runner.Run();
                });
            
            if (err == null)
                return;

            Console.WriteLine(err);
        }

    }
}