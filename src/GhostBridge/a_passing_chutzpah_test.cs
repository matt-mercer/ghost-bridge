using System;
using Machine.Specifications;

namespace GhostBridge
{
    [Behaviors]
    public class a_passing_chutzpah_test
    {
        protected static Exception err;
        protected static ChutzpahTestRun runner;

        It should_not_error = () => err.ShouldBeNull();

        It should_succeed = () => runner.Success.ShouldBeTrue();

        It should_have_no_test_errors = () => runner.Errors.ShouldBeEmpty();

        It should_have_some_passed_tests = () => runner.PassedTests.ShouldNotBeEmpty();

        It should_have_no_failed_tests = () => runner.FailedTests.ShouldBeEmpty();

    }
}