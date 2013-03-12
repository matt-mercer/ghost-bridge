using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace GhostBridge
{

    public class MSBuildTask : Task
    {
        [Required]
        public string ProjectDir { get; set; }

        [Required]
        public string BaseDirectory { get; set; }

        public string Namespace { get; set; }

        public string Pattern { get; set; }

        public string[] Extensions { get; set; }

        [Required]
        public string ChutzpahLocation { get; set; }

        [Output]
        public ITaskItem SpecCount { get; set; }

        readonly Regex name_regex = new Regex("[^A-Za-z0-9]");

        readonly List<string> usedNames = new List<string>();

        public override bool Execute()
        {
            SpecCount = new TaskItem("0");
            LogMessage("Project Dir :: " + ProjectDir);
            var outputfile = Path.Combine(ProjectDir, "ghost_bridge_specs.cs");
            LogMessage("Will write to : " + outputfile,MessageImportance.Low);
            if (File.Exists(outputfile))
            {
                LogMessage("Output file exists, deleting", MessageImportance.Low);
                File.Delete(outputfile);
            }
            var output = CreateSpecs();
            if (int.Parse(SpecCount.ItemSpec) > 0)
            {
                LogMessage("Writing specs :: \r\n" + output, MessageImportance.Low);
                File.WriteAllText(outputfile, output);
            }

            return true;
            
        }

        public string CreateSpecs()
        {
            SpecCount = new TaskItem("0");
            string baseDirectory;
            var files = FindSpecFiles(out baseDirectory).ToArray();
            SpecCount = new TaskItem(files.Length.ToString());
            if (files.Length == 0)
                return "";

            ChutzpahLocation = ChutzpahLocation ?? @"..\..\..\..\lib\chutzpah\chutzpah.console.exe";

            var thisns = GetType().Namespace;
            var ns = Namespace ?? (string.IsNullOrWhiteSpace(ProjectDir) ? thisns : (new DirectoryInfo(ProjectDir).Name));
            
            var compile = new CodeCompileUnit();
            var specNamespace = new CodeNamespace(ns + ".ChutzpahSpecs");
            compile.Namespaces.Add(specNamespace);
            specNamespace.Types.AddRange(files.Select(f=>CreateSpec(f,baseDirectory)).ToArray());

            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions {BracingStyle = "C"};
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using Machine.Specifications;");
            stringBuilder.AppendLine("using " + thisns + ";");
            using (var writer = new StringWriter(stringBuilder))
            {
                provider.GenerateCodeFromCompileUnit(compile, writer, options);
                writer.Flush();
                writer.Close();
            }
            var result = stringBuilder.ToString();
            LogMessage(result,MessageImportance.Normal);
            return result;

        }

        CodeTypeDeclaration CreateSpec(FileInfo file, string baseDirectory)
        {
            LogMessage("processing : " + file.FullName);
            var testname = CreateUsefulName(file, baseDirectory);
            var spec = new CodeTypeDeclaration(testname)
                {
                    IsClass = true, TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
                }; 
            
            spec.BaseTypes.Add(typeof (with_chutzpah_test_runner));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tEstablish context = () => { testFile = @\"" + file.FullName + "\"; chutzpahExe = @\"" + ChutzpahLocation + "\"; };"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tBecause of = () => Execute();"));
            //spec.Members.Add(new CodeSnippetTypeMember("\tBehaves_like<a_passing_chutzpah_test> success;"));
            //ncrunch not playing with behaviours very well right now

            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_not_error = () => err.ShouldBeNull();"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_not_fail = () => ChutzpahTestRun.StdOut.ShouldNotContain(\"[FAIL]\");"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_compile = () => ChutzpahTestRun.StdOut.ShouldNotContain(\"ERROR OCCURRED\");"));
            spec.Members.Add(new CodeSnippetTypeMember("\t\tIt should_run_ok = () => ChutzpahTestRun.StdErr.ShouldBeEmpty();"));
            

            return spec;



        }

        string CreateUsefulName(FileInfo file, string baseDirectory)
        {
            var pathAndName = file.DirectoryName.ToLower().Replace(baseDirectory,"") + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file.FullName);
            var name = name_regex.Replace(pathAndName, "_").Trim('_').ToLower();
            while (name.Contains("__"))
                name = name.Replace("__", "_");
            name = "with_" + name + "_" + Guid.NewGuid().ToString("N");
            var suffix = "";
            var index = 0;
            while (usedNames.Contains(name + suffix))
            {
                index++;
                suffix = "_" + index;
            }

            name = name + suffix;
            usedNames.Add(name);
            
            return name;
        }


        IEnumerable<FileInfo> FindSpecFiles(out string baseDirectory)
        {
            
            baseDirectory = (BaseDirectory ?? "").Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim();
            if (!Path.IsPathRooted(baseDirectory))
            {
                baseDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), baseDirectory);
            }

            LogMessage("BaseDirectory :: " + baseDirectory);

            if (!Directory.Exists(baseDirectory))
                throw new DirectoryNotFoundException("Directory not found : " + baseDirectory);

            var pattern = (Pattern ?? "*.*").Trim();
            var extensions = (Extensions ?? new[] { ".js", ".cofee", ".ts", ".html", ".htm" }).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();

            var directory = new DirectoryInfo(baseDirectory);
            baseDirectory = directory.FullName.ToLower();
            return directory.GetFiles(pattern, SearchOption.AllDirectories).Where(f => extensions.Contains(f.Extension.ToLower()));

        }

        void LogMessage(string message, MessageImportance importance = MessageImportance.High)
        {
            try
            {
                Log.LogMessage(importance, message);
            }
            catch (Exception)
            {
            }
            
        }
    }
}