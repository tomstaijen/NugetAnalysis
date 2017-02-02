using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PackageFixer
{
    public class AssemblyCache
    {

        private static IDictionary<string, AssemblyReference> _assemblyReferences = new Dictionary<string, AssemblyReference>();

        public AssemblyReference GetAssembly(string path)
        {
            if (!_assemblyReferences.ContainsKey(path))
            {
                _assemblyReferences.Add(path, new AssemblyReference(path));
            }
            return _assemblyReferences[path];
        }

        public class AssemblyReference
        {
            private readonly string _hintPath;

            private AssemblyName[] _references;
            private Version _version;

            public string Name { get; private set; }

            public Assembly Assemby { get; private set; }

            public AssemblyReference(string hintPath)
            {
                _hintPath = hintPath;

                Assemby = Assembly.LoadFile(_hintPath);

                Name = Assemby.FullName.Split(',')[0];

                _version = Assemby.FullName.VersionFromAssemblyFullName();

                _references = Assemby.GetReferencedAssemblies();
            }

            public AssemblyName[] References
            {
                get { return _references; }
            }

            public string PublicKeyToken
            {
                get
                {
                    var parts = Assemby.FullName.Split(',');
                    var tokenPart = parts.Single(x => x.Trim().StartsWith("PublicKeyToken"));
                    var token = tokenPart.Split('=')[1];
                    return token;
                }
            }

            public Version Version { get { return _version; } }
        }

        public IEnumerable<AssemblyReference> GetCache(string assemblyName)
        {
            return _assemblyReferences.Values.Where(ar => ar.Name == assemblyName);
        }


    }

    public static class ExtensionsToAssemblyName
    {

        public static string ReferenceAssemblyFolder =
@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2";

        public static bool IsFrameworkAssembly(this AssemblyName asmName)
        {
            return File.Exists(Path.Combine(ReferenceAssemblyFolder, asmName.Name + ".dll"));
        }
    }

}