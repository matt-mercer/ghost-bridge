using System.IO;
using GhostBridge.MSpec;
using Machine.Specifications;

namespace GhostBridge.Specs
{
    public class MSpecRunnerSpecs
    {
        [Subject(typeof(ChutzpahTestRun))]
        public class with_a_failing_test : with_chutzpah_test_runner
        {
            Establish context = () => Init("jasmine-specs/specs/failing-test.spec.js");

            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_fail = () => runner.Success.ShouldBeFalse();

            It should_have_no_test_errors = () => runner.Errors.ShouldBeEmpty();

            It should_have_no_passed_tests = () => runner.PassedTests.ShouldBeEmpty();

            It should_have_some_failed_tests = () => runner.FailedTests.ShouldNotBeEmpty();
        }

        [Subject(typeof(ChutzpahTestRun))]
        public class with_a_passing_test : with_chutzpah_test_runner
        {
            Establish context = () => Init("jasmine-specs/specs/passing-test.spec.js");

            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_succeed = () => runner.Success.ShouldBeTrue();

            It should_have_no_test_errors = () => runner.Errors.ShouldBeEmpty();

            It should_have_some_passed_tests = () => runner.PassedTests.ShouldNotBeEmpty();

            It should_have_no_failed_tests = () => runner.FailedTests.ShouldBeEmpty();
        }

        [Subject(typeof(ChutzpahTestRun))]
        public class with_rubbish_setup : with_chutzpah_test_runner
        {
            Because of = () => Execute();

            It should_throw_a_file_not_found = () => err.ShouldBeOfType<FileNotFoundException>();
        }
 
    }
}