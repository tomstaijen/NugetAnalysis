using System;
using System.Collections.Generic;
using System.Reflection;

namespace PackageFixer
{
    public class AssemblyCache
    {
        public static string ReferenceAssemblyFolder =
            @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework";

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

            public Version Version { get { return _version; } }
        }
    }
}