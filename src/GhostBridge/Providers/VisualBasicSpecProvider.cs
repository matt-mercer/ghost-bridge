using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.VisualBasic;

namespace GhostBridge.Providers
{
    public class VisualBasicSpecProvider : SpecCodeProvider
    {
        public VisualBasicSpecProvider(ProviderConfig config)
            : base(config)
        {
        }

        protected override string UsingNamespace(string ns)
        {
            return string.Concat("Imports ", ns);
        }

        protected override CodeDomProvider CreateProvider()
        {
            return new VBCodeProvider();
        }

        protected override void AddMembers(CodeTypeDeclaration spec, string filePath, string baseDirectory)
        {
            spec.Members.Add(new CodeSnippetTypeMember("\t\tFriend context As Establish = (Sub() Init(\"" + filePath + "\"))"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend [of] As Because = (Sub() Execute())"));
            //spec.Members.Add(new CodeSnippetTypeMember("\tBehaves_like<a_passing_chutzpah_test> success;"));
            //ncrunch not playing with behaviours very well right now

            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_not_error As It = (Sub() err.ShouldBeNull())\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_succeed As It = (Sub() runner.Success.ShouldBeTrue())\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_have_no_test_errors As It = (Sub() runner.Errors.ShouldBeEmpty())\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_have_some_passed_tests As It = (Sub() runner.PassedTests.ShouldNotBeEmpty())\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_have_no_failed_tests As It = (Sub() runner.FailedTests.ShouldBeEmpty())\r\n"));

        }

    }
}