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
using NUnit.Framework;

namespace GhostBridge.NUnit.Providers
{
    public abstract class NUnitCodeProvider
    {

        protected ProviderConfig config;

        protected NUnitCodeProvider(ProviderConfig config)
        {
            this.config = config;
        }

        public string CreateSpecs(out int specCount)
        {
            var files = FindSpecFiles().ToArray();
            specCount = files.Length;
            if (specCount < 1)
                return string.Empty;
            
            var compile = new CodeCompileUnit();

            var globalns = new CodeNamespace();
            globalns.Imports.Add(new CodeNamespaceImport(typeof(TestFixtureAttribute).Namespace));
            globalns.Imports.Add(new CodeNamespaceImport(config.MyNamespace));
            compile.Namespaces.Add(globalns);
            var specNamespace = new CodeNamespace(config.TargetNamespace + ".ChutzpahSpecs");
            compile.Namespaces.Add(specNamespace);

            foreach (var fileInfo in files)
            {
                AddSpec(specNamespace, fileInfo);
            }
            //specNamespace.Types.AddRange(files.Select(CreateSpec).ToArray());

            var provider = CreateProvider();
            var options = new CodeGeneratorOptions { BracingStyle = "C", BlankLinesBetweenMembers = false };
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                provider.GenerateCodeFromCompileUnit(compile, writer, options);
                writer.Flush();
                writer.Close();
            }
            var result = stringBuilder.ToString();
            return result;


        }

        void AddSpec(CodeNamespace ns,FileInfo file)
        {
            var fullPath = file.FullName;
            LogMessage("processing : " + file.FullName);
            var className = CreateUsefulName(file);

            var spec = new CodeTypeDeclaration(className)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
            };
            ns.Types.Add(spec);
            spec.CustomAttributes.Add(new CodeAttributeDeclaration(typeof (TestFixtureAttribute).Name.Replace("Attribute", "")));
            spec.BaseTypes.Add(new CodeTypeReference(typeof(with_chutzpah_test_runner).Name));

            AddMembers(spec, fullPath, config.BaseDirectory);
        }

        protected abstract void AddMembers(CodeTypeDeclaration spec, string filePath, string baseDirectory);

        protected abstract string UsingNamespace(string ns);

        protected abstract CodeDomProvider CreateProvider();


        readonly Regex name_regex = new Regex("[^A-Za-z0-9]");

        readonly List<string> usedNames = new List<string>();

        protected string CreateUsefulName(FileInfo file)
        {
            var pathAndName = file.DirectoryName.Replace(config.BaseDirectory, "") + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file.FullName);
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

        protected  IEnumerable<FileInfo> FindSpecFiles()
        {

            var baseDirectory = (config.BaseDirectory ?? "").Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Trim();
            if (!Path.IsPathRooted(baseDirectory))
            {
                baseDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), baseDirectory);
            }

            LogMessage("BaseDirectory :: " + baseDirectory);

            if (!Directory.Exists(baseDirectory))
                throw new DirectoryNotFoundException("Directory not found : " + baseDirectory);

            var pattern = (config.Pattern ?? "*.*").Trim();
            var extensions = (config.Extensions ?? new[] { ".js", ".cofee", ".ts", ".html", ".htm" }).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToArray();

            var directory = new DirectoryInfo(baseDirectory);
            config.BaseDirectory = directory.FullName;
            return directory.GetFiles(pattern, SearchOption.AllDirectories).Where(f => extensions.Contains(f.Extension.ToLower()));

        }

        protected virtual void LogMessage(string message, MessageImportance importance = MessageImportance.High)
        {
            try
            {
                config.Log.LogMessage(message,importance);
            }
            catch (Exception err)
            {
            }
        }

    }
}