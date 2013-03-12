using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace GhostBridge.Providers
{
    public class CSharpSpecProvider : SpecCodeProvider
    {
        public CSharpSpecProvider(ProviderConfig config) : base(config)
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
            spec.Members.Add(new CodeSnippetTypeMember("\t\tEstablish context = () => Init(@\"" + filePath + "\",@\"" + config.ChutzpahLocation + "\");"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tBecause of = () => Execute();"));
            //spec.Members.Add(new CodeSnippetTypeMember("\tBehaves_like<a_passing_chutzpah_test> success;"));
            //ncrunch not playing with behaviours very well right now

            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_not_error = () => err.ShouldBeNull();"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_not_fail = () => stdOut.ShouldNotContain(\"[FAIL]\");"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_compile = () => stdOut.ShouldNotContain(\"ERROR OCCURRED\");"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_run_ok = () => stdErr.ShouldBeEmpty();"));

        }
         
    }
}