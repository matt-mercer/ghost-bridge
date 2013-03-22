using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.VisualBasic;
using NUnit.Framework;

namespace GhostBridge.NUnit.Providers
{
    public class VisualBasicNUnitProvider : NUnitCodeProvider
    {
        public VisualBasicNUnitProvider(ProviderConfig config)
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

            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t<" + typeof(SetUpAttribute).Name.Replace("Attribute", "") + ">"));

            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tpublic Sub Init()"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tInit(@\"" + filePath + "\")"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tEnd Sub"));

            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t<" + typeof(TestAttribute).Name.Replace("Attribute", "") + ">"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tpublic Sub AllTestsPass()"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tExecute();"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tAssert.IsNull(err);"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tAssert.IsTrue(runner.Success);"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tAssert.IsEmpty(runner.Errors);"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tAssert.IsNotEmpty(runner.PassedTests);"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\t\tAssert.IsEmpty(runner.FailedTests);"));
            spec.Members.Add(new CodeSnippetTypeMember("\r\n\t\tEnd Sub"));


        }

    }
}