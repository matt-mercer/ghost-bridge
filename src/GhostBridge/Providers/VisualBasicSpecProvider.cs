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
            spec.Members.Add(new CodeSnippetTypeMember("\t\tFriend context As Establish = (Sub() Init(\"" + filePath + "\",\"" + config.ChutzpahLocation + "\"))"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend [of] As Because = (Sub() Execute())"));
            //spec.Members.Add(new CodeSnippetTypeMember("\tBehaves_like<a_passing_chutzpah_test> success;"));
            //ncrunch not playing with behaviours very well right now

            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_not_error As It = (Sub() err.ShouldBeNull())"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_not_fail As It = (Sub() stdOut.ShouldNotContain(\"[FAIL]\"))"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_compile As It = (Sub() stdOut.ShouldNotContain(\"ERROR OCCURRED\"))"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tFriend should_run_ok As It = (Sub() stdErr.ShouldBeEmpty())"));
        }

    }
}