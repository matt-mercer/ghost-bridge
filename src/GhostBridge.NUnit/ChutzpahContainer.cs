using Chutzpah;
using Chutzpah.FileGenerator;
using Chutzpah.FileProcessors;
using Chutzpah.FrameworkDefinitions;
using Chutzpah.Utility;
using StructureMap;

namespace GhostBridge.NUnit
{
    public class ChutzpahContainer
    {
        private static readonly IContainer container = CreateContainer();

        public static IContainer Current
        {
            get
            {
                return container;
            }
        }

        static ChutzpahContainer()
        {
        }

        private static IContainer CreateContainer()
        {
            var container = new Container();
            container.Configure(config =>
                {
                    config.For<IHasher>().Singleton().Use<Hasher>();
                    config.For<ICompilerCache>().Singleton().Use<CompilerCache>();
                    config.Scan(scan =>
                        {
                            scan.AssemblyContainingType<TestRunner>();
                            scan.WithDefaultConventions();
                            scan.AddAllTypesOf<IFileGenerator>();
                            scan.AddAllTypesOf<IQUnitReferencedFileProcessor>();
                            scan.AddAllTypesOf<IJasmineReferencedFileProcessor>();
                            scan.AddAllTypesOf<IFrameworkDefinition>();
                        });
                });
            return container;
        }
    }
}