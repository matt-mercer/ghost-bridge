using System;
using Machine.Specifications;

namespace GhostBridge
{
    [Behaviors]
    public class a_failing_chutzpah_test
    {
        protected static Exception err;
        protected static ChutzpahTestRun runner;

        It should_fail = () => runner.Success.ShouldBeFalse();

        It should_have_no_test_errors = () => runner.Errors.ShouldBeEmpty();

        It should_have_no_passed_tests = () => runner.PassedTests.ShouldBeEmpty();

        It should_have_some_failed_tests = () => runner.FailedTests.ShouldNotBeEmpty();
    }
}