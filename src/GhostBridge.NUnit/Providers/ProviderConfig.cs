using Microsoft.Build.Utilities;

namespace GhostBridge.NUnit.Providers
{
    public class ProviderConfig
    {
        public string BaseDirectory { get; set; }
        public string Pattern { get; set; }
        public string TargetNamespace { get; set; }
        public string MyNamespace { get; set; }
        public string[] Extensions { get; set; }
        public TaskLoggingHelper Log { get; set; }
    }
}