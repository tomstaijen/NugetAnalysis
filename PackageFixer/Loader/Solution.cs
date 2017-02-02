using System;
using System.Collections.Generic;
using System.Linq;

namespace PackageFixer
{
    public class Solution
    {
        public Solution(string pathToSolution)
        {
        }

        public string[] Projectnames { get; set; }
        public IDictionary<string,Project> Projects { get; set; }

        public IEnumerable<string> AllPackages
        {
            get
            {
                IEnumerable<string> result = new List<string>();

                foreach (var proj in Projects)
                {
                    result = result.Union(proj.Value.Packages.Select(p => p.Id));
                }

                return result.Distinct();
            }
        }

        public IDictionary<string, Version> PackagesThenVersion()
        {
            return null;
        }




        public IDictionary<string, IEnumerable<Project>> ProjectsByPackage
        {
            get
            {
                return AllPackages.ToDictionary(p => p,
                    p => Projects.Values.Where(proj => proj.Packages.Any(pack => pack.Id == p)));
            }
        }

        public string Name { get; internal set; }
    }
}