using System;
using System.Diagnostics;
using System.Linq;

namespace PackageFixer
{
    [DebuggerDisplay("{Id} {Version} {TargetFramework}")]
    public class NugetPackage
    {
        public NugetPackage(string id, string version, string targetFramework)
        {
            Id = id;

            TargetFramework = TargetFrameworkToVersion(targetFramework);

            var intendedVersion = version.Split('-')[0];

            Version = new Version(intendedVersion);
        }

        public string Id { get; private set; }
        public Version Version { get; private set; }
        public Version TargetFramework { get; private set; }


        private static Version TargetFrameworkToVersion(string targetFramework)
        {
            string[] versionParts = {"0", "0", "0", "0"};
            int partIndex = 0;

            targetFramework
                .Replace("net", String.Empty)
                .Select(c => c.ToString())
                .ToList()
                .ForEach(v =>
                {
                    versionParts[partIndex++] = v;
                });

            return new Version(String.Join(".", versionParts));
        }
    }
}