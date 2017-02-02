using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

using PackageFixer.Analysis;

namespace PackageFixer
{
    class Program
    {

        static void Main(string[] args)
        {
            new Program().Run();
//            PrintLibs();
            Console.ReadKey();
        }

        public void Run()
        {
            var solution = SolutionLoader.Load(@"D:\projects\NControlLegacy\nc\Source\NControl\NControl.sln");

            new CheckPackageVersions().Check(solution);

            solution.AnalyzePackages("NControl.Bootstrap");
            solution.FindAssemblyRedirects("NControl.MijnNControl");
        }

        public static void PrintLibs()
        {
            var result = new List<string>();

            foreach (var s in Dir.Glob(@"D:\projects\NControlLegacy\nc\Source\NControl\*.sln"))
            {
                var sln = SolutionLoader.Load(s);

                Console.WriteLine("Processing solution " + sln.Name);

                result = result.Union(Libs(sln)).ToList();
            }



            result = result.Distinct().OrderBy(h => h).ToList();
            var libs = result.Where(p => p.StartsWith(@"D:\Projects\NControlLegacy\nc\Source\NControl\Lib"));
            foreach (var p in libs)
            {
                Console.WriteLine(p);
            }

            var dlls = Dir.Glob(@"D:\Projects\NControlLegacy\nc\Source\NControl\Lib\**\*.dll");
            foreach (var dll in dlls)
            {
                if (!libs.Contains(dll))
                {
                    Console.WriteLine($"Remove: {dll}");
                }
            }
        }

        public static IEnumerable<string> Libs(Solution solution)
        {
            foreach (var p in solution.Projects)
            {
                foreach (var r in p.Value.References)
                {
                    if (!string.IsNullOrEmpty(r.HintPath))
                    {
                        yield return p.Value.AbsolutePath(r.HintPath);
                    }
                }
            }
        }

        public void PrintAllPackages(Solution solution)
        {
            foreach (var p in solution.AllPackages.OrderBy(o => o))
            {
                Console.WriteLine(p);
            }
        }

        public void PrintHttpFormatting(Solution solution)
        {
            foreach (var p in solution.Projects.Values)
            {
                var reference = p.ReferenceByName("System.Net.Http.Formatting");
                if (reference != null)
                {
                    var package = p.PackageByReference(reference);
                    Console.WriteLine($"{p.Name} - {reference.Name} - {reference.Package} - {reference.PackageVersion} - {package?.Version} - {package?.TargetFramework}");
                }
            }
        }
    }
}
