using System.IO;
using GhostBridge.NUnit;
using NUnit.Framework;

namespace GhostBridge.Specs
{
    public class NUnitRunnerSpecs : with_chutzpah_test_runner
    {
        [TestFixture]
        public class with_a_failing_test : with_chutzpah_test_runner
        {
            [SetUp]
            public void Init()
            {
                Init("jasmine-specs/specs/failing-test.spec");
            }
            [Test]
            public void AllTestsPass()
            {
                Execute();
                Assert.IsNull(err);
                Assert.IsTrue(runner.Success);
                Assert.IsEmpty(runner.Errors);
                Assert.IsEmpty(runner.PassedTests);
                Assert.IsNotEmpty(runner.FailedTests);
            }
        }

        [TestFixture]
        public class with_a_passing_test : with_chutzpah_test_runner
        {
            [SetUp]
            public void Init()
            {
                Init("jasmine-specs/specs/passing-test.spec");
            }
            [Test]
            public void AllTestsPass()
            {
                Execute();
                Assert.IsNull(err);
                Assert.IsTrue(runner.Success);
                Assert.IsEmpty(runner.Errors);
                Assert.IsNotEmpty(runner.PassedTests);
                Assert.IsEmpty(runner.FailedTests);
            }
        }

        [TestFixture]
        public class with_rubbish_setup : with_chutzpah_test_runner
        {
            [Test]
            public void ItThrowsFileNotFound()
            {
                Execute();
                Assert.IsInstanceOf<FileNotFoundException>(err);
            }
        

        }

    }
}