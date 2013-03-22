using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace GhostBridge.MSpec.Providers
{
    public class CSharpMSpecProvider : MSpecCodeProvider
    {
        public CSharpMSpecProvider(ProviderConfig config) : base(config)
        {
        }

        protected override string UsingNamespace(string ns)
        {
            return string.Concat("using ", ns, ";");
        }

        protected override CodeDomProvider CreateProvider()
        {
            return new CSharpCodeProvider();
        }

        protected override void AddMembers(CodeTypeDeclaration spec, string filePath, string baseDirectory)
        {
            spec.Members.Add(new CodeSnippetTypeMember("\t\tEstablish context = () => Init(@\"" + filePath + "\");"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tBecause of = () => Execute();"));
            //spec.Members.Add(new CodeSnippetTypeMember("\tBehaves_like<a_passing_chutzpah_test> success;"));
            //ncrunch not playing with behaviours very well right now

            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tIt should_not_error = () => err.ShouldBeNull();\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tIt should_succeed = () => runner.Success.ShouldBeTrue();\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tIt should_have_no_test_errors = () => runner.Errors.ShouldBeEmpty();\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tIt should_have_some_passed_tests = () => runner.PassedTests.ShouldNotBeEmpty();\r\n"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tIt should_have_no_failed_tests = () => runner.FailedTests.ShouldBeEmpty();\r\n"));

       
        }
         
    }
}