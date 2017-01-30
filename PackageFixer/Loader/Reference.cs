using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PackageFixer
{
    [DebuggerDisplay("{Name} {AssemblyVersion} {Framework}")]
    public class Reference
    {
        //                                                          ^([A-Za-z]+[0-9]*( \.[A-Za-z]+[0-9]*)*)\ .(          [0-9]+(\ .[0-9]+)*)
        private Regex _packageVersionMatcher = new Regex(@"^(?<package>[a-zA-Z][a-zA-Z0-9-]*(\.[a-zA-Z][a-zA-Z0-9-]*)*)\.(?<version>[0-9]+(\.[0-9]+)*)(-.*)?$");

        public Reference(Project project, string id, string hintPath)
        {
            Project = project;
            Id = id;
            HintPath = hintPath;
        }

        public Project Project { get; private set; }

        public string Id { get; private set; }
        public string HintPath { get; private set; }


        public string Name
        {
            get { return Id.Split(',')[0]; }
        }

        private string PackageAndVersionFromHintPath
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

        public Version PackageVersion
        {
            get
            {
                if (!string.IsNullOrEmpty(PackageAndVersionFromHintPath))
                {
                    var matches = _packageVersionMatcher.Matches(PackageAndVersionFromHintPath);
                    return new Version(matches[0].Groups["version"].Value);
                }
                return null;
            }
        }

        public string Package
        {
            get
            {
                if (!string.IsNullOrEmpty(PackageAndVersionFromHintPath))
                {
                    var matches = _packageVersionMatcher.Matches(PackageAndVersionFromHintPath);
                    return matches[0].Groups["package"].Value;
                }
                return string.Empty;
            }
        }

        public Version AssemblyVersion
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
        public static Version VersionFromAssemblyFullName(this string fullname)
        {
            if (fullname.Contains("Version="))
            {
                return new Version(fullname.Split(',')[1].Split('=')[1]);
            }
            return null;
        }
    }
}