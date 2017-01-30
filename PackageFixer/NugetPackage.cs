using System.Diagnostics;

namespace PackageFixer
{
    [DebuggerDisplay("{Id} {Version} {TargetFramework}")]
    public class NugetPackage
    {
        public NugetPackage(string id, string version, string targetFramework)
        {
            Id = id;
            Version = version;
            TargetFramework = targetFramework;
            while (Version.Split('.').Length < 4)
            {
                Version = $"{Version}.0";
            }
        }

        public string Id { get; private set; }
        public string Version { get; private set; }
        public string TargetFramework { get; private set; }

    }
}