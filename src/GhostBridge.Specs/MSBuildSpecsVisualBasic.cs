using System;
using System.IO;
using System.Text.RegularExpressions;
using GhostBridge.MSpec;
using Machine.Specifications;

namespace GhostBridge.Specs
{
    public class MSBuildSpecsVisualBasic
    {

        [Subject(typeof(MSBuildTask))]
        public class with_two_nested_specs : with_ghost_bridge_vb
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
        public class when_creating_a_multiple_tests_setting_the_proj_dir : with_ghost_bridge_vb
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
        public class when_creating_a_single_test_setting_the_namespace : with_ghost_bridge_vb
        {

            Establish context = () => builder.Namespace = "Bob";

            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_set_the_namespace = () => output.ShouldContain(Namespace("Bob.ChutzpahSpecs"));

            It should_output_something = () => output.ShouldNotBeEmpty();

            It have_a_using_for_mspec = () => output.ShouldContain(Using("Machine.Specifications"));
            
            It have_a_using_for_gb = () => output.ShouldContain(Using(gb_ns));

            It should_declare_the_class = () => output.ShouldMatch(SpecNamePattern("passing_test_spec"));

            //It have_behaves = () => output.ShouldContain(BehavesLike());

            It should_have_an_execute = () => output.ShouldContain(ExecutePattern());

            It should_init_the_test = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\specs\passing-test.spec.js"));

            It should_set_the_spec_count = () => builder.SpecCount.ToString().ShouldEqual("1");
        }
        
        [Subject(typeof(MSBuildTask))]
        public class when_creating_a_single_test : with_ghost_bridge_vb
        {
            Because of = () => Execute();

            It should_not_error = () => err.ShouldBeNull();

            It should_output_something = () => output.ShouldNotBeEmpty();

            It should_set_the_namespace = () => output.ShouldContain(Namespace(gb_ns + ".ChutzpahSpecs"));

            It have_a_using_for_mspec = () => output.ShouldContain(Using("Machine.Specifications"));

            It have_a_using_for_gb = () => output.ShouldContain(Using(gb_ns));

            It should_declare_the_class = () => output.ShouldMatch(SpecNamePattern("passing_test_spec"));

            //It have_behaves = () => output.ShouldContain(BehavesLike());

            It should_have_an_execute = () => output.ShouldContain(ExecutePattern());

            It should_have_the_asserts_1 = () => output.ShouldContain("\r\n\t\tFriend should_not_error As It = (Sub() err.ShouldBeNull())\r\n");

            It should_have_the_asserts_2 = () => output.ShouldContain("\r\n\t\tFriend should_succeed As It = (Sub() runner.Success.ShouldBeTrue())\r\n");

            It should_have_the_asserts_3 = () => output.ShouldContain("\r\n\t\tFriend should_have_no_test_errors As It = (Sub() runner.Errors.ShouldBeEmpty())\r\n");

            It should_have_the_asserts_4 = () => output.ShouldContain("\r\n\t\tFriend should_have_some_passed_tests As It = (Sub() runner.PassedTests.ShouldNotBeEmpty())\r\n");

            It should_have_the_asserts_5 = () => output.ShouldContain("\r\n\t\tFriend should_have_no_failed_tests As It = (Sub() runner.FailedTests.ShouldBeEmpty())\r\n");

            It should_init_the_test = () => output.ShouldMatch(SpecInitPattern(@"jasmine-specs\specs\passing-test.spec.js"));

            It should_set_the_spec_count = () => builder.SpecCount.ToString().ShouldEqual("1");
        }

        public class with_ghost_bridge_vb
        {

            protected static MSBuildTask builder;
            protected static string output;
            protected static Exception err;
            protected static string gb_ns;

            Establish context = () =>
                {
                    gb_ns = typeof (MSBuildTask).Namespace;
                    builder = new MSBuildTask
                        {
                            Pattern = "passing-test.spec.js",
                            Language = "VB"
                        };
                    BaseDirectory(@"..\..\jasmine-specs\specs");
                };

            protected static string Using(string ns)
            {
                return "Imports " + ns;
            }

            protected static string Namespace(string ns)
            {
                return "\r\nNamespace " + ns + "\r\n";
            }

            protected static string BehavesLike()
            {
                return "\r\n\t\tBehaves_like<a_passing_chutzpah_test> success";
            }

            protected static string ExecutePattern()
            {
                return "\r\n\t\tFriend [of] As Because = (Sub() Execute())";
            }

            protected static string SpecNamePattern(string filebit)
            {
                return @"\r\n\s*Public NotInheritable Class with_" + filebit + @"_([A-Za-z0-9]*)\s*Inherits with_chutzpah_test_runner";
            }
        

            protected static string SpecInitPattern(string filebit)
            {
                return @"\r\n\s*Friend context As Establish = \(Sub\(\) Init\("".*" + Regex.Escape(filebit) + @"""\)\)";
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