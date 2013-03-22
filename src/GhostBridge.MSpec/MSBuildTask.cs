using System;
using System.IO;
using GhostBridge.MSpec.Providers;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace GhostBridge.MSpec
{

    public class MSBuildTask : Task
    {
        [Required]
        public string ProjectDir { get; set; }

        [Required]
        public string BaseDirectory { get; set; }

        [Required]
        public string Language { get; set; }

        public string Namespace { get; set; }

        public string Pattern { get; set; }

        public string[] Extensions { get; set; }

        [Output]
        public ITaskItem SpecCount { get; set; }

        [Output]
        public string SpecFile { get; set; }

        public override bool Execute()
        {
            LogMessage("Language " + Language);
            SpecCount = new TaskItem("0");
            LogMessage("Project Dir :: " + ProjectDir);
            SpecFile = "ghost_bridge_specs" + GetExtension();
            var outputfile = Path.Combine(ProjectDir, SpecFile);
            LogMessage("Will write to : " + outputfile, MessageImportance.Low);
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


        string GetExtension()
        {
            var language = (Language ?? string.Empty);
            switch (language)
            {
                case "C#":
                    return ".cs";
                case "VB":
                    return ".vb";
                default:
                    throw new Exception("Language not recognised, supported languages : C# & VB");
            }
        }

        MSpecCodeProvider GetProvider(ProviderConfig config)
        {
            var language = (Language ?? string.Empty);
            switch (language)
            {
                case "C#":
                    return new CSharpMSpecProvider(config);
                case "VB":
                    return new VisualBasicMSpecProvider(config);
                default:
                    throw new Exception("Language not recognised, supported languages : C# & VB");
            }

        }

        public string CreateSpecs()
        {
            SpecCount = new TaskItem("0");
            int specCount;
            var config = new ProviderConfig
                {
                    BaseDirectory = BaseDirectory,
                    Extensions = Extensions,
                    Log = Log,
                    MyNamespace = GetType().Namespace,
                    TargetNamespace = Namespace ?? (string.IsNullOrWhiteSpace(ProjectDir) ? GetType().Namespace : (new DirectoryInfo(ProjectDir).Name)),
                    Pattern = Pattern
                };

            var provider = GetProvider(config);
            var result = provider.CreateSpecs(out specCount);
            SpecCount = new TaskItem(specCount.ToString());
            return result;
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