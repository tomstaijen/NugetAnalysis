using System;
using System.Collections.Generic;
using System.Linq;

namespace PackageFixer
{
    public static class PackagesConfigReader
    {
        public static IEnumerable<NugetPackage> GetDependencies(string packagesFile)
        {
            if (!System.IO.File.Exists(packagesFile))
                throw new ArgumentException("packagesFile");

            System.Xml.XmlDocument packages = new System.Xml.XmlDocument();
            var lines = System.IO.File.ReadAllLines(packagesFile);
            var content = string.Join(Environment.NewLine, lines);
            content = content.Replace("utf-8", "utf-16");
            packages.LoadXml(content);
            return packages.DocumentElement.SelectNodes("/packages/package").Cast<System.Xml.XmlNode>().Select(x => new NugetPackage(x.Attributes["id"].Value, x.Attributes["version"].Value, x.Attributes["targetFramework"].Value)).ToList();
        }
    }
}