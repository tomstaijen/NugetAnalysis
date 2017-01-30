using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace PackageFixer
{
    public class CsProjManager
    {
        public static IEnumerable<string> Projects(string sln)
        {
            var lines = File.ReadAllLines(sln);

            foreach (var line in lines)
            {
                // Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "NControl.DagelijkseSamenvatting", "NControl.DagelijkseSamenvatting\NControl.DagelijkseSamenvatting.csproj", "{D6A6AEB3-045E-4B33-8EA9-0B73C9157FA3}"
                if (line.StartsWith("Project"))
                {
                    var path = line.Split(',')[1];
                    if (!path.Contains('\\'))
                        continue;

                    var dir = path.Split('\\')[0].Replace("\"", "");
                    yield return dir.Trim();
                }
            }


        }

        public static IEnumerable<Reference> GetReferences(string csprojFile)
        {
            var result = new List<Reference>();

            if (!System.IO.File.Exists(csprojFile))
                throw new ArgumentException("packagesFile");

            System.Xml.XmlDocument project = new System.Xml.XmlDocument();
            project.Load(csprojFile);

            var manager = new XmlNamespaceManager(project.NameTable);
            manager.AddNamespace("p", "http://schemas.microsoft.com/developer/msbuild/2003");

            foreach (
                var node in
                project.DocumentElement.SelectNodes("/p:Project/p:ItemGroup/p:Reference", manager).Cast<XmlNode>())
            {
                var name = node.Attributes["Include"].Value;
                var hintPathNode = node.SelectNodes("p:HintPath", manager).Cast<XmlNode>();
                string hintPath = null;
                if (hintPathNode.Any())
                {
                    hintPath = hintPathNode.Single().InnerText;
                }
                result.Add(new Reference(name, hintPath));
            }

            return result;
        }
    }
}