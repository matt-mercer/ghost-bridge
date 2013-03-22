using System;
using System.IO;
using System.Text.RegularExpressions;
using GhostBridge.NUnit;
using Machine.Specifications;

namespace GhostBridge.Specs
{
    public class MSBuildNUnitCSharp
    {

        [Subject(typeof(MSBuildTask))]
        public class with_two_nested_specs : with_ghost_bridge_cs
        {

            Establish context = () => BaseDirectory(@"..\..\jasmine-specs\nested-specs");

            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_set_the_namespace = () => output.ShouldContain(Namespace(gb_ns + ".ChutzpahSpecs"));


            It should_declare_the_class_1 = () => output.ShouldMatch(SpecNamePattern("sub1_passing_test_spec"));

            It should_declare_the_class_2 = () => output.ShouldMatch(SpecNamePattern("sub2_passing_test_spec"));

            It should_init_the_tests_1 = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\nested-specs\sub1\passing-test.spec.js"));

            It should_init_the_tests_2 = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\nested-specs\sub2\passing-test.spec.js"));

            It should_set_the_spec_count = () => builder.SpecCount.ToString().ShouldEqual("2");
        }


        [Subject(typeof(MSBuildTask))]
        public class when_creating_a_multiple_tests_setting_the_proj_dir : with_ghost_bridge_cs
        {

            Establish context = () =>
            {
                builder.ProjectDir = "c:\\blah\\fakeProject";
                builder.Pattern = "*.*";
            };

            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_set_the_namespace = () => output.ShouldContain(Namespace("fakeProject.ChutzpahSpecs"));

            It should_declare_the_class_1 = () => output.ShouldMatch(SpecNamePattern("passing_test_spec"));

            It should_declare_the_class_2 = () => output.ShouldMatch(SpecNamePattern("failing_test_spec"));

            It should_init_the_test_1 = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\specs\passing-test.spec.js"));

            It should_init_the_test_2 = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\specs\failing-test.spec.js"));

            It should_set_the_spec_count = () => builder.SpecCount.ToString().ShouldEqual("2");
        }


        [Subject(typeof(MSBuildTask))]
        public class when_creating_a_single_test_setting_the_namespace : with_ghost_bridge_cs
        {

            Establish context = () => builder.Namespace = "Bob";

            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_set_the_namespace = () => output.ShouldContain(Namespace("Bob.ChutzpahSpecs"));

            It should_output_something = () => output.ShouldNotBeEmpty();

            It have_a_using_for_nunit = () => output.ShouldContain(Using("NUnit.Framework"));

            It have_a_using_for_gb = () => output.ShouldContain(Using(gb_ns));

            It should_declare_the_class = () => output.ShouldMatch(SpecNamePattern("passing_test_spec"));

            //It have_behaves = () => output.ShouldContain(BehavesLike());

            It should_have_an_execute = () => output.ShouldContain(ExecutePattern());

            It should_init_the_test = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\specs\passing-test.spec.js"));

            It should_set_the_spec_count = () => builder.SpecCount.ToString().ShouldEqual("1");
        }

        [Subject(typeof(MSBuildTask))]
        public class when_creating_a_single_test : with_ghost_bridge_cs
        {
            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_output_something = () => output.ShouldNotBeEmpty();

            It should_set_the_namespace = () => output.ShouldContain(Namespace(gb_ns + ".ChutzpahSpecs"));

            It have_a_using_for_mspec = () => output.ShouldContain(Using("NUnit.Framework"));

            It have_a_using_for_gb = () => output.ShouldContain(Using(gb_ns));

            It should_declare_the_class = () => output.ShouldMatch(SpecNamePattern("passing_test_spec"));

            //It have_behaves = () => output.ShouldContain(BehavesLike());

            It should_have_the_asserts_1 = () => output.ShouldContain("\r\n\t\t[Test]\r\n\t\tpublic void AllTestsPass()");

            It should_have_the_asserts_2 = () => output.ShouldContain("\r\n\t\t\tExecute();\r\n");

            It should_have_the_asserts_3 = () => output.ShouldContain("\r\n\t\t\tAssert.IsNull(err);\r\n");

            It should_have_the_asserts_4 = () => output.ShouldContain("\r\n\t\t\tAssert.IsTrue(runner.Success);\r\n");

            It should_have_the_asserts_5 = () => output.ShouldContain("\r\n\t\t\tAssert.IsEmpty(runner.Errors);\r\n");

            It should_have_the_asserts_6 = () => output.ShouldContain("\r\n\t\t\tAssert.IsNotEmpty(runner.PassedTests);\r\n");

            It should_have_the_asserts_7 = () => output.ShouldContain("\r\n\t\t\tAssert.IsEmpty(runner.FailedTests);\r\n");

            It should_init_the_test = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\specs\passing-test.spec.js"));

            It should_set_the_spec_count = () => builder.SpecCount.ToString().ShouldEqual("1");
        }

        public class with_ghost_bridge_cs
        {

            protected static MSBuildTask builder;
            protected static string output;
            protected static Exception err;
            protected static string gb_ns;

            Establish context = () =>
                {
                    gb_ns = typeof(MSBuildTask).Namespace;
                    builder = new MSBuildTask {
                        Pattern = "passing-test.spec.js",
                        Language = "C#"
                    };
                    BaseDirectory(@"..\..\jasmine-specs\specs");
                };

            protected static string Namespace(string ns)
            {
                return "\r\nnamespace " + ns + "\r\n{";
            }

            protected static string Using(string ns)
            {
                return "using " + ns + ";";
            }

            protected static string ExecutePattern()
            {
                return "";
            }

            protected static string SpecNamePattern(string filebit)
            {
                return @"\[TestFixture\(\)\]\r\n\s*public sealed class with_" + filebit + "_([A-Za-z0-9]*) : with_chutzpah_test_runner";
            }

            protected static string SpecInitPattern(string filebit)
            {
                return @"\[SetUp\]\r\n\s*public void Init\(\)\r\n\s*{\r\n\s*Init\(@"".*" + Regex.Escape(filebit) + @"""\);";

            }

            protected static void BaseDirectory(string path)
            {
                if(builder==null)
                    return;

                if (!Path.IsPathRooted(path))
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                builder.BaseDirectory = path;
            }
            

            protected static void Execute()
            {
                err = Catch.Exception(()=>output = builder.CreateSpecs());
                Console.WriteLine(output);
            }

        }
    }
}