using System;
using System.Collections.Generic;
using System.IO;
using Chutzpah;
using Chutzpah.Models;

namespace GhostBridge.NUnit
{
    public class ChutzpahRunnerCallback : RunnerCallback
    {
        readonly bool teamCityMode;
        readonly string testSuiteName;
        public List<string> Errors { get; private set; }
        public List<string> FailedTests { get; private set; }
        public List<string> PassedTests { get; private set; }
        public ChutzpahRunnerCallback(bool teamCityMode, string testFile)
        {
            Errors = new List<string>();
            FailedTests = new List<string>();
            PassedTests = new List<string>();
            this.teamCityMode = teamCityMode;
            testSuiteName = Path.GetFileName(testFile);
        }


        protected void WriteLine(string message,params object[] parms)
        {
            if (teamCityMode)
                message = "##teamcity[" + message + "]";
            Console.WriteLine(message, parms);
        }



        public override void TestSuiteFinished(TestCaseSummary summary)
        {
            base.TestSuiteFinished(summary);
            WriteLine("testSuiteFinished name='{0}'", Escape(testSuiteName));
        }

        public override void TestSuiteStarted()
        {
            WriteLine("testSuiteStarted name='{0}'", Escape(testSuiteName));
        }

        protected override void TestFailed(TestCase testCase)
        {
            var output = string.Format("testFailed name='{0}' details='{1}'", Escape(testCase.GetDisplayName()), Escape(GetTestFailureMessage(testCase)));
            FailedTests.Add(output);
            WriteLine(output);

            WriteOutput(testCase.GetDisplayName(), GetTestFailureMessage(testCase));
        }

        protected override void TestComplete(TestCase testCase)
        {
            WriteFinished(testCase.GetDisplayName(), testCase.TimeTaken);
        }

        protected override void TestPassed(TestCase testCase)
        {
            PassedTests.Add(testCase.GetDisplayName());
            WriteOutput(testCase.GetDisplayName(), "Passed");
        }

        public override void TestStarted(TestCase testCase)
        {
            WriteLine("testStarted name='{0}'", Escape(testCase.GetDisplayName()));
        }

        public override void ExceptionThrown(Exception exception, string fileName)
        {
            var output = fileName + "\r\n" + exception;
            if(!teamCityMode)
                Console.WriteLine(output);
            Errors.Add(output);
        }

        public override void FileError(TestError error)
        {
            var output = string.Concat("Error in test file : ", error.InputTestFile, "\r\n", error.Message, "\r\n", error.Stack);
            Errors.Add(output);
        }

        // Helpers

        string Escape(string value)
        {
            if (!teamCityMode)
                return value;

            return value.Replace("|", "||")
                        .Replace("'", "|'")
                        .Replace("\r", "|r")
                        .Replace("\n", "|n")
                        .Replace("]", "|]");
        }

        void WriteFinished(string name, double duration)
        {
            WriteLine("testFinished name='{0}' duration='{1}'", Escape(name), duration);
        }

        void WriteOutput(string name, string output)
        {
            if (output != null)
                WriteLine("testStdOut name='{0}' out='{1}'", Escape(name), Escape(output));
        }
    }
}