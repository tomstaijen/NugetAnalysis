using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PackageFixer
{
    [DebuggerDisplay("{Name}")]
    public class Project
    {
        private readonly string _csprojPath;

        public Project(string csprojPath)
        {
            _csprojPath = csprojPath;
        }

        public string AbsolutePath(string projectRelativePath)
        {
            var dir = new FileInfo(_csprojPath).DirectoryName;
            var fi = new FileInfo(Path.Combine(dir, projectRelativePath));
            return fi.FullName;
        }

        public string Name { get; set; }

        public ICollection<NugetPackage> Packages { get; set; }
        public ICollection<Reference> References { get; set; }

        public IEnumerable<Reference> ReferenceByPackage(NugetPackage package)
        {
            return References.Where(r => r.Package == package.Id);
        }

        public Reference ReferenceByName(string name)
        {
            return References.SingleOrDefault(r => r.Name == name);
        }

        public NugetPackage PackageById(string id)
        {
            return Packages.SingleOrDefault(s => s.Id == id);
        }

        public NugetPackage PackageByReference(Reference reference)
        {
            return Packages.SingleOrDefault(p => p.Id == reference.Package);
        }
    }
}