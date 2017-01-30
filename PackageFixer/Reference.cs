using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PackageFixer
{
    [DebuggerDisplay("{Name} {AssemblyVersion} {Framework}")]
    public class Reference
    {
        //                                                          ^([A-Za-z]+[0-9]*( \.[A-Za-z]+[0-9]*)*)\ .(          [0-9]+(\ .[0-9]+)*)
        private Regex _packageVersionMatcher = new Regex(@"^(?<package>[a-zA-Z][a-zA-Z0-9-]*(\.[a-zA-Z][a-zA-Z0-9-]*)*)\.(?<version>[0-9]+(\.[0-9]+)*)$");

        public Reference(string id, string hintPath)
        {
            Id = id;
            HintPath = hintPath;

        }
        public string Id { get; private set; }
        public string HintPath { get; private set; }


        public string Name
        {
            get { return Id.Split(',')[0]; }
        }

        public string PackageWithVersion
        {
            get
            {
                // ..\packages\Autofac.3.0.1\lib\net40\Autofac.dll
                if (!string.IsNullOrEmpty(HintPath) && HintPath.StartsWith("..") && HintPath.Contains("packages"))
                {
                    var dir = HintPath.Split('\\')[2];
                    return dir;
                }

                return string.Empty;
            }
        }

        public string PackageVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(PackageWithVersion))
                {
                    var matches = _packageVersionMatcher.Matches(PackageWithVersion);
                    return matches[0].Groups["version"].Value;
                }
                return string.Empty;
            }
        }

        public string Package
        {
            get
            {
                if (!string.IsNullOrEmpty(PackageWithVersion))
                {
                    var matches = _packageVersionMatcher.Matches(PackageWithVersion);
                    return matches[0].Groups["package"].Value;
                }
                return string.Empty;
            }
        }

        public string AssemblyVersion
        {
            get { return Id.VersionFromAssemblyFullName(); }
        }

        public string Framework
        {
            get
            {
                if (!string.IsNullOrEmpty(HintPath))
                {
                    if (HintPath.Contains("packages"))
                    {
                        return HintPath.Split('\\')[4];
                    }
                }
                return string.Empty;
            }
        }
    }

    public static class StringExtensions
    {
        public static string VersionFromAssemblyFullName(this string fullname)
        {
            if (fullname.Contains("Version="))
            {
                return fullname.Split(',')[1].Split('=')[1];
            }
            return "local";
        }
    }
}