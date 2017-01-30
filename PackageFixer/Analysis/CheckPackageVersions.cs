using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageFixer.Analysis
{
    class CheckPackageVersions
    {
        public void Check(Solution solution)
        {
            foreach (var package in solution.ProjectsByPackage)
            {
                var versions = package.Value.GroupBy(p => p.PackageById(package.Key).Version).ToDictionary(g => g.Key, g => g.ToList());
                if (versions.Count() > 1)
                {
                    Console.WriteLine($"Found multiple versions for package {package}");
                    foreach (var v in versions)
                    {
                        Console.WriteLine($"\t{v.Key}");
                        foreach (var x in v.Value)
                        {
                            Console.WriteLine($"\t\t{x.Name}");
                        }
                    }
                }

            }
        }
    }
}
