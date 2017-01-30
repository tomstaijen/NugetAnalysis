using System.Collections.Generic;
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
            private string _version;

            public AssemblyReference(string hintPath)
            {
                _hintPath = hintPath;

                var asm = Assembly.LoadFile(_hintPath);

                var name = asm.FullName.Split(',')[0];

                _version = asm.FullName.VersionFromAssemblyFullName();

                _references = asm.GetReferencedAssemblies();
            }

            public AssemblyName[] References
            {
                get { return _references; }
            }

            public string Version { get { return _version; } }
        }
    }
}