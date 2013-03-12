using System;
using Machine.Specifications;

namespace GhostBridge
{
    [Behaviors]
    public class a_failing_chutzpah_test
    {
        protected static Exception err;
        protected static ChutzpahTestRun ChutzpahTestRun;

        It should_not_error = () => err.ShouldBeNull();

        It should_not_fail = () => ChutzpahTestRun.StdOut.ShouldContain("[FAIL]");

        It should_compile = () => ChutzpahTestRun.StdOut.ShouldNotContain("ERROR OCCURRED");

        It should_run_ok = () => ChutzpahTestRun.StdErr.ShouldBeEmpty();
    }
}